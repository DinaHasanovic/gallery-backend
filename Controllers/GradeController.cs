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
    public class GradeController : ControllerBase
    {
        private readonly IArtworkGradeService gradeService;
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly IArtworkService artworkService;

        public GradeController(IArtworkGradeService artworkGrade,IMapper mapper, IUserService userService, IArtworkService artworkService)
        {
            this.artworkService = artworkService;
            this.mapper = mapper;   
            this.userService = userService;
            this.gradeService = artworkGrade;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGrades() {
            return Ok(mapper.Map<List<GradeResponseDTO>>(await gradeService.GetAllGrades()));
        }
        [HttpGet("getByJuryMember/{juryMemberId}")]
        public async Task<IActionResult> GetAllGradesByJuryMember([FromRoute] int juryMemberId)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.JuryMember)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userId = User.FindFirst("id")?.Value;
            if(juryMemberId != int.Parse(userId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });

            }

            JuryMember? juryMember = await userService.GetJuryMemberData(juryMemberId);
            if (juryMember == null) {
                return BadRequest(new { Message = "User is not a jury member" });
            }

            return Ok(mapper.Map<List<GradeResponseDTO>>(await gradeService.GetAllGradesByJuryMember(juryMember.Id)));
        }
        //API za sve ocene dela
        [HttpGet("getByArtwork/{artworkId}")]
        public async Task<IActionResult> GetAllGradesByArtwork([FromRoute] int artworkId)
        {
            return Ok(mapper.Map<List<GradeResponseDTO>>(await gradeService.GetAllGradesByArtwork(artworkId)));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            ArtworkGrade? grade = await gradeService.GetArtworkGradeById(id);
            if (grade == null)
            {
                return BadRequest(new { Message = $"There is no grade with id {id}" });
            }
            else
            {
                return Ok(mapper.Map<GradeResponseDTO>(grade));
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateGrade([FromForm] CreateGradeRequestDTO request)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.JuryMember)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }

            User? user = await userService.GetUserById(request.JuryMemberId);
            if(user == null)
            {
                return BadRequest(new { Message = $"There is no user with id {request.JuryMemberId}" });
            }
            else if(user.Role != (int)UserRoles.JuryMember)
            {
                return BadRequest(new { Message = $"User with id {request.JuryMemberId} is not registered as jury member."});
            }
            JuryMember? juryMember = await userService.GetJuryMemberData(request.JuryMemberId);
            if (juryMember == null)
            {
                return BadRequest(new { Message = $"User with ID = {request.JuryMemberId} is not registered as jury member." });
            }
            List<ArtworkGrade> grades = await gradeService.GetAllGradesByJuryMember(juryMember.Id);
            ArtworkGrade? check = grades.Where(g => g.ArtworkId == request.ArtworkId).FirstOrDefault();
            if (check != null)
            {
                await gradeService.UpdateGrade(check.Id, request.Points);
                return Ok(new { Message = "Grade has been updated" });
            }
            request.JuryMemberId = juryMember.Id;
            ArtworkGrade grade = mapper.Map<ArtworkGrade>(request);
            bool result = await gradeService.CreateGrade(grade);
            if (result)
            {
                return Ok(new { Message = "Grade successfully created."});
            }
            else
            {
                return BadRequest(new { Message = "There was an error while creating grade" });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrade([FromRoute] int id, [FromForm] int newGrade)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.JuryMember)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }

            ArtworkGrade? grade = await gradeService.GetArtworkGradeById(id);
            if (grade == null)
            {
                return BadRequest(new { Message = $"Grade with id {id} doesn't exist." });
            }
            else
            {
                if (newGrade > 0 && newGrade < 6)
                {
                    bool result = await gradeService.UpdateGrade(id, newGrade);
                    if (result)
                    {
                        return Ok(new { Message = "Grade has been updated." });
                    }
                    else
                    {
                        return BadRequest(new { Message = "There was an error while updating grade." });
                    }
                }
                else
                {
                    return BadRequest(new { Message = "Grade must be a value between 1 and 5." });
                }
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrade([FromRoute] int id) {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.JuryMember)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            ArtworkGrade? grade = await gradeService.GetArtworkGradeById(id);
            if(grade == null)
            {
                return BadRequest(new { Message = $"Grade with id {id} doesn't exist." });
            }
            else
            {
                bool result = await gradeService.DeleteGrade(id);
                if (result)
                {
                return Ok(new { Message = "Grade deleted successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "There was an error while deleting grade." });
                }
            }
        }
        [HttpGet("averageGrade/{artworkId}")]
        public async Task<IActionResult> GetAverageGrade([FromRoute] int artworkId)
        {
            Artwork? art = await artworkService.GetArtworkById(artworkId);
            if(art == null)
            {
                return BadRequest(new { Message = "Artwork not found." });
            }
            List<ArtworkGrade> grades = await gradeService.GetAllGradesByArtwork(artworkId);
            if(grades.Count == 0)
            {
                return Ok(new { averageGrade = 0.0 });
            }
            float sum = 0;
            foreach (ArtworkGrade grade in grades)
            {
                sum += grade.Points;
            }

            return Ok(new { averageGrade= sum/grades.Count });
        }

    }
}
