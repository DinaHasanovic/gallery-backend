using AppBackEnd.Data;
using AppBackEnd.DTO;
using AppBackEnd.Interfaces;
using AppBackEnd.Profiles;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            this.userService = userService;
            this.mapper = mapper;
        }
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userService.GetAllUsers();

            if (users == null || !users.Any())
            {
                return NoContent();
            }

            return Ok(mapper.Map<List<UserSearchResponseDTO>>(await userService.GetAllUsers()));
        }

        [HttpGet("getUserByEmail/{email}")]
        public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
        {
            User? user = await userService.GetUserByEmail(email);

            if (user == null)
            {
                return NoContent();
            }
            else
            {
                return Ok(mapper.Map<UserSearchResponseDTO>(user));
            }

        }

        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            User? user = await userService.GetUserById(id);

            if (user == null)
            {
                return NoContent();
            }
            else
            {
                return Ok(mapper.Map<UserSearchResponseDTO>(user));
            }
        }
        [HttpPost("registerUser")]
        public async Task<IActionResult> RegisterUser([FromForm] RegisterUserRequestDTO request)
        {
            bool takenEmail = await userService.TakenEmail(request.Email);
            if (takenEmail)
            {
                return BadRequest(new { Message = "Email is taken" });
            }


            User user = mapper.Map<User>(request);
            user.Password = userService.HashPassword(request.Password);

            bool result = await userService.RegisterUser(user);
            if(result) return Ok(new { Message = "User successfully registered."});
            else
            {
                return BadRequest(new { Message = "There was an error while registering user." });
            }
        }

        [HttpPost("loginUser")]
        public async Task<IActionResult> LoginUser([FromForm] LoginUserRequestDTO request)
        {
            User? user = await userService.GetUserByEmail(request.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "User with this email not found" });
            }

            if (user.Password != userService.HashPassword(request.Password))
            {
                return BadRequest(new { Message = "Incorrect password." });
            }

            string token = userService.GenerateToken(user);

            switch (user.Role)
            {
                case (int)UserRoles.Visitor:
                case (int)UserRoles.Admin:
                    var visitorOrAdminResponse = mapper.Map<LoginUserResponseDTO>(user);
                    visitorOrAdminResponse.Token = token;
                    return Ok(visitorOrAdminResponse);

                case (int)UserRoles.Journalist:
                    Journalist? j = await userService.GetJournalistData(user.Id);
                    LoginJournalistResponseDTO journalistResponse = new LoginJournalistResponseDTO();
                    journalistResponse.Token = token;
                    journalistResponse.Role = user.Role;
                    journalistResponse.firstName = user.FirstName;
                    journalistResponse.lastName = user.LastName;
                    journalistResponse.agency=j.Agency;
                    journalistResponse.email = user.Email;
                    journalistResponse.Id = user.Id;

                    return Ok(journalistResponse);

                case (int)UserRoles.JuryMember:
                    LoginJuryMemberResponseDTO response = new LoginJuryMemberResponseDTO();
                    response.Id = user.Id;
                    response.firstName = user.FirstName;
                    response.lastName = user.LastName;
                    response.email = user.Email;
                    response.role = user.Role;
                    response.Token = token;
                    return Ok(response);

                case (int)UserRoles.Painter:
                    Painter? painter = await userService.GetPainterData(user.Id);
                    LoginPainterResponseDTO painterResponse = new LoginPainterResponseDTO();
                    painterResponse.firstName = user.FirstName;
                    painterResponse.lastName = user.LastName;
                    painterResponse.email = user.Email;
                    painterResponse.cityName = painter.City.Name;
                    painterResponse.JMBG = painter.JMBG;
                    painterResponse.Role = user.Role;
                    painterResponse.Token = token;
                    painterResponse.Id = user.Id;
                    return Ok(painterResponse);

                default:
                    return BadRequest(new { Message = "There seems to be an error" });
            }
        }

        [HttpPut("updateToVisitor/{id}")]
        public async Task<IActionResult> UpdateToVisitor([FromRoute] int id)
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

            User? user = await userService.GetUserById(id);
            if (user == null)
            {
                return BadRequest(new { Message = $"User with this id ${id} not found" });
            }
            else
            {
                await userService.UpdateUserToVisitor(id);
                return Ok();
            }
        }
        
        [HttpPut("updateToJournalist/{id}")]
        public async Task<IActionResult> UpdateToJournalist([FromRoute] int id, [FromForm] UpdateToJournalistRequestDTO request)
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

            bool result = await userService.UpdateUserToJournalist(id, request);
            if (!result)
            {
                return BadRequest(new { Message = $"User with id {id} not found or update failed" });
            }
            return Ok(new { Message = "User updated to journalist successfully."});
        }
        
        [HttpPut("updateToJury/{id}")]
        public async Task<IActionResult> UpdateToJury([FromRoute] int id, [FromForm] string JMBG)
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

            JuryMember? juryMember = await userService.GetJuryMemberWithJMBG(JMBG);
             
            if (juryMember != null)
            {
                return BadRequest(new { Message = $"{JMBG} is taken" });

            }
            bool result = await userService.UpdateUserToJury(id, JMBG);
            if (!result)
            {
                return BadRequest(new { Message = $"User with id {id} not found or update failed" });
            }
            return Ok(new { Message = "User updated to Jury Member successfully." });
        }

        [HttpPut("updateToPainter/{id}")]
        public async Task<IActionResult> UpdateToPainter([FromRoute] int id, [FromForm] UpdateToPainterRequestDTO request)
        {
            Painter? painter = await userService.GetPainterWithJMBG(request.JMBG);
            if (painter != null) {

                return BadRequest(new { Message = $"{request.JMBG} is taken" });

            }
            bool result = await userService.UpdateUserToPainter(id, request);
            if (!result)
            {
                return BadRequest(new { Message = $"User with id {id} not found or update failed" });
            }
            return Ok(new { Message = "User updated to Painter successfully." });
        }
        [HttpPut("updateUser/{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromForm] UpdateUserRequestDTO request)
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

            User? user = await userService.GetUserById(id);
            if (user == null)
            {
                return BadRequest(new { Message = $"User with id {id} not found or update failed" });
            }
            else
            {
                request.Id = id;
                bool result = await userService.UpdateUser(request);
                if (result)
                    return Ok(new { Message = "User successfully updated!" });
                else
                { return BadRequest(new { Message = "Error!" }); }
            }
        }

        [HttpGet("searchByEmail")]
        public async Task<IActionResult> SearchUsersByEmail([FromQuery] string? email)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if(userRole != null && int.Parse(userRole) != (int)UserRoles.Admin)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "You don't have access." });
            }

            if (email == null)
            {
            return BadRequest(new { Message = "Enter search parameters." });
            }
            if(email.Trim().Contains(' '))
            {
                return BadRequest(new { Message = "Email can't contain white spaces." });
            }
            return Ok(mapper.Map<List<UserSearchResponseDTO>>(await userService.SearchUsersByEmail(email.Trim())));
                
        }
        [HttpPut("updateToAdministrator/{id}")]
        public async Task<IActionResult> UpdateUserToAdmin([FromRoute] int id)
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

            bool result = await userService.UpdateUserToAdministrator(id);
            if (!result)
            {
                return BadRequest(new { Message = $"User with id {id} not found or update failed" });
            }
            return Ok(new { Message = "User updated to Administrator successfully." });
        }
        [HttpGet("getUsersByRole/{role}")]
        public async Task<IActionResult> GetUsersByRole([FromRoute] int role)
        {
         
            return Ok(mapper.Map<List<UserSearchResponseDTO>>(await userService.GetUsersByRole(role)));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
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

            bool result = await userService.DeleteUserById(id);
            if (!result)
            {
                return BadRequest(new { Message = "Deletion of user failed." });
            }
            else
            {
                return Ok(new { Message = "User has been successfully deleted." });
            }
        }
        [HttpGet("getPainterData/{userId}")]
        public async Task<IActionResult> GetPainterData([FromRoute] int userId)
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

            User? user = await userService.GetUserById(userId);
            if(user == null)
            {
                return BadRequest(new { Message = $"User with id = {userId} doesn't exist." });
            }
            Painter? painter = await userService.GetPainterData(userId);
            if(painter == null)
            {
                return BadRequest(new { Message = "Painter not found." });
            }
            return Ok(new { JMBG = painter.JMBG, cityId = painter.CityId });
        }
        [HttpGet("getJournalistData/{userId}")]
        public async Task<IActionResult> GetJournalistData([FromRoute] int userId)
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

            User? user = await userService.GetUserById(userId);
            if (user == null)
            {
                return BadRequest(new { Message = $"User with id = {userId} doesn't exist." });
            }
            Journalist? journalist= await userService.GetJournalistData(userId);
            if (journalist == null)
            {
                return BadRequest(new { Message = "Journalist not found." });
            }
            return Ok(new { agency = journalist.Agency});
        }

        [HttpGet("getJuryMemberData/{userId}")]
        public async Task<IActionResult> GetJuryMemberData([FromRoute] int userId)
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

            User? user = await userService.GetUserById(userId);
            if (user == null)
            {
                return BadRequest(new { Message = $"User with id = {userId} doesn't exist." });
            }
            JuryMember? juryMember= await userService.GetJuryMemberData(userId);
            if (juryMember == null)
            {
                return BadRequest(new { Message = "Jury member not found." });
            }
            return Ok(new { JMBG=juryMember.JMBG });
        }
        [HttpGet("paintersUser/{painterId}")]
        public async Task<IActionResult> GetPaintersUser([FromRoute] int painterId)
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

            User? user = await userService.GetPaintersUser(painterId);
            if(user == null)
            {
                return BadRequest(new { Message = $"Non existent user." });
            }
            return Ok(user);
        }
    }
}
