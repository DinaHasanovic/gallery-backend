using AppBackEnd.Data;
using AppBackEnd.DTO;
using AppBackEnd.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace AppBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HallsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IHallService hallService;
        private readonly ICityService cityService;

        public HallsController(IMapper mapper, IHallService service, ICityService cityService)
        {
            this.mapper = mapper;
            this.hallService = service;
            this.cityService = cityService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllHalls()
        {

            return Ok(mapper.Map<List<HallResponseDTO>>(await hallService.GetAllHalls()));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHallById([FromRoute] int id)
        {
            return Ok(mapper.Map<HallResponseDTO>(await hallService.GetHallById(id)));
        }
        [HttpPost]
        public async Task<IActionResult> CreateHall([FromForm] CreateHallRequestDTO hall)
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

            try
            {
                bool result = await hallService.CreateHall(mapper.Map<Hall>(hall));
                if (result)
                {
                    return Ok(new { Message = "Hall created successfully" });
                }
                else
                {
                    return BadRequest(new { Message = "There was an error while creating hall" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCity([FromForm] UpdateHallRequestDTO request)
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

            Hall? hallToUpdate = await hallService.GetHallById(request.Id);
            if (hallToUpdate == null)
            {
                return BadRequest(new { Message = $"Hall with id = {request.Id} doesn't exist." });
            }
            else
            {
                try
                {
                    Hall? h = mapper.Map<Hall>(request);
                    if(h.CityId.HasValue)
                    {
                        City? city = await cityService.GetCityById((int)h.CityId);
                        if(city == null)
                        {
                            return BadRequest(new { Message = $"City with id = {h.CityId} doesn't exist" });
                        }
                    }
                    bool result = await hallService.UpdateHall(h);
                    if (result)
                    {
                        return Ok(new { Message = "Hall update successfully" });
                    }
                    else
                    {
                        return BadRequest(new { Message = "There was an error while updating hall." });
                    }
                }
                catch (Exception ex) { 
                    return BadRequest(new { Message = ex.Message});
                    }
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHall([FromRoute] int id)
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

            try { 
                Hall? h = await hallService.GetHallById(id);
                if (h == null)
                {
                    return BadRequest(new { Message = $"Hall with id = {id} doesn't exist." });
                }
                else
                {
                    bool result = await hallService.DeleteHallById(id);
                    if (result)
                    {
                        return Ok(new { Message = "Hall deleted successfully" });
                    }
                    else
                    {
                        return BadRequest(new { Message = "There was an error while deleting hall." });

                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("hallsByCity/{cityId}")]
        public async Task<IActionResult> GetHallsByCity([FromRoute] int cityId)
        {
            City? city = await cityService.GetCityById(cityId);
            if(city == null)
            {
                return BadRequest(new { Message = $"City with ID={cityId} doesn't exist" });
            }
            return Ok(mapper.Map<List<HallResponseDTO>>(await hallService.GetHallsByCityId(cityId)));

        }
        [HttpGet("hallsByNameSearch")]
        public async Task<IActionResult> GetHallsByCity([FromQuery] string cityName)
        {
            return Ok(mapper.Map<List<HallResponseDTO>>(await hallService.GetHallByNameSearch(cityName)));

        }
    }
}
