using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Lab08.Errors;

public sealed class ReservaExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ReservaExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetails;

    public ReservaExceptionHandler(
        ILogger<ReservaExceptionHandler> logger, IProblemDetailsService problemDetails)
    {
        _logger = logger;
        _problemDetails = problemDetails;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        // El middleware conserva aquí los valores originales de la ruta.
        var id = context.Features.Get<IExceptionHandlerFeature>()?.RouteValues?["id"];
        _logger.LogError(exception, "Error procesando reserva {ReservaId}", id);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails
            {
                Status = 500,
                Title = "No se pudo procesar la solicitud.",
                Detail = "Error simulado en el laboratorio. Consultar los logs del servidor.",
                Instance = context.Request.Path.ToString()
            }
        });
    }
}
