using AppBackEnd.Data;
using AppBackEnd.DTO;
using AppBackEnd.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;

namespace AppBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IThemeService themeService;

        public ThemeController(IMapper mapper, IThemeService themeService)
        {
            this.themeService = themeService;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
           
            return Ok(mapper.Map<List<ThemeResponseDTO>>(await themeService.GetAllThemes()));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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
            Theme? theme = await themeService.GetThemeById(id);
            if (theme != null)
            {
                return Ok(mapper.Map<ThemeResponseDTO>(theme));
            }
            else
            {
                return BadRequest(new { Message = $"Theme with ID={id} doesn't exist." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromForm] string themeName)
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

            if (string.IsNullOrEmpty(themeName.Trim()))
            {
                return BadRequest(new { Message = "Theme must have name" });
            }
            {
                bool result = await themeService.CreateTheme(themeName);
                if (result)
                {
                    return Ok(new { Message = "Theme created successfully" });
                }
                else
                {
                    return BadRequest(new { Message = "There was an error while creating new theme." });

                }
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThemeById([FromRoute] int id)
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
            Theme? theme = await themeService.GetThemeById(id);
            if(theme == null)
            {
                return BadRequest(new { Message = $"There is no theme with ID={id}." });

            }
            else
            {
                bool result = await themeService.DeleteThemeById(id);
                if (result)
                {
                    return Ok(new { Message = "Theme deleted successfully" });
                }
                else
                {
                    return BadRequest(new { Message = "There was an error while deleting theme." });

                }
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTheme([FromRoute] int id, [FromForm] string themeName)
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
            Theme? theme = await themeService.GetThemeById(id);
            if (theme == null)
            {
                return BadRequest(new { Message = $"There is no theme with ID={id}." });
            }
            if (string.IsNullOrEmpty(themeName))
            {
                return BadRequest(new { Message = "Theme must have name." });
            }
            bool result = await themeService.UpdateTheme(id,themeName);
            if (result)
            {
                return Ok(new { Message = "Theme updated successfully" });
            }
            else
            {
                return BadRequest(new { Message = "There was an error while updating theme." });

            }
        }

    }
}
