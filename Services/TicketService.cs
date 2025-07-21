using EvoTicketingGRPC;
using Grpc.Net.Client;

namespace TheTicketShop.Services;

public class TicketService
{
    private readonly TicketingService.TicketingServiceClient _client;
    public TicketService(TicketingService.TicketingServiceClient client)
    {
        _client = client;
    }
    public async Task<TicketResponse> GetAllTickets()
    {
        var result = await _client.TicketsAsync(new TicketRequest
        {
            TicketId = 1
        });

        return result;
    }
}