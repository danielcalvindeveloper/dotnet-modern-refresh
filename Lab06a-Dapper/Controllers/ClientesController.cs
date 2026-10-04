using Lab06a.Models;
using Lab06a.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab06a.Controllers;

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
