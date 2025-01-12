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
    public class VisitController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IExhibitionService exhibitionService;
        private readonly IVisitService visitService;
        private readonly IMapper mapper;

        public VisitController(IUserService us, IExhibitionService es, IVisitService vs, IMapper m)
        {
            this.userService = us;
            this.exhibitionService = es;
            this.visitService = vs;
            this.mapper = m;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            
            return Ok(mapper.Map<List<VisitResponseDTO>>(await visitService.GetAll()));
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateVisitRequestDTO request)
        {

            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.Journalist)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            User? user = await userService.GetUserById(request.JournalistId);
            if (user == null)
            {
                return BadRequest(new { Message = "User doesn't exist." });
            }
            if (user.Role != (int)UserRoles.Journalist)
            {
                return BadRequest(new { Message = "User is not a journalist" });
            }
            Exhibition? ex = await exhibitionService.GetExhibitionById(request.ExhibitionId);
            if (ex == null)
            {
                return BadRequest(new { Message = "Exhibition doesn't exist." });
            }
            if (request.Date < ex.StartDate || request.Date > ex.EndDate)
            {
                return BadRequest(new { Message = "Visit must be between the start date and end date of exhibition." });
            }
            JournalistVisit jv = mapper.Map<JournalistVisit>(request);
            Journalist? j = await userService.GetJournalistData(request.JournalistId);
            if (j == null)
            {
                return BadRequest(new { Message = "Journalist is not registered." });
            }
            jv.Journalist = j;
            jv.Exhibition = ex;

            bool result = await visitService.CreateVisit(jv);
            if (result)
            {
                return Ok(new { Message = "Visit created successfully." });
            }
            else
            {
                return BadRequest(new { Message = "There was an error while creating a visit." });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != null && int.Parse(userRole) != (int)UserRoles.Journalist)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userId = User.FindFirst("id")?.Value;
            JournalistVisit? v = await visitService.GetVisitById(id);

            if (int.Parse(userId) != v.Journalist.UserId) {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            if (v == null)
            {
                return BadRequest(new { Message = "Visit doesn't exist." });
            }
            bool result = await visitService.DeleteVisit(id);
            if (result)
            {
                return Ok(new { Message = "Visit deleted successfully." });
            }
            else
            {
                return BadRequest(new { Message = "There was an error while deleting visit." });
            }
        }
        [HttpGet("{journalistId}")]
        public async Task<IActionResult> GetByJournalist([FromRoute] int journalistId)
        {
            User? user = await userService.GetUserById(journalistId);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found." });
            }
            else if(user.Role != (int)UserRoles.Journalist)
            {
                return BadRequest(new { Message = "User is not journalist." });
            }

            return Ok(mapper.Map<List<VisitResponseDTO>>(await visitService.GetVisitsByJournalist(journalistId)));
        }
    }
}
