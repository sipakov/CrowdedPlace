using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.Libraries.Domain.Models;
using OnlineDemonstrator.MobileApi.Interfaces;

namespace OnlineDemonstrator.MobileApi.Controllers
{
    [Route("[controller]")]
    public class ObjectionableReasonController : ControllerBase
    {
        private readonly IObjectionableReasonService _objectionableReasonService;

        public ObjectionableReasonController(IObjectionableReasonService objectionableReasonService)
        {
            _objectionableReasonService = objectionableReasonService ?? throw new ArgumentNullException(nameof(objectionableReasonService));
        }

        [HttpPost("add")]
        public async Task<ActionResult<BaseResult>> AddAsync([FromBody, Required] ObjectionableContent objectionableContent)
        {
            if (!ModelState.IsValid) return BadRequest();

            return await _objectionableReasonService.AddAsync(objectionableContent);
        }
    }
}