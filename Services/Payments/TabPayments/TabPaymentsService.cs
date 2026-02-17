using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.DTO.PaymentResponse.TapPayments;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Helpers;
using invoice.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;



namespace invoice.Services.Payments.TabPayments
{
    public class TabPaymentsService : PaymentGatewayBase, IPaymentGateway
    {
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IRepository<TapPaymentsPayout> _tapPaymentsPayoutRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TabPaymentsService> _logger;
        private readonly TabPaymentsOptions _options;
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private readonly string _MarketplaceId;
        private readonly string _Domain;
        private readonly IMapper _mapper;
        private readonly IRepository<ApplicationUser> _ApplicationUserRepo;


        public override PaymentType PaymentType => PaymentType.TabPayments;

        public TabPaymentsService(
           IRepository<ApplicationUser> ApplicationUserRepo,
            IConfiguration configuration,
            ILogger<TabPaymentsService> logger,
            IOptions<TabPaymentsOptions> options,
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            IInvoiceRepository invoiceRepo, UserManager<ApplicationUser> userManager, IRepository<TapPaymentsPayout> tapPaymentsPayoutRepo)
            : base(configuration)
        {
            _ApplicationUserRepo = ApplicationUserRepo;
            _options = options.Value;
            _invoiceRepo = invoiceRepo;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _tapPaymentsPayoutRepo = tapPaymentsPayoutRepo;
            _secretKey = configuration["TapSettings:SecretKey"];
            _MarketplaceId = configuration["TapSettings:MarketplaceId"];
            _Domain = configuration["AppSettings:BaseUrl"];
            

            if (string.IsNullOrWhiteSpace(_options.BaseUrl))
                _options.BaseUrl = "https://api.tap.company";

            if (string.IsNullOrWhiteSpace(_options.SecretKey))
                throw new ArgumentException("Tap SecretKey is required");

            _httpClient = httpClientFactory.CreateClient("TabPayments");

            try { _httpClient.BaseAddress = new Uri(_options.BaseUrl); }
            catch { _httpClient.BaseAddress = null; }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.SecretKey}");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("InvoiceApp/1.0");
        }

        #region  Tap_Onboardin

