using AppBackEnd.Data;
using AppBackEnd.DTO;
using AppBackEnd.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;

namespace AppBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExhibitionController : ControllerBase
    {
        private readonly IExhibitionService exhibitionService;
        private readonly IMapper mapper;
        private readonly IArtworkService artworkService;
        private readonly IHallService hallService;

        public ExhibitionController(IHallService hallService,IMapper mapper,IArtworkService artworkService,IExhibitionService exhibitionService)
        {
            this.hallService = hallService;
            this.mapper = mapper;
            this.artworkService = artworkService;
            this.exhibitionService = exhibitionService;
        }
        //API za sve egzibicije
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(mapper.Map<List<ExhibitionResponseDTO>>(await exhibitionService.GetAllExhibitions()));
        }
        //API za sve egzibicije sa ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExhibitionById([FromRoute] int id) { 
            Exhibition? exhibition = await exhibitionService.GetExhibitionById(id);
            if (exhibition == null) {
                return BadRequest(new { Message = $"Exhibition with ID = {id} doesn't exist." });
            }
            return Ok(mapper.Map<ExhibitionResponseDTO>(exhibition));
        }
        //API za kreiranje egzibicije
        [HttpPost]
        public async Task<IActionResult> CreateExhibition([FromForm] CreateExhibitionRequestDTO request) {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.Admin)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            Exhibition exhibition = mapper.Map<Exhibition>(request);
            if (exhibition == null)
            {
                return BadRequest(new { Message = "There was an error while creating exhibition." });
            }
            else
            {
                Hall? hall = await hallService.GetHallById(request.HallId);
                if(hall == null)
                {
                    return BadRequest(new { Message = $"Hall with ID={request.HallId} doesn't exist." });
                }

                bool result = await exhibitionService.CreateExhibition(exhibition);
                if (result)
                {
                    return Ok(new { Message = "Exhibition created successfully" });
                }
                else
                {
                    return BadRequest(new { Message = "There was an error while creating exhibition." });
                }
            }
        }
        //API za azuriranje
        [HttpPut]
        public async Task<IActionResult> UpdateExhibition([FromForm] UpdateExhibitionRequestDTO request)
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
            Exhibition? exhibition = await exhibitionService.GetExhibitionById(request.Id);
            if (exhibition == null)
            {
                return BadRequest(new { Message = $"Exhibition with ID = {request.Id} doesn't exist." });
            }
            else
            {
                if(request.HallId != null && request.HallId >0 && request.HallId != exhibition.HallId)
                {
                    Hall? hall = await hallService.GetHallById((int)request.HallId);
                    if(hall == null)
                    {
                        return BadRequest(new { Message = $"Hall with ID={request.HallId} doesn't exist." });
                    }
                }
                bool result = await exhibitionService.UpdateExhibition(request);
                if (result)
                {
                    return Ok(new { Message = "Exhibition updated successfully" });
                }
                else
                {
                    return BadRequest(new { Message = "There was an error while updating exhibition" });
                }

            }
        }
        //API za brisanje
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExhibition([FromRoute] int id)
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
            Exhibition? exhibition = await exhibitionService.GetExhibitionById(id);
            if (exhibition == null)
            {
                return BadRequest(new { Message = $"Exhibition with ID = {id} doesn't exist." });
            }
            else
            {
                bool result = await exhibitionService.DeleteExhibition(id);
                if (result)
                {
                    return Ok(new { Message = "Exhibition deleted successfully" });
                }
                else
                {
                    return BadRequest(new { Message = "There was an error while deleting exhibition" });
                }
            }
        }
        //API za citanje sva dela egzibicije
        [HttpGet("ArtworksOfExhibition/{id}")]
        public async Task<IActionResult> GetAllArtworks([FromRoute] int id) {
            Exhibition? exhibition = await exhibitionService.GetExhibitionById(id);
            if (exhibition == null)
            {
                return BadRequest(new { Message = $"Exhibition with ID = {id} doesn't exist." });
            }
            {
                return Ok(mapper.Map<List<ArtworkResponseDTO>>(await exhibitionService.GetAllArtworksOfExhibition(id)));
            }

        }
        //API za dodavanje slike u egzibiciju
        [HttpPost("addPictureToExhibition/{exhibitionId}")]
        public async Task<IActionResult> AddPictureToExhibition([FromRoute] int exhibitionId, [FromForm] int PictureId)
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

            Exhibition? exhibition = await exhibitionService.GetExhibitionById(exhibitionId);
            if(exhibition == null)
            {
                return BadRequest(new { Message = $"Exhibition with ID = {exhibitionId} doesn't exist." });
            }
            else
            {
                Artwork? artwork = await artworkService.GetArtworkById(PictureId);
                if(artwork == null)
                {
                    return BadRequest(new { Message = $"Artwork with ID = {PictureId} doesn't exist." });
                }
                else
                {
                   bool result = await exhibitionService.AddPictureToExhibition(exhibitionId,PictureId);
                    if (result)
                    {
                        return Ok(new { Message = "Artwork successfully added to exhibition" });
                    }
                    else
                    {
                        return BadRequest(new { Message = "There was an error while adding artwork to exhibition" });
                    }
                }
            }
        }
        //API za uklanjanje slike sa egzibicije
        [HttpPost("removePictureFromExhibition/{exhibitionId}")]
        public async Task<IActionResult> RemovePictureFromExhibition([FromRoute] int exhibitionId, [FromForm] int PictureId) {

            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.Admin)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }

            Exhibition? exhibition = await exhibitionService.GetExhibitionById(exhibitionId);
            if (exhibition == null)
            {
                return BadRequest(new { Message = $"Exhibition with ID = {exhibitionId} doesn't exist." });
            }
            else
            {
                Artwork? artwork = await artworkService.GetArtworkById(PictureId);
                if (artwork == null)
                {
                    return BadRequest(new { Message = $"Artwork with ID = {PictureId} doesn't exist." });
                }
                else
                {
                    bool result = await exhibitionService.RemovePictureFromExhibition(exhibitionId, PictureId);
                    if (result)
                    {
                        return Ok(new { Message = "Artwork successfully removed from exhibition" });
                    }
                    else
                    {
                        return BadRequest(new { Message = "There was an error while removing artwork from exhibition" });
                    }
                }
            }
        }
        //API za pretragu sa naslovom
        [HttpGet("searchByTitle")]
        public async Task<IActionResult> GetByTitleSearch([FromQuery] string title)
        {
            return Ok(mapper.Map<List<ExhibitionResponseDTO>>(await exhibitionService.GetExhibitionsWithTitle(title)));
        }
        //API za pretragu sa mestom
        [HttpGet("searchByPlaces")]
        public async Task<IActionResult> GetByPlaces([FromQuery] string PTT)
        {
            return Ok(mapper.Map<List<ExhibitionResponseDTO>>(await exhibitionService.GetExhibitionsWithPlaces(PTT)));
        }
        //API za pretragu sa datumima
        [HttpGet("searchByDates")]
        public async Task<IActionResult> GetByPlaces([FromQuery] SearchByDatesDTO request)
        {
            return Ok(mapper.Map<List<ExhibitionResponseDTO>>(await exhibitionService.GetExhibitionsWithBetweenDates(request.startDate,request.endDate)));
        }

    }
}
