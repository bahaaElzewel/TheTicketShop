using EvoTicketingGRPC;
using Grpc.Core;
using Grpc.Net.Client;

namespace TheTicketShop.Services;

public class TicketService
{
    private readonly TicketingService.TicketingServiceClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TicketService(TicketingService.TicketingServiceClient client, IHttpContextAccessor httpContextAccessor)
    {
        _client = client;
        _httpContextAccessor = httpContextAccessor;

    }
    public async Task<TicketResponse> GetAllTickets()
    {
        var jwtToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
        var headers = new Metadata
        {
            { "Authorization", $"Bearer {jwtToken}" }
        };
        var result = await _client.TicketsAsync(new TicketRequest
        {
            TicketId = 1
        });

        return result;
    }
}