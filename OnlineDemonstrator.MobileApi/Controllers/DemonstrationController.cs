using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.MobileApi.Interfaces;


namespace OnlineDemonstrator.MobileApi.Controllers
{
    [Route("[controller]")]
    public class DemonstrationController : ControllerBase
    {
        private readonly IDemonstrationService _demonstrationService;

        public DemonstrationController(IDemonstrationService demonstrationService)
        {
            _demonstrationService = demonstrationService ?? throw new ArgumentNullException(nameof(demonstrationService));
        }  
        
        [HttpGet("getActualDemonstrations")]
        public async Task<ActionResult<List<DemonstrationOut>>> GetActualDemonstrationsAsync()
        {
            if (!ModelState.IsValid) return BadRequest();
            
            return (await _demonstrationService.GetActualDemonstrations()).ToList();
        }
        
        [HttpPost("getNearestDemonstration")]
        public async Task<ActionResult<DemonstrationOut>> GetNearestDemonstrationAsync([FromBody, BindRequired] PointsIn pointsIn)
        {
            if (!ModelState.IsValid) return BadRequest();

            return await _demonstrationService.GetNearestDemonstration(pointsIn);
        }
    }
}