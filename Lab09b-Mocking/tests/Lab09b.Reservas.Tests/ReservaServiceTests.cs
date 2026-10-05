using Lab09b.Repositories;
using Lab09b.Services;
using Moq;
using Xunit;

namespace Lab09b.Reservas.Tests;

public sealed class ReservaServiceTests
{
    [Fact]
    public async Task ReservarAsync_CuandoHayCupo_ConfirmaReserva()
    {
        // Arrange: el doble cumple el papel de stub; controlamos sus respuestas.
        const int turnoId = 1;
        const int cantidadSolicitada = 1;
        var repository = new Mock<IReservaRepository>();
        repository.Setup(r => r.ObtenerCuposDisponiblesAsync(turnoId)).ReturnsAsync(2);
        repository.Setup(r => r.GuardarReservaAsync(turnoId, cantidadSolicitada))
            .Returns(Task.CompletedTask);
        var service = new ReservaService(repository.Object);

        // Act
        bool confirmado = await service.ReservarAsync(turnoId, cantidadSolicitada);

        // Assert: interesa el resultado; no necesitamos Verify.
        Assert.True(confirmado);
    }

    [Fact]
    public async Task ReservarAsync_CuandoHayCupo_GuardaUnaVez()
    {
        // Arrange
        const int turnoId = 1;
        const int cantidadSolicitada = 1;
        var repository = new Mock<IReservaRepository>();
        repository.Setup(r => r.ObtenerCuposDisponiblesAsync(turnoId)).ReturnsAsync(2);
        repository.Setup(r => r.GuardarReservaAsync(turnoId, cantidadSolicitada))
            .Returns(Task.CompletedTask);
        var service = new ReservaService(repository.Object);

        // Act
        await service.ReservarAsync(turnoId, cantidadSolicitada);

        // Assert: guardar con estos argumentos forma parte del comportamiento.
        repository.Verify(r => r.GuardarReservaAsync(turnoId, cantidadSolicitada), Times.Once());
    }

    [Fact]
    public async Task ReservarAsync_CuandoNoHayCupo_RechazaReserva()
    {
        // Arrange
        const int turnoId = 1;
        var repository = new Mock<IReservaRepository>();
        repository.Setup(r => r.ObtenerCuposDisponiblesAsync(turnoId)).ReturnsAsync(0);
        var service = new ReservaService(repository.Object);

        // Act
        bool confirmado = await service.ReservarAsync(turnoId, cantidadSolicitada: 1);

        // Assert
        Assert.False(confirmado);
    }

    [Fact]
    public async Task ReservarAsync_CuandoNoHayCupo_NoGuardaReserva()
    {
        // Arrange
        const int turnoId = 1;
        var repository = new Mock<IReservaRepository>();
        repository.Setup(r => r.ObtenerCuposDisponiblesAsync(turnoId)).ReturnsAsync(0);
        var service = new ReservaService(repository.Object);

        // Act
        await service.ReservarAsync(turnoId, cantidadSolicitada: 1);

        // Assert: ninguna reserva debe guardarse, independientemente de sus argumentos.
        repository.Verify(
            r => r.GuardarReservaAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(3)]
    public async Task ReservarAsync_CuandoCantidadEsInvalidaParaDosCupos_RechazaReserva(
        int cantidadSolicitada)
    {
        // Arrange
        const int turnoId = 1;
        var repository = new Mock<IReservaRepository>();
        repository.Setup(r => r.ObtenerCuposDisponiblesAsync(turnoId)).ReturnsAsync(2);
        var service = new ReservaService(repository.Object);

        // Act
        bool confirmado = await service.ReservarAsync(turnoId, cantidadSolicitada);

        // Assert
        Assert.False(confirmado);
    }
}
