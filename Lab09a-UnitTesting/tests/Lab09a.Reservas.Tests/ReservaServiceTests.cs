using Lab09a.Services;
using Xunit;

namespace Lab09a.Reservas.Tests;

public sealed class ReservaServiceTests
{
    [Fact]
    public void PuedeReservar_CuandoHayCupo_DevuelveTrue()
    {
        // Arrange
        var service = new ReservaService();
        int lugaresDisponibles = 2;
        int cantidadSolicitada = 1;

        // Act
        bool resultado = service.PuedeReservar(lugaresDisponibles, cantidadSolicitada);

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void PuedeReservar_CuandoNoHayCupo_DevuelveFalse()
    {
        // Arrange
        var service = new ReservaService();

        // Act
        bool resultado = service.PuedeReservar(lugaresDisponibles: 0, cantidadSolicitada: 1);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void PuedeReservar_CuandoSolicitaTodosLosLugares_DevuelveTrue()
    {
        // Arrange
        var service = new ReservaService();

        // Act
        bool resultado = service.PuedeReservar(lugaresDisponibles: 2, cantidadSolicitada: 2);

        // Assert
        Assert.True(resultado);
    }

    [Theory]
    [InlineData(3, 1, true)]
    [InlineData(3, 3, true)]
    [InlineData(3, 4, false)]
    [InlineData(3, 0, false)]
    [InlineData(3, -1, false)]
    [InlineData(0, 1, false)]
    [InlineData(-1, 1, false)]
    public void PuedeReservar_SegunDisponibilidadYCantidad_DevuelveResultadoEsperado(
        int lugaresDisponibles, int cantidadSolicitada, bool esperado)
    {
        // Arrange
        var service = new ReservaService();

        // Act
        bool resultado = service.PuedeReservar(lugaresDisponibles, cantidadSolicitada);

        // Assert
        Assert.Equal(esperado, resultado);
    }
}
