using GoogleMapInfo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace microservice_map_info.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapInfoController : ControllerBase
    {
        private readonly ILogger<MapInfoController> _logger;
        private readonly GoogleDistanceApi _googleDistanceApi;

        public MapInfoController(ILogger<MapInfoController> logger,GoogleDistanceApi googleDistanceApi)
        {
            _logger = logger ?? throw new
                ArgumentException(nameof(logger));               
            _googleDistanceApi = googleDistanceApi;
        }
        [HttpGet]
        public async Task<ActionResult<GoogleDistanceData>> GetDistance(string originCity,string destinationCity)
        {
            try
            {
                return await _googleDistanceApi.GetMapDistance(originCity, destinationCity);
            }
            catch   (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error getting map distance:${originCity} to" +
                    $"{destinationCity}," +
                    $"status code:{ex.StatusCode}");
                return StatusCode(500);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex,$"Error geting address detail from Google:{originCity} to {destinationCity}");
                return StatusCode(500);
            }
        }
    }
}
