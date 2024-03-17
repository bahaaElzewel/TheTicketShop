namespace TheTicketShop.Utility;

public class StaticDetails
{
    public static string EvoTicketingUrlBase {get; set; }
    public enum ApiType
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}