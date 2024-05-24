using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using KiBoards.Management.Service.Models;
using KiBoards.Management.Service.Services;

namespace KiBoards.Management.Service.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IHealthService _healthService;
        private readonly IMapper _mapper;

        public HealthController(ILogger<HealthController> logger, IHealthService healthService, IMapper mapper)
        {
            _logger = logger;
            _healthService = healthService;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetHealthInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<HealthInfoDto>> GetHealthInfoAsync()
        {
            _logger.LogDebug("Getting health info");
            var healthInfo = await _healthService.GetHealthInfoAsync();
            return Ok(_mapper.Map<HealthInfoDto>(healthInfo));
        }
    }
}