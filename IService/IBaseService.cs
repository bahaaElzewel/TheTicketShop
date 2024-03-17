using TheTicketShop.DTOs;

namespace TheTicketShop.IService;

public interface IBaseService
{
    Task<ResponseDTO>SendAsync(RequestDTO requestDTO);
}