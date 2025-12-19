public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }      // Bruges kun til input, ikke til database
    public string Userpassword { get; set; }  // Bruges til/fra database
    public DateTime Timecreated { get; set; }
}