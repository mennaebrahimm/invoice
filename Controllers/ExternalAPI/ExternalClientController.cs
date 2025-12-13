using invoice.Core.DTO.Client;
using invoice.Core.Interfaces.Services;
using invoice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _clientService.GetByIdAsync(id, GetExternalUserId());
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] ClientUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var response = await _clientService.UpdateAsync(id, dto, GetExternalUserId());
            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _clientService.DeleteAsync(id, GetExternalUserId());
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
    }
}
