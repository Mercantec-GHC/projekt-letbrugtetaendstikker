public class User
{
	public int UserID { get; set; }
	public string Username { get; set; }
	public string Password { get; set; }      // Bruges kun til input, ikke til database
	public string PasswordHash { get; set; }  // Bruges til/fra database
	public DateTime CreationTime { get; set; }
}