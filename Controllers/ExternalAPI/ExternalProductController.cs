using Core.Interfaces.Services;
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

    }
}
