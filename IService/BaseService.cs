using System.Net;
using System.Text;
using Newtonsoft.Json;
using Serilog;
using TheTicketShop.DTOs;
using TheTicketShop.Utility;

namespace TheTicketShop.IService;

public class BaseService : IBaseService
{
    private readonly IHttpClientFactory _httpClient;

    public BaseService(IHttpClientFactory httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<ResponseDTO> SendAsync(RequestDTO requestDTO)
    {
        try
        {
            HttpClient client = _httpClient.CreateClient("EvoTicketing");
            
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri(requestDTO.Url);
            
            if (requestDTO.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
            }

            switch(requestDTO.ApiType)
            {
                case StaticDetails.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case StaticDetails.ApiType.PUT:
                message.Method = HttpMethod.Put;
                    break;
                case StaticDetails.ApiType.DELETE:
                message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            HttpResponseMessage apiResponse = null;
            apiResponse = await client.SendAsync(message);
            Log.Information("this is the http request details => {@apiResponse}", apiResponse);

            switch(apiResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return new ResponseDTO {IsSuccess = false, Message = "Not found in the api"};
                case HttpStatusCode.Forbidden:
                    return new ResponseDTO {IsSuccess = false, Message = "Forbidden in the api"};
                case HttpStatusCode.Unauthorized:
                    return new ResponseDTO {IsSuccess = false, Message = "Unauthorized in the api"};
                case HttpStatusCode.InternalServerError:
                    return new ResponseDTO {IsSuccess = false, Message = "Internal server error in the api"};
                default:
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    try
                    {
                        var apiResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(apiContent);
                        Log.Information("this is the http apiResult => {@apiResult}", apiResult);
                        var apiResponseDTO = new ResponseDTO
                        {
                            Result = apiResult,
                            IsSuccess = true,
                        };
                        return apiResponseDTO;
                    }
                    catch(Exception ex)
                    {
                        var apiResult = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(apiContent);
                        Log.Information("this is the http apiResult => {@apiResult}", apiResult);
                        var apiResponseDTO = new ResponseDTO
                        {
                            Result = apiResult,
                            IsSuccess = true,
                        };
                        return apiResponseDTO;
                    }
            }
        }
        catch (Exception ex)
        {
            var DTO = new ResponseDTO 
            {
                IsSuccess = false,
                Message = ex.InnerException?.Message ?? ex.Message
            };
            Log.Information("this is the exception => {@DTO}", DTO);
            return DTO;
        }
    }
}