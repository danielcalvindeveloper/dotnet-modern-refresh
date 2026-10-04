using Lab06b.Models;
using Lab06b.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab06b.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _service;

    public ClientesController(IClienteService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetAsync()
    {
        return Ok(await _service.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cliente>> GetPorIdAsync(int id)
    {
        var cliente = await _service.ObtenerPorIdAsync(id);
        if (cliente is null)
        {
            return NotFound();
        }

        return Ok(cliente);
    }
}
