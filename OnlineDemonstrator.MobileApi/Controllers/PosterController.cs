using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.MobileApi.Interfaces;

namespace OnlineDemonstrator.MobileApi.Controllers
{
    [Route("[controller]")]
    public class PosterController : ControllerBase
    {
        private readonly IPosterService _posterService;
        public PosterController(IPosterService posterService)
        {
            _posterService = posterService ?? throw new ArgumentNullException(nameof(posterService));
        }
        
        [HttpPost("add")]
        public async Task<ActionResult<Poster>> AddAsync([FromBody, BindRequired] PosterIn posterIn)
        {
            if (!ModelState.IsValid) return BadRequest();
            
            var currentDateTime = DateTime.UtcNow; 

            return await _posterService.AddPosterAsync(posterIn, currentDateTime);
        }
        
        [HttpGet("getFromActualDemonstrations")]
        public async Task<ActionResult<List<PosterOut>>> GetFromActualDemonstrationsAsync([FromQuery, Required] int postersCountInDemonstration)
        {
            if (!ModelState.IsValid) return BadRequest();

            return await _posterService.GetFromActualDemonstrations(postersCountInDemonstration);
        }
        
        [HttpGet("getPostersByDemonstrationId")]    
        public async Task<ActionResult<List<PosterOut>>> GetPostersByDemonstrationIdAsync([FromQuery, Required] int demonstrationId)
        {
            if (!ModelState.IsValid) return BadRequest();
            
            return await _posterService.GetPostersByDemonstrationId(demonstrationId);
        }
        
        [HttpPost("getPosterById")]    
        public async Task<ActionResult<PosterOut>> GetPostersByIdAsync([FromBody, Required] PosterOut posterOut)
        {
            if (!ModelState.IsValid) return BadRequest();

            return await _posterService.GetPosterById(posterOut.DeviceId, posterOut.CreatedDate, posterOut.DemonstrationId);
        }
        
        [HttpPost("addToExistDemonstration")]
        public async Task<ActionResult<Poster>> AddToExistDemonstrationAsync([FromBody] PosterIn posterIn)
        {
            if (!ModelState.IsValid) return BadRequest();
            
            var currentDateTime = DateTime.UtcNow; 

            return await _posterService.AddPosterToExistDemonstrationAsync(posterIn, currentDateTime);
        }
        
        [HttpPost("addToExpiredDemonstration")]
        public async Task<ActionResult<Poster>> AddToExpiredDemonstrationAsync([FromBody] PosterIn posterIn)
        {
            if (!ModelState.IsValid) return BadRequest();
            
            var currentDateTime = DateTime.UtcNow; 

            return await _posterService.AddPosterToExpiredDemonstrationAsync(posterIn, currentDateTime);
        }
        
        
        
        [HttpGet("test")]
        public string GetStringTest()
        {
            return "123!Success";
        }
    }
}