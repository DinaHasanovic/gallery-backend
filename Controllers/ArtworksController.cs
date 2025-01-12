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
    public class ArtworksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IArtworkService artworkService;
        private readonly IUserService userService;
        private readonly IHallService hallService;
        private readonly IThemeService themeService;

        public ArtworksController( IMapper mapper,IThemeService themeService, IArtworkService artwork, IUserService userService,IHallService hallService)
        {
            this.mapper = mapper;
            this.themeService = themeService;
            this.artworkService = artwork;
            this.userService = userService;
            this.hallService = hallService;
        }
        //API za sve slike
        [HttpGet]
        public async Task<IActionResult> GetAllArtworks() {
            return Ok(mapper.Map<List<ArtworkResponseDTO>>(await artworkService.GetAllArtworks()));        
        }
        //API za jednu sliku sa ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArtworkById([FromRoute] int id)
        {
            Artwork? artwork = await artworkService.GetArtworkById(id);
            if (artwork == null)
            {
                return BadRequest(new { Message = $"Artwork with ID={id} doesn't exist." });
            }
            return Ok(mapper.Map<ArtworkResponseDTO>(artwork));
        }
        //API za sliku sa jedinstvenim kodom
        [HttpGet("getArtworkByCode/{uniqueCode}")]
        public async Task<IActionResult> GetArtworkByUniqueCode([FromRoute] string uniqueCode)
        {
            Artwork? artwork = await artworkService.GetArtworkWithCode(uniqueCode);
            if (artwork == null)
            {
                return BadRequest(new { Message = $"Artwork with code {uniqueCode} doesn't exist." });
            }

            return Ok(mapper.Map<ArtworkResponseDTO>(artwork));
        }
        //API za sve slike slikara
        [HttpGet("getArtworksByPainterId/{painterId}")]
        public async Task<IActionResult> GetArtworksByArtist([FromRoute] int painterId)
        {
            User? user = await userService.GetUserById(painterId);
            if (user == null) {
                return BadRequest(new { Message = $"User with ID={painterId} doesn't exist." });
            }
            if( user.Role != (int)UserRoles.Painter)
            {
                return BadRequest(new { Message = $"User with ID={painterId} isn't registered as painter." });
            }
            Painter? painter = await userService.GetPainterData(painterId);
            if (painter == null) {
                return BadRequest(new { Message = $"Painter not found." });
            }
            return Ok(mapper.Map<List<ArtworkResponseDTO>>(await artworkService.GetArtworksByArtistId(painter.Id)));
        }
        //API za dodavanje slika
        [HttpPost]
        public async Task<IActionResult> CreateArtwork([FromForm] CreateArtworkRequestDTO request)
        {
            //Provera da li korisnik sme da doda delo
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            //Iz tokena kupi ulogu korisnika kako bi proverio da li sme da odradi akciju
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.Painter)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }

            User? user = await userService.GetUserById(request.PainterId);
            if (user == null)
            {
                return BadRequest(new { Message = $"User with ID={request.PainterId} doesn't exist." });
            }
            if (user.Role != (int)UserRoles.Painter)
            {
                return BadRequest(new { Message = $"User with ID={request.PainterId} isn't registered as painter." });
            }
            //da li postoji slika sa istim kodom
            Artwork? existingArtwork = await artworkService.GetArtworkWithCode(request.UniqueCode);
            if (existingArtwork != null)
            {
                return BadRequest(new { Message = $"Artwork with code {request.UniqueCode} already exists." });
            }
            //da li postoji uneta lokacija
            if (request.HallId > 0)
            {
                Hall? hall = await hallService.GetHallById((int)request.HallId);
                if (hall == null)
                {
                    return BadRequest(new { Message = $"Hall with ID={request.HallId} doesn't exist." });
                }
            }
            //da li postoji tema kojoj slika pripada
            if (request.ThemeId > 0)
            {
                Theme? theme = await themeService.GetThemeById(request.ThemeId);
                if (theme == null)
                {
                    return BadRequest(new { Message = $"Theme with ID={request.ThemeId} doesn't exist." });
                }
                Painter? painter = await userService.GetPainterData(request.PainterId);
                if (painter == null)
                {
                    return BadRequest(new { Message = "Painter not found in database." });
                }
                if (theme.Artworks.Any(a => a.PainterId == painter.Id))
                {
                    return BadRequest(new { Message = $"Painter with ID={request.PainterId} already has an artwork in theme ID {request.ThemeId}." });
                }
            }
            //da li postoji fajl prosledjen sa zahtevom
            if (request.Image != null && request.Image.Length > 0)
            {
                var painterFolder = Path.Combine("wwwroot", "uploads", request.PainterId.ToString()); 
                Directory.CreateDirectory(painterFolder); 

                var fileExtension = Path.GetExtension(request.Image.FileName);
                if (string.IsNullOrEmpty(fileExtension))
                {
                    return BadRequest(new { Message = "File extension is missing." });
                }

                var randomFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(painterFolder, randomFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(stream);
                }

                var artwork = mapper.Map<Artwork>(request);
                artwork.ImageUrl = Path.Combine("uploads", request.PainterId.ToString(), randomFileName);

                bool result = await artworkService.CreateArtwork(artwork);
                if (result)
                {
                    return Ok(new { Message = "Artwork created successfully!" });
                }
            }

            return BadRequest(new { Message = "There was an error while creating the artwork." });

        }
        //API za azuriranje slike
        [HttpPut]
        public async Task<IActionResult> UpdateArtwork([FromForm] UpdateArtworkRequestDTO request)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.Admin && int.Parse(userRole) != (int)UserRoles.Painter)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }


            Artwork? existingArtwork = await artworkService.GetArtworkById(request.Id);
            if (existingArtwork == null)
            {
                return BadRequest(new { Message = $"Artwork with ID {request.Id} doesn't exist." });
            }

            if (!string.IsNullOrEmpty(request.UniqueCode))
            {
                Artwork? artworkWithSameCode = await artworkService.GetArtworkWithCode(request.UniqueCode);
                if (artworkWithSameCode != null && artworkWithSameCode.Id != request.Id)
                {
                    return BadRequest(new { Message = $"Code {request.UniqueCode} is already taken." });
                }
            }

            if (request.HallId.HasValue && request.HallId > 0)
            {
                Hall? hall = await hallService.GetHallById((int)request.HallId);
                if (hall == null)
                {
                    return BadRequest(new { Message = $"Hall with ID {request.HallId} doesn't exist." });
                }
            }

            bool result = await artworkService.UpdateArtwork(request);
            if (result)
            {
                return Ok(new { Message = "Artwork successfully updated." });
            }

            return BadRequest(new { Message = "There was an error while updating the artwork." });
        }
        //API za brisanje slike
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArtwork([FromRoute] int id)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.Admin && int.Parse(userRole) != (int)UserRoles.Painter)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }

            Artwork? artwork = await artworkService.GetArtworkById(id);
            if (artwork == null)
            {
                return BadRequest(new { Message = $"Artwork with ID={id} doesn't exist." });
            }

            if (!string.IsNullOrEmpty(artwork.ImageUrl))
            {
                var filePath = Path.Combine("wwwroot", artwork.ImageUrl);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            bool result = await artworkService.DeleteArtworkById(id);
            if (result)
            {
                return Ok(new { Message = "Artwork successfully deleted." });
            }

            return BadRequest(new { Message = "There was an error while deleting the artwork." });
        }
        //API za pretragu sa naslovom
        [HttpGet("searchByTitle")]
        public async Task<IActionResult> SearchByTitle([FromQuery]  string title)
        {
            return Ok(mapper.Map<List<ArtworkResponseDTO>>(await artworkService.SearchByTitle(title)));
        }
        //API za pretragu sa kodom
        [HttpGet("searchByCode")]
        public async Task<IActionResult> SearchByCode([FromQuery] string code)
        {
            return Ok(mapper.Map<List<ArtworkResponseDTO>>(await artworkService.SearchByCode(code)));
        }

    }
   
}
