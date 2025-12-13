using Core.Interfaces.Services;
using invoice.Core.DTO.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers.ExternalAPI
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/external/[controller]")]
    public class ExternalProductController: ControllerBase
    {
        private readonly IProductService _productService;

        public ExternalProductController(IProductService productService)
        {
            _productService = productService;
        }

        private string GetExternalUserId() => HttpContext.Items["ExternalUserId"]?.ToString();


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateDTO dto)
        {
            var response = await _productService.CreateAsync(dto, GetExternalUserId());
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] ProductUpdateDTO dto)
        {
            var response = await _productService.UpdateAsync(id, dto, GetExternalUserId());
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _productService.DeleteAsync(id, GetExternalUserId());
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _productService.GetByIdWithInvoicesAsync(id, GetExternalUserId());
            return Ok(response);
        }

    }
}