        public async Task<GeneralResponse<object>> CreateLeadRetailerAsync(CreateLeadDTO dto, string userId)
        {
            if (dto == null)
            {
                return  new GeneralResponse<object>(false, "Invalid request: user data is required", null);
            }
            var user = await _ApplicationUserRepo.GetByIdAsync(userId);
            if (user == null)
            {
                return new GeneralResponse<object>(false, "User not found", null);
            }
            if (user.TabAccountId != null)
            {
                return new GeneralResponse<object>(false, "Invalid request: user already have account on tap payments", null);

            }
            var filedto = new CreateFileDTO
            {
                Purpose = "business_logo",
                Title = dto.brand.logo.FileName,
                ExpiresAt = DateTime.UtcNow.AddYears(1),
                FileLinkCreate = true,
                File = dto.brand.logo

            };
            var fileResult = await CreateFileAsync(filedto);
            if (!fileResult.Success)
            {
                return new GeneralResponse<object>(false, $"Failed to upload logo file {fileResult.Message}");
            }
            var fileId = fileResult.Data?.ToString();
     
            var requestBody = new 
            {
                segment = new
                {
                    type = "BUSINESS",
                    sub_segment = new
                    {
                        type = "RETAILER"

                    }
                },
                country = dto.country,

                brand = new 
                {
                    name = dto.brand.name,
                    //logo = "file_SnT71325710rfos29W611a274",   // for testing
                    logo = fileId,

                    channel_services = dto.brand.channel_services


                },

                entity = new Entity
                {
                    legal_name = dto.entity.legal_name,

                    license = dto.entity.license,
                },
                users = dto.users.Select(u => new User
                {
                    name = u.name,
                    identification = u.identification,
                    primary = u.primary,
                    Contact = new ContactDto
                    {
                        Email = u.Contact?.Email,
                        Phone = u.Contact?.Phone
                    }
                }).ToList(),


                wallet = new Wallet
                {
                    name = dto.wallet.name,

                    linked_financial_account = dto.wallet.linked_financial_account,
                },
                marketplace = new
                {
                    id = _MarketplaceId
                },
                post = new
                {
                    url = $"{_Domain}/api/Payments/onboarding-webhook"
                },
            };



            try
            {

                var response = await _httpClient.PostAsJsonAsync("/v3/lead", requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogError("Failed to create lead. Status code: {StatusCode}. Error details: {ErrorContent}", response.StatusCode, errorContent);
                    return new GeneralResponse<object>(
                        false,
                        $"Failed to create lead. Status code: {response.StatusCode}. Error details: {errorContent}",
                        null);


                }
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response Body: " + responseBody);

                var json = await response.Content.ReadFromJsonAsync<JsonElement>();

           
                if (json.TryGetProperty("id", out var leadId))
                {
                    string leadIdValue = leadId.GetString();
                    if (string.IsNullOrEmpty(leadIdValue))
                    {
                        _logger.LogWarning("Lead ID is empty.");
                        return new GeneralResponse<object>(false, "Lead ID is empty.", null);
                    }
                    else
                    {
                        _logger.LogInformation("Lead ID: {LeadId}", leadIdValue);
                   
                        return await CreateAccountRetailerAsync(leadIdValue, userId);
                    }
                }
                else
                {
                    _logger.LogError("The 'id' key was not found in the response.");
                    return new GeneralResponse<object>(false, "Lead ID not returned from Tap", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating Tap retailer for user {UserId}", userId);
                return new GeneralResponse<object>(false, $"Error while creating Tap retailer for user {userId}", ex.Message);
            }
        }

        public async Task<GeneralResponse<object>> CreateAccountRetailerAsync(string leadId ,string userId)
        {

            var body = new
            {
                lead_id = leadId

            };
           
            var response = await _httpClient.PostAsJsonAsync("/v3/connect/account", body);

            if (!response.IsSuccessStatusCode)
                return new GeneralResponse<object>(false, $"failed to create Account Retailer:{response.StatusCode}");
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response Body: " + responseBody);

            Console.WriteLine("Response Body: " + responseBody);
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var retailerid = json.GetProperty("retailer").GetProperty("id").GetString();
            if (string.IsNullOrEmpty(retailerid))
            {
                return new GeneralResponse<object>(false, "Retailer ID not returned from Tap");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new GeneralResponse<object>(false, "user not found");
            user.TabAccountId = retailerid;
            await _userManager.UpdateAsync(user);
            return   new GeneralResponse<object>(true, "TabAccount Id saved successfully");


        }
        public async Task<GeneralResponse<object>> CreateFileAsync(CreateFileDTO dto)
        {
            if (dto?.File == null || dto.File.Length == 0)
                return new GeneralResponse<object>(false, "File is required");

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(dto.Purpose), "purpose");
            content.Add(new StringContent(dto.Title), "title");

            var expiresAtUnix =
                new DateTimeOffset(dto.ExpiresAt).ToUnixTimeSeconds();
            content.Add(new StringContent(expiresAtUnix.ToString()), "expires_at");

            content.Add(
                new StringContent(dto.FileLinkCreate.ToString().ToLower()),
                "file_link_create"
            );

            using var stream = dto.File.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(dto.File.ContentType);

            content.Add(fileContent, "file", dto.File.FileName);

            var response = await _httpClient.PostAsync("/v2/files/", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return new GeneralResponse<object>(
                    false, $"failed to create file: {error}");
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var fileId = json.GetProperty("id").GetString();

            if (string.IsNullOrEmpty(fileId))
                return new GeneralResponse<object>(
                    false, "file ID not returned from Tap");

            return new GeneralResponse<object>(
                true, "file created successfully", fileId);
        }


        //public async Task<string?> CreateConnectUrlAsync(string leadId ,string userId)
        //{

        //    var dto = new CreateConnectDto
        //    { 

        //        Lead = new LeadRefDto { Id = leadId },
        //        Redirect = new RedirectDto { Url = "https://yourwebsite.com/success" },
        //        Post = new PostDto { Url  = $"https://myinvoice.runasp.net/api/Payments/onboarding-success?userid={userId}",

        //        }
        //    };
        //    //var dto = new
        //    //{
        //    //      scope = "merchant",
        //    //    lead = new
        //    //    {
        //    //        id= leadId
        //    //    },

        //    //     post = new
        //    //     {
        //    //         url = "https://myinvoice.runasp.net/api/Payments/createcharge-success",

        //    //     },

        //    //    redirect = new
        //    //    {
        //    //        url = "https://myinvoice.runasp.net"
        //    //    }
        //    //};

        //    var response = await _httpClient.PostAsJsonAsync("/v3/connect", dto);


        //    if (!response.IsSuccessStatusCode)
        //        return null;
        //    //var raw = await response.Content.ReadAsStringAsync();
        //    //Console.WriteLine("RAW TAP RESPONSE: " + raw);

        //    var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        //    return json.GetProperty("connect").GetProperty("url").GetString(); // connect_onboarding URL
        //}



        #endregion

        #region create payment
        public async Task<GeneralResponse<string>> CreatePaymentAsync(CreateChargeDTO dto)
        {
            if (dto == null)
            {
                return new GeneralResponse<string>(false, "Invalid request: Payment data is required", null);
            }

            //checks

            var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId,null,q => q
              .Include(x => x.Client)
              .Include(x => x.User)
               );


            if (invoice==null)
            {
                return new GeneralResponse<string>(false, "Invalid request: invalid invoice id", null);

            }
            if (string.IsNullOrEmpty(invoice.User.TabAccountId))
            {
                return new GeneralResponse<string>(
                    false, "Merchant Tap account is not configured", null);
            }

            var commission = invoice.FinalValue * 0.05m; 
            var merchantAmount = invoice.FinalValue - commission;

            var body = new
            {
                amount = invoice.FinalValue,
                currency=invoice.Currency,
                customer_initiated = true,
                threeDSecure = true,
                save_card = false,
                description = $"your invoice id is {invoice.Id}",
                metadata = new
                {
                    invoiceid = invoice.Id
                },

                destinations = new
                {
                    destination = new[] {
                    new {
                            id = invoice.User.TabAccountId, 
                           // id = "67989550",   //for testing
                            amount = merchantAmount,
                            currency = invoice.Currency
                        }
                    }
                },
                receipt = new
                {
                    email=true,
                    sms=false
                },
                customer = new
                {
                    first_name = invoice.Client.Name,
                    email = invoice.Client.Email,
                    //phone = invoice.Client.PhoneNumber,

                },
                merchant = new
                {
                    id = _MarketplaceId,

                },
                source = new
                {
                    id = "src_all"
                },
                post = new
                {
                    url = $"{_Domain}/api/Payments/payout-webhook",

                },

            redirect = new
                {
                    url = $"{_Domain}/PaymentSuccess"
            },
               

            };

            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://api.tap.company/v2/charges", content);
                var result = await response.Content.ReadAsStringAsync();

                _logger.LogInformation(
             "Tap response received. StatusCode: {StatusCode}, Body: {ResponseBody}",
             response.StatusCode,
             result
                 );

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "Tap payment failed. InvoiceId: {InvoiceId}, Response: {ResponseBody}",
                        invoice.Id,
                        result
                    );

                    return new GeneralResponse<string>(
                        false,
                        "Failed to create payment",
                        null
                    );
                }

                var charge = JsonConvert.DeserializeObject<dynamic>(result);
                string redirectUrl = charge?.transaction?.url;

                _logger.LogInformation(
                    "Payment created successfully. InvoiceId: {InvoiceId}, RedirectUrl: {RedirectUrl}",
                    invoice.Id,
                    redirectUrl
                );

                return new GeneralResponse<string>(
                    true,
                    "redirect Url created successfully",
                    redirectUrl
                );
            
            }
            catch (Exception ex)
            {
                _logger.LogError(
            ex,
            "Exception occurred while creating payment. InvoiceId: {InvoiceId}",
            dto.InvoiceId);
                return new GeneralResponse<string>(
                    false,
                    $"Error: {ex.Message}", null
                );
            }
        }


        #endregion
        #region Payout Webhook 
        public async Task<GeneralResponse<object>> WebhookAsync(PayoutWebhookDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
                return new GeneralResponse<object>(false, "dto or id not found");

            var strategy = _tapPaymentsPayoutRepo.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _tapPaymentsPayoutRepo.BeginTransactionAsync();

                try
                {
                    var payout = await _tapPaymentsPayoutRepo
                        .GetSingleByPropertyAsync(x => x.PayoutId == dto.Id);

                    var alreadyCompleted = false;

                    if (payout == null)
                    {
                        payout = _mapper.Map<TapPaymentsPayout>(dto);
                        payout.CreatedAt = GetSaudiTime.Now();

                        await _tapPaymentsPayoutRepo.AddAsync(payout);

                        _logger.LogInformation(
                            "Payout created: {PayoutId}", dto.Id);
                    }
                    else
                    {
                        if (payout.Status == "PAID_OUT")
                        {
                            alreadyCompleted = true;
                        }
                        else
                        {
                            _mapper.Map(dto, payout);
                            payout.UpdatedAt = GetSaudiTime.Now();

                            await _tapPaymentsPayoutRepo.UpdateAsync(payout);

                            _logger.LogInformation(
                                "Payout updated: {PayoutId}, Status: {Status}",
                                dto.Id, dto.Status);
                        }
                    }

                    if (!alreadyCompleted && dto.Status == "PAID_OUT")
                    {
                        var invoiceId = dto.Metadata?.GetValueOrDefault("invoiceid");

                        if (!string.IsNullOrWhiteSpace(invoiceId))
                        {
                            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);

                            if (invoice != null &&
                                invoice.InvoiceStatus != InvoiceStatus.Paid)
                            {
                                invoice.InvoiceStatus = InvoiceStatus.Paid;
                                await _invoiceRepo.UpdateAsync(invoice);
                            }
                        }
                    }

                    await _tapPaymentsPayoutRepo.CommitTransactionAsync(transaction);

                    return alreadyCompleted
                        ? new GeneralResponse<object>(true, "Payout already completed")
                        : new GeneralResponse<object>(true, "Payout saved successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    _logger.LogError(ex,
                        "Error processing payout webhook. PayoutId: {PayoutId}",
                        dto?.Id);

                    throw; 
                }
            });
        }

        #endregion

        public override async Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(_options.BaseUrl) && await TestDomain(_options.BaseUrl))
            {
                var result = await TryCreateSessionWithDomain(dto, _options.BaseUrl);
                if (result.Success) return result;
            }

            var fallbackDomains = new[] { "https://api.tap.company", "https://api.tap.company/" };
            foreach (var domain in fallbackDomains)
            {
                var res = await TryCreateSessionWithDomain(dto, domain);
                if (res.Success) return res;
            }

            return await HandleTabPaymentsConfigurationError(dto);
        }

        private async Task<bool> TestDomain(string domain)
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(Math.Max(5, _options.TimeoutSeconds)) };
                var resp = await client.GetAsync(NormalizeDomain(domain));
                return resp.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        private static string NormalizeDomain(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl)) return string.Empty;
            var u = baseUrl.Trim().TrimEnd('/');
            if (!u.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                u = "https://" + u;
            return u;
        }

        private async Task<GeneralResponse<PaymentSessionResponse>> HandleTabPaymentsConfigurationError(PaymentCreateDTO dto)
        {
            return new GeneralResponse<PaymentSessionResponse>(
                false,
                $"Tap service unavailable. Merchant ID: {_options.MerchantId}, Test Mode: {_options.TestMode}"
            );
        }

        private async Task<GeneralResponse<PaymentSessionResponse>> TryCreateSessionWithDomain(PaymentCreateDTO dto, string baseUrl)
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            try
            {
                var validation = ValidatePaymentRequest(dto);
                if (!validation.Success) return validation;

                var amountInCents = (long)Math.Round(dto.Cost * 100M);
                if (amountInCents < 1)
                    return new GeneralResponse<PaymentSessionResponse>(false, "Payment amount too small");

                var domain = NormalizeDomain(baseUrl);

                // --- 🟢 Detect P2P mode ---
                Core.Entities.Invoice? invoice = null;
                if (!string.IsNullOrEmpty(dto.InvoiceId))
                    invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId);

                var sellerAccountId = GetSellerTabAccountId(dto, invoice);
                var isP2P = !string.IsNullOrEmpty(sellerAccountId);

                var payload = new Dictionary<string, object?>
                {
                    ["amount"] = amountInCents,
                    ["currency"] = (dto.Currency ?? "SAR").ToUpperInvariant(),
                    ["merchant"] = new Dictionary<string, object?> { ["id"] = _options.MerchantId },
                    ["source"] = new Dictionary<string, object?> { ["id"] = "src_all" },
                    ["redirect"] = new Dictionary<string, object?> { ["url"] = $"{domain}/payments/success?invoice={Uri.EscapeDataString(dto.InvoiceId)}" },
                    ["post"] = new Dictionary<string, object?> { ["url"] = $"{domain}/api/webhooks/tap" },
                    ["customer"] = new Dictionary<string, object?>
                    {
                        ["first_name"] = dto.ClientId ?? "Customer",
                        ["email"] = dto.ClientEmail ?? string.Empty
                    },
                    ["metadata"] = CreateTabMetadata(dto, amountInCents, isP2P)
                };

                // --- 🟢 Add P2P destination if seller found ---
                if (isP2P)
                {
                    payload["destination"] = new Dictionary<string, object?>
                    {
                        ["merchant"] = new Dictionary<string, object?> { ["id"] = sellerAccountId },
                        ["amount"] = amountInCents
                    };
                }

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload, jsonOptions), Encoding.UTF8, "application/json");
                var endpoints = new[] { "/v2/charges/", "/v2/charges" };

                foreach (var ep in endpoints)
                {
                    var fullUrl = domain + ep;
                    using var req = new HttpRequestMessage(HttpMethod.Post, fullUrl) { Content = content };
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.SecretKey);

                    using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
                    var body = await resp.Content.ReadAsStringAsync();

                    if (!resp.IsSuccessStatusCode)
                    {
                        continue;
                    }

                    string? sessionId = null;
                    string? paymentUrl = null;
                    DateTime? expiresAt = null;

                    try
                    {
                        using var doc = JsonDocument.Parse(body);
                        var root = doc.RootElement;
                        sessionId = root.GetProperty("id").GetString();
                        if (root.TryGetProperty("transaction", out var tx))
                        {
                            paymentUrl = tx.GetProperty("url").GetString();
                        }
                        else if (root.TryGetProperty("url", out var urlProp))
                        {
                            paymentUrl = urlProp.GetString();
                        }
                        if (root.TryGetProperty("expires_at", out var expProp))
                        {
                            DateTime.TryParse(expProp.GetString(), out var dt);
                            expiresAt = dt.ToUniversalTime();
                        }
                    }
                    catch { }

                    var responseData = new PaymentSessionResponse
                    {
                        SessionId = sessionId ?? Guid.NewGuid().ToString(),
                        PaymentUrl = paymentUrl ?? string.Empty,
                        ExpiresAt = expiresAt ?? DateTime.UtcNow.AddHours(1),
                        PaymentType = PaymentType.TabPayments,
                        PaymentStatus = PaymentStatus.Pending,
                        InvoiceId = dto.InvoiceId,
                        Amount = dto.Cost,
                        Currency = (dto.Currency ?? "SAR").ToUpperInvariant(),
                        RawResponse = body
                    };

                    var msg = string.IsNullOrEmpty(paymentUrl)
                        ? "Charge created but no redirect URL returned. Check RawResponse."
                        : "Payment session created successfully";

                    return new GeneralResponse<PaymentSessionResponse>(true, msg, responseData);
                }

                return new GeneralResponse<PaymentSessionResponse>(false, "No working Tap endpoint found for this domain");
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentSessionResponse>(false, $"Domain unreachable: {ex.Message}");
            }
        }

        private Dictionary<string, object> CreateTabMetadata(PaymentCreateDTO dto, long sellerAmount, bool isP2P)
        {
            var metadata = new Dictionary<string, object>
            {
                ["invoice_id"] = dto.InvoiceId,
                ["seller_amount"] = sellerAmount,
                ["is_p2p"] = isP2P,
                ["merchant_id"] = _options.MerchantId,
                ["test_mode"] = _options.TestMode
            };

            if (dto.Metadata != null)
                foreach (var item in dto.Metadata)
                    metadata[item.Key] = item.Value ?? string.Empty;

            return metadata;
        }

        private string? GetSellerTabAccountId(PaymentCreateDTO dto, Core.Entities.Invoice? invoice = null)
        {
            if (dto.Metadata != null && dto.Metadata.TryGetValue("seller_tab_account_id", out var accountId) && !string.IsNullOrWhiteSpace(accountId))
                return accountId;

            if (!string.IsNullOrWhiteSpace(invoice?.User?.TabAccountId))
                return invoice.User.TabAccountId;

            return null;
        }

        public override async Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paymentId)) return new GeneralResponse<bool>(false, "PaymentId required");

                var baseUrl = NormalizeDomain(_options.BaseUrl);
                var endpoint = $"{baseUrl}/v2/charges/{Uri.EscapeDataString(paymentId)}/refund";
                using var req = new HttpRequestMessage(HttpMethod.Post, endpoint);
                req.Headers.Remove("Authorization");
                req.Headers.Add("Authorization", $"Bearer {_options.SecretKey}");
                req.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                using var resp = await _httpClient.SendAsync(req);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    return new GeneralResponse<bool>(false, $"Failed to cancel/refund payment: {body}");
                }

                return new GeneralResponse<bool>(true, "Payment cancelled/refunded successfully", true);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>(false, $"Failed to cancel payment: {ex.Message}");
            }
        }

        public override async Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paymentId)) return new GeneralResponse<PaymentStatusResponse>(false, "PaymentId required");

                var baseUrl = NormalizeDomain(_options.BaseUrl);
                var endpoint = $"{baseUrl}/v2/charges/{Uri.EscapeDataString(paymentId)}";
                using var req = new HttpRequestMessage(HttpMethod.Get, endpoint);
                req.Headers.Remove("Authorization");
                req.Headers.Add("Authorization", $"Bearer {_options.SecretKey}");

                using var resp = await _httpClient.SendAsync(req);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    return new GeneralResponse<PaymentStatusResponse>(false, $"Failed to retrieve payment status: {body}");
                }

                string providerStatus = string.Empty;
                try
                {
                    using var doc = JsonDocument.Parse(body);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("transaction", out var tx) && tx.TryGetProperty("status", out var st))
                        providerStatus = st.GetString() ?? string.Empty;
                    else if (root.TryGetProperty("status", out var s2))
                        providerStatus = s2.GetString() ?? string.Empty;
                }
                catch { }

                var mapped = MapTabStatus(providerStatus);

                var statusResponse = new PaymentStatusResponse
                {
                    PaymentId = paymentId,
                    Status = mapped,
                    LastUpdated = DateTime.UtcNow,
                    RawResponse = body
                };

                return new GeneralResponse<PaymentStatusResponse>(true, "Status retrieved successfully", statusResponse);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaymentStatusResponse>(false, $"Failed to get payment status: {ex.Message}");
            }
        }

        public override async Task<GeneralResponse<bool>> HandleWebhookAsync(WebhookPayloadDTO payload)
        {
            try
            {
                if (payload == null || string.IsNullOrEmpty(payload.RawPayload))
                    return new GeneralResponse<bool>(false, "Webhook payload is required");

                using var doc = JsonDocument.Parse(payload.RawPayload);
                var root = doc.RootElement;

                string chargeId = string.Empty;
                string status = string.Empty;
                string invoiceId = string.Empty;
                decimal amount = 0;
                string currency = "SAR";

                // Parse Tab webhook structure
                if (root.TryGetProperty("id", out var idProp))
                    chargeId = idProp.GetString() ?? string.Empty;

                if (root.TryGetProperty("status", out var statusProp))
                    status = statusProp.GetString() ?? string.Empty;

                if (root.TryGetProperty("amount", out var amountProp) && amountProp.ValueKind == JsonValueKind.Number)
                    amount = amountProp.GetDecimal() / 100M;

                if (root.TryGetProperty("currency", out var currencyProp))
                    currency = currencyProp.GetString() ?? "SAR";

                // Extract invoice ID from metadata
                if (root.TryGetProperty("metadata", out var metadataProp))
                {
                    if (metadataProp.TryGetProperty("invoice_id", out var invoiceIdProp))
                        invoiceId = invoiceIdProp.GetString() ?? string.Empty;
                }

                // Alternative: check transaction object
                if (string.IsNullOrEmpty(invoiceId) && root.TryGetProperty("transaction", out var txProp))
                {
                    if (txProp.TryGetProperty("metadata", out var txMetadataProp) &&
                        txMetadataProp.TryGetProperty("invoice_id", out var txInvoiceIdProp))
                        invoiceId = txInvoiceIdProp.GetString() ?? string.Empty;
                }

                if (string.IsNullOrEmpty(chargeId))
                    return new GeneralResponse<bool>(false, "Charge ID not found in webhook payload");

                if (string.IsNullOrEmpty(invoiceId))
                    return new GeneralResponse<bool>(false, "Invoice ID not found in webhook payload");

                // Map Tab status to our payment status
                var paymentStatus = MapTabStatus(status);

                // Update invoice status if payment is completed
                if (paymentStatus == PaymentStatus.Completed)
                {
                    if (_invoiceRepo == null)
                        return new GeneralResponse<bool>(false, "Invoice repository not available");

                    var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);
                    if (invoice == null)
                        return new GeneralResponse<bool>(false, $"Invoice {invoiceId} not found");

                    // Update invoice as paid
                    invoice.InvoiceStatus = InvoiceStatus.Paid;
                    invoice.FinalValue = amount;
                    invoice.UpdatedAt = GetSaudiTime.Now();

                    // Update invoice in repository
                    var updateResult = await _invoiceRepo.UpdateAsync(invoice);

                    return new GeneralResponse<bool>(true, "Invoice successfully marked as paid", true);
                }
                else if (paymentStatus == PaymentStatus.Failed || paymentStatus == PaymentStatus.Cancelled)
                {
                    if (_invoiceRepo != null)
                    {
                        var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);
                        if (invoice != null)
                        {
                            invoice.InvoiceStatus = InvoiceStatus.Paid;
                            invoice.UpdatedAt = GetSaudiTime.Now();
                            await _invoiceRepo.UpdateAsync(invoice);
                        }
                    }
                    return new GeneralResponse<bool>(true, $"Payment {paymentStatus} for invoice {invoiceId}", true);
                }
                else
                {
                    // Log other statuses (pending, etc.)
                    return new GeneralResponse<bool>(true, $"Webhook received - Invoice: {invoiceId}, Status: {paymentStatus}", true);
                }
            }
            catch (System.Text.Json.JsonException jsonEx)
            {
                return new GeneralResponse<bool>(false, $"Invalid JSON in webhook payload: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>(false, $"Error processing webhook: {ex.Message}");
            }
        }


        private PaymentStatus MapTabStatus(string tabStatus)
        {
            if (string.IsNullOrWhiteSpace(tabStatus)) return PaymentStatus.Pending;

            var normalized = tabStatus.Trim().ToLowerInvariant();
            return normalized switch
            {
                "captured" or "succeeded" or "successful" or "paid" => PaymentStatus.Completed,
                "pending" or "authorized" or "in_progress" => PaymentStatus.Pending,
                "failed" or "declined" or "error" => PaymentStatus.Failed,
                "cancelled" or "canceled" => PaymentStatus.Cancelled,
                "refunded" or "reversed" => PaymentStatus.Refunded,
                _ => PaymentStatus.Pending
            };
        }
    }
}
