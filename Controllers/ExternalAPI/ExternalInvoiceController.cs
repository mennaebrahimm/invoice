using invoice.Core.DTO.Invoice;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers.ExternalAPI
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/external/[controller]")]
    public class ExternalInvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public ExternalInvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }
        private string GetExternalUserId() => HttpContext.Items["ExternalUserId"]?.ToString();


        
        [HttpPost]
        public async Task<IActionResult> ExternalCreate([FromBody] InvoiceCreateDTO dto)
        {

            var userId = GetExternalUserId();

            if (userId == null)
                return Unauthorized("API key required");

            var response = await _invoiceService.CreateAsync(dto, userId);
            return Ok(response);
        }





    }
}
