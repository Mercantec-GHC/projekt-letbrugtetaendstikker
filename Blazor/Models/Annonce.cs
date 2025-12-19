public class Annonce
{
    public int Id { get; set; }
    public string Titel { get; set; }
    public string Beskrivelse { get; set; }
    public decimal Pris { get; set; }
    public DateTime Oprettet { get; set; }
    public int? UserId { get; set; }  // BrugerId kan være null hvis gamle annoncer mangler ejer
    public User User { get; set; }  // (valgfrit, kun til visning, ikke gemt automatisk)
    public byte[] Image { get; set; }
}