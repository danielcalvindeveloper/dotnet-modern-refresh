using Dapper;
using Lab06a.Models;
using Microsoft.Data.Sqlite;

namespace Lab06a.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly string _connectionString;

    public ClienteRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Cliente>> ObtenerTodosAsync()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();
            return await connection.QueryAsync<Cliente>(
                "SELECT Id, Nombre, Email FROM Clientes ORDER BY Id;");
        } // Dispose cierra la conexión después de completar y materializar la consulta.
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();
            return await connection.QuerySingleOrDefaultAsync<Cliente>(
                "SELECT Id, Nombre, Email FROM Clientes WHERE Id = @Id;",
                new { Id = id });
        } // Dispose cierra la conexión después del await, incluso al salir por una excepción.
    }
}
