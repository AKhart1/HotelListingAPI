﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.CoreData;
using HotelListing.API.CoreModels.Country;
using AutoMapper;
using HotelListing.API.CoreContracts;
using Microsoft.AspNetCore.Authorization;
using HotelListing.API.CoreExceptions;
using HotelListing.API.CoreModels;
using Newtonsoft.Json.Converters;

namespace HotelListing.API.CoreControllers
{
    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]

    public class CountriesController : ControllerBase
    {       
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(IMapper _mapper, ICountriesRepository countriesRepository, ILogger<CountriesController> logger)
        {           
            this._mapper = _mapper;
            this._countriesRepository = countriesRepository;
            this._logger = logger;
        }

        // GET: api/Countries/GetAll
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            //"Ok" mean that API response can contain about 200 

            var countries = await _countriesRepository.GetAllAsync<CountryDto>();            
            return Ok(countries);
        }

        //GET: api/Countries/?StartIndex=0&pageSize=25&PageNumber=1
        [HttpGet]
        public async Task<ActionResult<PageResult<GetCountryDto>>> GetPagedCountries(
            [FromQuery] QueryParameters queryParameters)
        {
            var pagedCountriesResult  = await _countriesRepository.GetAllAsync<GetCountryDto>(queryParameters);
            return Ok(pagedCountriesResult);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {            
            var country = await _countriesRepository.GetDetails(id);
            return Ok(country);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id)
            {
                return BadRequest("Invalid Record Id");
            }
            try
            {
                await _countriesRepository.UpdateAsync(id,updateCountryDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
        {
            var country = await _countriesRepository.AddAsync<CreateCountryDto, GetCountryDto>(createCountryDto);
            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles ="Administartor")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            await _countriesRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            return await _countriesRepository.Exists(id);
        }
    }
}
