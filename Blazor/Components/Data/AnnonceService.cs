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
            "INSERT INTO Annoncer (Titel, Beskrivelse, Pris, Image, UserId) VALUES (@t, @b, @p, @Image, @userid)", conn);
        cmd.Parameters.AddWithValue("t", annonce.Titel);
        cmd.Parameters.AddWithValue("b", annonce.Beskrivelse ?? "");
        cmd.Parameters.AddWithValue("p", annonce.Pris); 
        cmd.Parameters.AddWithValue("Image", (object?)annonce.Image ?? DBNull.Value);
        cmd.Parameters.AddWithValue("userid", annonce.UserId ?? (object)DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<Annonce>> HentAlleAnnoncer()
    {
        var annoncer = new List<Annonce>();

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        //using var cmd = new NpgsqlCommand("SELECT a.Id, b.Id FROM Annoncer a LEFT JOIN UserProfiles b ON a.UserId = b.Id ORDER BY a.Oprettet DESC", conn);
        //using var cmd = new NpgsqlCommand("SELECT Id, Titel, Beskrivelse, Pris, Oprettet, Image FROM Annoncer a ORDER BY Oprettet DESC", conn);
        using var cmd = new NpgsqlCommand(
    "SELECT a.Id, a.Titel, a.Beskrivelse, a.Pris, a.Oprettet, a.Image, a.UserId, b.Username " +
    "FROM Annoncer a LEFT JOIN Userprofiles b ON a.UserId = b.Id " +
    "ORDER BY a.Oprettet DESC", conn); 
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            annoncer.Add(new Annonce
            {
                Id = reader.GetInt32(0),
                Titel = reader.GetString(1),
                Beskrivelse = reader.GetString(2),
                Pris = reader.GetDecimal(3),
                Oprettet = reader.GetDateTime(4),
                Image = !reader.IsDBNull(5) ? (byte[])reader["Image"] : null,
                UserId = !reader.IsDBNull(6) ? reader.GetInt32(6) : (int?)null,
                User = !reader.IsDBNull(7) ? new User { Username = reader.GetString(7) } : null
            });
        }

        return annoncer;
    }
    public async Task<Annonce> HentAnnonceVedId(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(
     "SELECT a.Id, a.Titel, a.Beskrivelse, a.Pris, a.Oprettet, a.Image, a.UserId, b.Username " +
     "FROM Annoncer a LEFT JOIN Userprofiles b ON a.UserId = b.Id " +
     "ORDER BY a.Oprettet DESC", conn); cmd.Parameters.AddWithValue("id", id);
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Annonce
            {
                Id = reader.GetInt32(0),
                Titel = reader.GetString(1),
                Beskrivelse = reader.GetString(2),
                Pris = reader.GetDecimal(3),
                Oprettet = reader.GetDateTime(4),
                Image = !reader.IsDBNull(5) ? (byte[])reader["Image"] : null,
                UserId = !reader.IsDBNull(6) ? reader.GetInt32(6) : (int?)null,
                User = !reader.IsDBNull(7) ? new User { Username = reader.GetString(7) } : null
            };
        }
        return null;
    }
}

