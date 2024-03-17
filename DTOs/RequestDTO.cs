using TheTicketShop.Utility;

namespace TheTicketShop.DTOs;

public class RequestDTO
{
    public StaticDetails.ApiType ApiType { get; set; } = StaticDetails.ApiType.GET;
    public string Url { get; set; }
    public object? Data {get; set; }
    public string AccessToken { get; set; }
}