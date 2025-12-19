public class Listing
{
    public int ListingID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreationTime { get; set; }
    public byte[] Image { get; set; }
    public int? OwnerID { get; set; }
    public int ListViews { get; set; }
    public User User { get; set; }  // (valgfrit, kun til visning, ikke gemt automatisk)
}