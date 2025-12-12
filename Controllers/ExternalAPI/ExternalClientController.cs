using invoice.Core.DTO.Client;
using invoice.Core.Interfaces.Services;
using invoice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using invoice.Core.DTO.Client;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace invoice.Controllers.ExternalAPI
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/external/[controller]")]
    public class ExternalClientController:ControllerBase
    {
        private readonly IClientService _clientService;

        public ExternalClientController(IClientService clientService)
        {
            _clientService = clientService;
        }
        private string GetExternalUserId() => HttpContext.Items["ExternalUserId"]?.ToString();


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClientCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _clientService.CreateAsync(dto, GetExternalUserId());
            if (!response.Success) return BadRequest(response);
            return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response);
        }
    }
}
