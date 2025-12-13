using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Product;
using invoice.Core.DTO.Tax;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;


namespace invoice.Services
{
    public class TaxService : ITaxService
    {
        private readonly IRepository<Tax> _taxRepo;
        private readonly IMapper _mapper;

        public TaxService(IRepository<Tax> taxRepo, IMapper mapper)
        {
            _taxRepo = taxRepo;
            _mapper = mapper;
        }
        public async Task<GeneralResponse<TaxReadDTO>> CreateAsync(TaxReadDTO dto, string userId)
        {
            var taxDB = await _taxRepo.GetAllAsync(userId);
            if (taxDB.Any())
            {

                return new GeneralResponse<TaxReadDTO>(false, "this user have tax can update it only", null);

            }

            var tax = _mapper.Map<Tax>(dto);
            tax.UserId = userId;

            await _taxRepo.AddAsync(tax);
            var readDto = _mapper.Map<TaxReadDTO>(tax);

            return new GeneralResponse<TaxReadDTO>(true, "tax created successfully", readDto);

        }


        public async Task<GeneralResponse<TaxReadDTO>> GetByUserIdAsync(string userId)
        {
            var tax = (await _taxRepo.QueryAsync(p => p.UserId == userId)).FirstOrDefault();

            if (tax == null)
                return new GeneralResponse<TaxReadDTO>(false, "No tax found for this user", null);

            var dto = _mapper.Map<TaxReadDTO>(tax);
            return new GeneralResponse<TaxReadDTO>(true, "tax retrieved successfully", dto);
        }


        public async Task<GeneralResponse<TaxReadDTO>> UpdateAsync(TaxReadDTO dto, string userId)
        {
            if (dto == null || string.IsNullOrWhiteSpace(userId))
                return new GeneralResponse<TaxReadDTO>(false, "Invalid tax data");

            var tax = await _taxRepo.GetSingleByUserIdAsync(userId);
            if (tax == null)
                return new GeneralResponse<TaxReadDTO>(false, "Tax not found");

            var strategy = _taxRepo.CreateExecutionStrategy();

            return await strategy.ExecuteAsync<object, GeneralResponse<TaxReadDTO>>(
            state: null,

            operation: async (context, state, cancellationToken) =>
            {

              var transaction = await _taxRepo.BeginTransactionAsync();

                    try
                    {
                        await _taxRepo.DeleteAsync(tax.Id);

                        var newTax = _mapper.Map<Tax>(dto);
                        newTax.UserId = userId;

                        await _taxRepo.AddAsync(newTax);

                        await _taxRepo.CommitTransactionAsync(transaction);

                        return new GeneralResponse<TaxReadDTO>(
                            true,
                            "Tax updated successfully",
                            _mapper.Map<TaxReadDTO>(newTax)
                        );
                    }
                    catch (Exception ex)
                    {
                        await _taxRepo.RollbackTransactionAsync(transaction);

                        return new GeneralResponse<TaxReadDTO>(
                            false,
                            "Error updating tax: " + ex.Message
                        );
                    }
                },

                verifySucceeded: null,
                cancellationToken: CancellationToken.None
            );
        }


    }
}