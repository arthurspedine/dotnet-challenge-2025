using Microsoft.AspNetCore.Mvc;
using Motoflow.Models;
using Motoflow.Services;

namespace Motoflow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {

        private readonly AreaService _areaService;
        
        public AreaController(AreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Area>>> Get()
        {
            return Ok(await _areaService.GetAllAreasAsync());
        }
    }
}
