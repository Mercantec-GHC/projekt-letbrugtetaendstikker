using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;

public class ListingService
{
    private readonly string _connectionString;

    public ListingService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public async Task AddListing(Listing listing)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(
            "INSERT INTO Listings (Title, Description, Price, Image, OwnerID) VALUES (@t, @b, @p, @img, @ownerid)", conn);
        cmd.Parameters.AddWithValue("t", listing.Title);
        cmd.Parameters.AddWithValue("b", listing.Description ?? "");
        cmd.Parameters.AddWithValue("p", listing.Price);
        cmd.Parameters.AddWithValue("img", (object?)listing.Image ?? DBNull.Value);
        cmd.Parameters.AddWithValue("ownerid", (object?)listing.OwnerID ?? (object)DBNull.Value); //object? why?
        await cmd.ExecuteNonQueryAsync();
    }

    

    public async Task<List<Listing>> GetAllListings()
    {
        var listings = new List<Listing>();

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(
            "SELECT a.ListingID, a.Title, a.Description, a.Price, a.CreationTime, a.Image, a.OwnerID, b.Username " +
            "FROM Listings a LEFT JOIN Users b ON a.OwnerID = b.UserID " +
            "ORDER BY a.CreationTime DESC", conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            listings.Add(new Listing
            {
                ListingID = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.GetString(2),
                Price = reader.GetDecimal(3),
                CreationTime = reader.GetDateTime(4),
                Image = !reader.IsDBNull(5) ? (byte[])reader["Image"] : null,
                OwnerID = !reader.IsDBNull(6) ? reader.GetInt32(6) : (int?)null,
                User = !reader.IsDBNull(7) ? new User {Username = reader.GetString(7)} : null
            });
        }

        return listings;
    }

    public async Task<Listing> GetListingByID(int ID)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(
            "SELECT a.ListingID, a.Title, a.Description, a.Price, a.CreationTime, a.Image, a.OwnerID, b.Username " + 
            "FROM Listings a LEFT JOIN Users b ON a.OwnerID = b.UserID " +
            "WHERE a.ListingID = @id ORDER BY a.CreationTime DESC", conn);
        cmd.Parameters.AddWithValue("id", ID);
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Listing
            {
                ListingID = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.GetString(2),
                Price = reader.GetDecimal(3),
                CreationTime = reader.GetDateTime(4),
                Image = !reader.IsDBNull(5) ? (byte[])reader["Image"] : null,
                OwnerID = !reader.IsDBNull(6) ? reader.GetInt32(6) : null,
                User = !reader.IsDBNull(7) ? new User {Username = reader.GetString(7)} : null
            };
        }
        else return null;
        }

    // REDIGER
    public async Task EditListing(Listing listing)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(
            "UPDATE Listings SET Title=@t, description=@b, Price=@p, Image=@img WHERE ListingID=@id", conn);
        cmd.Parameters.AddWithValue("t", listing.Title);
        cmd.Parameters.AddWithValue("b", listing.Description ?? "");
        cmd.Parameters.AddWithValue("p", listing.Price);
        cmd.Parameters.AddWithValue("img", (object?)listing.Image ?? DBNull.Value);
        cmd.Parameters.AddWithValue("id", listing.ListingID);
        await cmd.ExecuteNonQueryAsync();
    }

    // SLET
    public async Task DeleteListing(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("DELETE FROM Listings WHERE ListingID=@id", conn);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}