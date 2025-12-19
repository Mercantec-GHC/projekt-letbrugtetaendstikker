using Npgsql;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

public class UserService
{
    private readonly string _connectionString;
    public UserService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    // Hash adgangskode (basic)
    private string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    // Opret bruger
    public async Task<bool> NewAccount(User user)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        using var cmd = new NpgsqlCommand(
            "INSERT INTO Users (Username, PasswordHash) VALUES (@unavn, @hash)", conn);
        cmd.Parameters.AddWithValue("unavn", user.Username);
        cmd.Parameters.AddWithValue("hash", HashPassword(user.Password));
        try
        {
            await cmd.ExecuteNonQueryAsync();
            return true;
        }
        catch
        {
            return false; // Fx brugernavn allerede brugt
        }
    }

    // Log ind
    public async Task<User> LogIn(string username, string password)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        using var cmd = new NpgsqlCommand(
            "SELECT UserID, Username, PasswordHash, CreationTime FROM Users WHERE Username=@unavn", conn);
        cmd.Parameters.AddWithValue("unavn", username);
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var hash = reader.GetString(2);
            if (hash == HashPassword(password))
            {
                return new User
                {
                    UserID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = hash,
                    CreationTime = reader.GetDateTime(3)
                };
            }
        }
        return null;
    }
}