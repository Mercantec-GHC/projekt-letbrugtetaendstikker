using Npgsql;
using Microsoft.Extensions.Configuration;

public class AnnonceService
{
    private readonly string _connectionString;

    public AnnonceService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public async Task TilføjAnnonce(Annonce annonce)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(
            "INSERT INTO Annoncer (Titel, Beskrivelse, Pris) VALUES (@t, @b, @p)", conn);
        cmd.Parameters.AddWithValue("t", annonce.Titel);
        cmd.Parameters.AddWithValue("b", annonce.Beskrivelse ?? "");
        cmd.Parameters.AddWithValue("p", annonce.Pris);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<Annonce>> HentAlleAnnoncer()
    {
        var annoncer = new List<Annonce>();

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT Id, Titel, Beskrivelse, Pris, Oprettet FROM Annoncer ORDER BY Oprettet DESC", conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            annoncer.Add(new Annonce
            {
                Id = reader.GetInt32(0),
                Titel = reader.GetString(1),
                Beskrivelse = reader.GetString(2),
                Pris = reader.GetDecimal(3),
                Oprettet = reader.GetDateTime(4)
            });
        }

        return annoncer;
    }
    public async Task<Annonce> HentAnnonceVedId(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT Id, Titel, Beskrivelse, Pris, Oprettet FROM Annoncer WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Annonce
            {
                Id = reader.GetInt32(0),
                Titel = reader.GetString(1),
                Beskrivelse = reader.GetString(2),
                Pris = reader.GetDecimal(3),
                Oprettet = reader.GetDateTime(4)
            };
        }
        return null;
    }
}

