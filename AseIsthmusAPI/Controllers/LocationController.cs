﻿using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _service;
        public LocationController(LocationService service)
        {
            _service = service;
        }

        [HttpGet("provinces")]
        public async Task<IEnumerable> GetAllProvinces()
        {
            return await _service.GetAllProvinces();
        }

        [HttpGet("province/{provinceId}/cantons")]
        public async Task<ActionResult<List<Canton>>> GetCantonsByProvince(int provinceId)
        {
            var cantons = await _service.GetCantonsByProvince(provinceId);
            return Ok(cantons);
        }

        [HttpGet("canton/{cantonId}/districts")]
        public async Task<ActionResult<List<District>>> GetDistrictsByCanton(int cantonId)
        {
            var districts = await _service.GetDistrictsByCanton(cantonId);
            return Ok(districts);
        }

        [HttpGet("district/{districtId}")]
        public async Task<ActionResult<District>> GetDistrictInformation([FromRoute] int districtId)
        {
            var districtInfo = await _service.GetDistrictInformation(districtId);
            return Ok(districtInfo);
        }
    }

}
