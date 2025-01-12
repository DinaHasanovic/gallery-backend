using AppBackEnd.Data;
using AppBackEnd.DTO;
using AppBackEnd.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService cityService;
        private readonly IMapper mapper;

        public CityController(IMapper mapper, ICityService cityService)
        {
            this.mapper = mapper;
            this.cityService = cityService;
        }
        //API za sve lokacije
        [HttpGet]
        public async Task<IActionResult> GetAllCities()
        {
            return Ok(mapper.Map<List<CityResponseDTO>>(await cityService.GetCities()));
        }
        //API za lokaciju sa ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityById([FromRoute] int id)
        {
            City? city = await cityService.GetCityById(id);
            if (city == null)
            {
                return Ok(new { Message = $"There are no cities with {id} identification" });
            }
            else
            {
                return Ok(mapper.Map<CityResponseDTO>(city));
            }
        }
        //API za lokaciju sa PPT-om
        [HttpGet("getByPTT/{PTT}")]
        public async Task<IActionResult> GetCityByPTT([FromRoute] string PTT)
        {
            City? city = await cityService.GetCityByPTT(PTT);
            if (city == null)
            {
                return NoContent();
            }
            else
            {
                return Ok(mapper.Map<CityResponseDTO>(city));
            }
        }
        //API za kreiranje lokacije
        [HttpPost]
        public async Task<IActionResult> CreateCity([FromForm] CreateCityRequestDTO city)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.Admin)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }

            City? check = await cityService.GetCityByPTT(city.PTT);
            if (check != null)
            {
                return BadRequest(new { Message = $"City with postal code {city.PTT} already exists" });
            }
            else
            {
                bool createdCity = await cityService.CreateCity(mapper.Map<City>(city));
                if (createdCity)
                {
                    return Ok(new { Message = "City created successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "There has been an error while creating city" });
                }
            }
        }
        //API za azuriranje lokacije
        [HttpPut]
        public async Task<IActionResult> UpdateCity([FromForm] UpdateCityRequestDTO city)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.Admin)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }

            City? check = await cityService.GetCityById(city.Id);
            if (check == null)
            {
                return BadRequest(new { Message = $"There is no city with {city.Id}" });
            }
            else
            {
                City? PttCheck = await cityService.GetCityByPTT(city.PTT);
                if (PttCheck != null && PttCheck.Id != city.Id)
                {
                    return BadRequest(new { Message = $"Postal code {city.PTT} is taken" });
                }
                else
                {
                    bool result = await cityService.UpdateCity(mapper.Map<City>(city));
                    if (result)
                    {
                        return Ok(new { Message = "City updated successfully." });
                    }
                    else
                    {
                        return BadRequest(new { Message = "There has been an error while updating city" });
                    }

                }
            }
        }
        //API za brisanje lokacije po ID
        [HttpDelete("deleteCityById/{id}")]
        public async Task<IActionResult> DeleteCityById([FromRoute] int id)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.Admin)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }

            City? city = await cityService.GetCityById(id);
            if (city == null)
            {
                return BadRequest(new { Message = $"City with {id} identification doesn't exist." });
            }
            else
            {
                bool result = await cityService.DeleteCityById(city.Id);
               if (result)
                return Ok(new { Message = "City deleted successfully." });
               else
                {
                    return BadRequest(new { Message = "There has been an error while deleting city" });
                }
            }
        }
        //API za brisanje lokacije po PTT
        [HttpDelete("deleteCityByPTT/{PTT}")]
        public async Task<IActionResult> DeleteCityByPTT([FromRoute] string PTT)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.Admin)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }

            City? city = await cityService.GetCityByPTT(PTT);
            if (city == null)
            {
                return BadRequest(new { Message = $"City with {PTT} postal code doesn't exist." });
            }
            else
            {
                bool result = await cityService.DeleteCityByPTT(city.PTT);
                if (result)
                    return Ok(new { Message = "City deleted successfully." });
                else
                {
                    return BadRequest(new { Message = "There has been an error while deleting city" });
                }
            }
        }
    }
    
}
