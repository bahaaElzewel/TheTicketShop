using Microsoft.AspNetCore.Mvc;
using TheTicketShop.DTOs;
using TheTicketShop.IService;
using TheTicketShop.Services;
using TheTicketShop.Utility;

namespace TheTicketShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsShopController : ControllerBase
{
    private IBaseService _baseService;
    private readonly TicketService _ticketService;

    public TicketsShopController(IBaseService baseService, TicketService ticketService)
    {
        _baseService = baseService;
        _ticketService = ticketService;
    }


    [HttpGet("GetTicketsFromEvo")]
    public async Task<ResponseDTO> GetTicketsFromEvo ()
    {
        return await _baseService.SendAsync(new RequestDTO {
            ApiType = StaticDetails.ApiType.GET,
            Url = "http://localhost:5274/api/tickets/alltickets"
        });
    }

    [HttpGet("GetTicketsFromEvoGRPC")]
    public async Task<IActionResult> GetTicketsFromEvoGRPC ()
    {
        return Ok(await _ticketService.GetAllTickets());
    }

    [HttpGet("GetOneTicketIdFromEvo/{ticketId}")]
    public async Task<ResponseDTO> GetOneTicketIdFromEvo (int ticketId)
    {
        return await _baseService.SendAsync(new RequestDTO {
            ApiType = StaticDetails.ApiType.GET,
            Url = $"http://localhost:5274/api/tickets/FindTicketId/{ticketId}"
        });
    }

    [HttpPost("CreateATicketThroughEvo")]
    public async Task<ResponseDTO> CreateATicketThroughEvo ([FromBody] NewTicketDTO request)
    {
        return await _baseService.SendAsync(new RequestDTO 
        {
            ApiType = StaticDetails.ApiType.POST,
            Url = $"http://localhost:5274/api/tickets/createticket",
            Data = request
        });
    }
}