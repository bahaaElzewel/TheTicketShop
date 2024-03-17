using Microsoft.AspNetCore.Mvc;
using TheTicketShop.DTOs;
using TheTicketShop.IService;

namespace TheTicketShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsShopController : ControllerBase
{
    private IBaseService _baseService;

    public TicketsShopController(IBaseService baseService)
    {
        _baseService = baseService;
    }


    [HttpGet("GetTicketsFromEvo")]
    public async Task<ResponseDTO> GetTicketsFromEvo ()
    {
        return await _baseService.SendAsync(new RequestDTO {
            ApiType = Utility.StaticDetails.ApiType.GET,
            Url = "http://localhost:5274/api/tickets/alltickets"
        });
    }

    [HttpGet("GetOneTicketIdFromEvo/{ticketId}")]
    public async Task<ResponseDTO> GetOneTicketIdFromEvo (int ticketId)
    {
        return await _baseService.SendAsync(new RequestDTO {
            ApiType = Utility.StaticDetails.ApiType.GET,
            Url = $"http://localhost:5274/api/tickets/FindTicketId/{ticketId}"
        });
    }

    [HttpPost("CreateATicketThroughEvo")]
    public async Task<ResponseDTO> CreateATicketThroughEvo ([FromBody] NewTicketDTO request)
    {
        return await _baseService.SendAsync(new RequestDTO 
        {
            ApiType = Utility.StaticDetails.ApiType.POST,
            Url = $"http://localhost:5274/api/tickets/createticket",
            Data = request
        });
    }
}