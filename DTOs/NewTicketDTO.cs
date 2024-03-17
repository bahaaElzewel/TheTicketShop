namespace TheTicketShop.DTOs;

public class NewTicketDTO
{
    public string TicketCode { get; set; }
    public string IssuerName { get; set; } 
    public string Occasion { get; set; }
    public string Benefeciary { get; set; }
    public int Price { get; set; }
}