using System.Net;
using System.Net.Http.Json;
using Lab09c.Contracts;
using Lab09c.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Lab09c.Tests;

public sealed class ReservasIntegrationTests
{
    [Fact]
    public async Task GetReserva_CuandoExiste_Devuelve200YJson()
    {
        // Arrange: un host nuevo para este test, sin compartir estado mutable.
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        using var response = await client.GetAsync("/api/reservas/1", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        var reserva = await response.Content.ReadFromJsonAsync<Reserva>(cancellationToken);
        Assert.NotNull(reserva);
        Assert.Equal(1, reserva.Id);
        Assert.Equal("Pendiente", reserva.Estado);
        Assert.Equal("middleware", response.Headers.GetValues("X-Lab09c").Single());
    }

    [Fact]
    public async Task GetReserva_CuandoNoExiste_Devuelve404()
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act: en este host sólo existe la reserva inicial 1.
        using var response = await client.GetAsync("/api/reservas/2", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostReserva_CuandoFaltaEstado_Devuelve400YNoCreaReserva()
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;
        var request = new CrearReservaRequest
        {
            ClienteId = 1,
            Fecha = new DateTime(2026, 10, 7, 11, 0, 0)
            // Estado queda null: viola [Required].
        };

        // Act
        using var response = await client.PostAsJsonAsync("/api/reservas", request, cancellationToken);

        // Assert: validación MVC real y respuesta estándar, antes de la Action.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken);
        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.Contains(nameof(CrearReservaRequest.Estado), problem.Errors.Keys);
        using var consulta = await client.GetAsync("/api/reservas/2", cancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, consulta.StatusCode);
    }

    [Fact]
    public async Task PostReserva_CuandoEsValida_Devuelve201YPermiteConsultarla()
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;
        var request = new CrearReservaRequest
        {
            ClienteId = 7,
            Fecha = new DateTime(2026, 10, 7, 11, 0, 0),
            Estado = "Confirmada"
        };

        // Act
        using var response = await client.PostAsJsonAsync("/api/reservas", request, cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var creada = await response.Content.ReadFromJsonAsync<Reserva>(cancellationToken);
        Assert.NotNull(creada);
        Assert.Equal(2, creada.Id);
        Assert.Equal(request.ClienteId, creada.ClienteId);
        Assert.Equal(request.Fecha, creada.Fecha);
        Assert.Equal(request.Estado, creada.Estado);
        var location = response.Headers.Location;
        Assert.NotNull(location);
        Assert.Equal("/api/reservas/2", location.AbsolutePath);

        // Otra request al mismo host: comprueba DI, almacenamiento y Location.
        using var consulta = await client.GetAsync(location, cancellationToken);
        Assert.Equal(HttpStatusCode.OK, consulta.StatusCode);
        var recuperada = await consulta.Content.ReadFromJsonAsync<Reserva>(cancellationToken);
        Assert.NotNull(recuperada);
        Assert.Equal(creada.Id, recuperada.Id);
        Assert.Equal(creada.ClienteId, recuperada.ClienteId);
        Assert.Equal(creada.Fecha, recuperada.Fecha);
        Assert.Equal(creada.Estado, recuperada.Estado);
    }
}
