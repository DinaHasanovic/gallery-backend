using AppBackEnd.Data;
using AppBackEnd.DTO;
using AppBackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppBackEnd.Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext db;
        private readonly IConfiguration configuration;

        public UserService(DatabaseContext db,IConfiguration configuration) { 
            this.db = db;
            this.configuration = configuration;
        }

        public async Task<bool> DeleteUserByEmail(string email)
        {
            User? user = await db.Users.Where( u => u.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return false;
            }
            else
            {
                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteUserById(int id)
        {
            User? user = await db.Users.Where( u => u.Id == id).FirstOrDefaultAsync();
            if (user == null) { 
            return false;
            }
            else
            {
                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return true;
            }
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Auth:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("id", user.Id.ToString()),
             new Claim("email", user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        public async Task<List<User>?> GetAllUsers()
        {
            return await db.Users.ToListAsync();
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await db.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public string HashPassword(string password)
        {
            if (String.IsNullOrEmpty(password))
            {
                return String.Empty;
            }

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha.ComputeHash(textBytes);

                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }


        public async Task<bool> TakenEmail(string email)
        {
            User? user = await db.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null) {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> RegisterUser(User User)
        {
           bool emailTaken = await this.TakenEmail(User.Email);
            if (emailTaken)
            {
                return false;
            }
            else
            {
                db.Users.Add(User);
                await db.SaveChangesAsync();
                return true;
            }

        }

        public async Task<bool> UpdateUser(UpdateUserRequestDTO userRequest)
        {
            User? user = await db.Users.Where(u => u.Id == userRequest.Id).FirstOrDefaultAsync();
            if (user == null)
            {
                return false;
            }

            switch ((UserRoles)user.Role)
            {
                case UserRoles.Admin:
                    if (user.FirstName != userRequest.FirstName && !string.IsNullOrEmpty(userRequest.FirstName)) user.FirstName = userRequest.FirstName;
                    if (user.LastName != userRequest.LastName && !string.IsNullOrEmpty(userRequest.LastName)) user.LastName = userRequest.LastName;
                    if (user.Email != userRequest.Email && !string.IsNullOrEmpty(userRequest.Email)) user.Email = userRequest.Email;
                    if (!string.IsNullOrEmpty(userRequest.Password) && user.Password != this.HashPassword(userRequest.Password)) user.Password = this.HashPassword(userRequest.Password);
                    break;

                case UserRoles.Journalist:
                    if (user.FirstName != userRequest.FirstName && !string.IsNullOrEmpty(userRequest.FirstName)) user.FirstName = userRequest.FirstName;
                    if (user.LastName != userRequest.LastName && !string.IsNullOrEmpty(userRequest.LastName)) user.LastName = userRequest.LastName;
                    if (user.Email != userRequest.Email && !string.IsNullOrEmpty(userRequest.Email)) user.Email = userRequest.Email;
                    if (!string.IsNullOrEmpty(userRequest.Password) && user.Password != this.HashPassword(userRequest.Password)) user.Password = this.HashPassword(userRequest.Password);
                    Journalist? journalist = await db.Journalists.FirstOrDefaultAsync(j => j.UserId == user.Id);
                    if (journalist != null)
                    {
                        if (journalist.Agency != userRequest.Agency) journalist.Agency = userRequest.Agency;
                    }
                    break;

                case UserRoles.JuryMember:
                    if (user.FirstName != userRequest.FirstName && !string.IsNullOrEmpty(userRequest.FirstName)) user.FirstName = userRequest.FirstName;
                    if (user.LastName != userRequest.LastName && !string.IsNullOrEmpty(userRequest.LastName)) user.LastName = userRequest.LastName;
                    if (user.Email != userRequest.Email && !string.IsNullOrEmpty(userRequest.Email)) user.Email = userRequest.Email;
                    if (!string.IsNullOrEmpty(userRequest.Password) && user.Password != this.HashPassword(userRequest.Password)) user.Password = this.HashPassword(userRequest.Password);
                    if (!string.IsNullOrEmpty(userRequest.JMBG) && userRequest.JMBG.Length == 13)
                    {
                        JuryMember? juryMember = await db.JuryMembers.FirstOrDefaultAsync(j => j.UserId == user.Id);
                        if (juryMember != null && juryMember.JMBG != userRequest.JMBG)
                        {
                            juryMember.JMBG = userRequest.JMBG;
                        }
                    }
                    break;

                case UserRoles.Painter:
                    if (user.FirstName != userRequest.FirstName && !string.IsNullOrEmpty(userRequest.FirstName)) user.FirstName = userRequest.FirstName;
                    if (user.LastName != userRequest.LastName && !string.IsNullOrEmpty(userRequest.LastName)) user.LastName = userRequest.LastName;
                    if (user.Email != userRequest.Email && !string.IsNullOrEmpty(userRequest.Email)) user.Email = userRequest.Email;
                    if (!string.IsNullOrEmpty(userRequest.Password) && user.Password != this.HashPassword(userRequest.Password)) user.Password = this.HashPassword(userRequest.Password);
                    Painter? painter = await db.Painters.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    if (painter != null)
                    {
                        if (!string.IsNullOrEmpty(userRequest.JMBG) && userRequest.JMBG.Length == 13 && painter.JMBG != userRequest.JMBG)
                        {
                            painter.JMBG = userRequest.JMBG;
                        }

                        if (userRequest.CityId.HasValue && painter.CityId != userRequest.CityId.Value)
                        {
                            painter.CityId = userRequest.CityId.Value;
                        }
                    }
                    break;


                case UserRoles.Visitor:
                    if (user.FirstName != userRequest.FirstName && !string.IsNullOrEmpty(userRequest.FirstName)) user.FirstName = userRequest.FirstName;
                    if (user.LastName != userRequest.LastName && !string.IsNullOrEmpty(userRequest.LastName)) user.LastName = userRequest.LastName;
                    if (user.Email != userRequest.Email && !string.IsNullOrEmpty(userRequest.Email)) user.Email = userRequest.Email;
                    if (!string.IsNullOrEmpty(userRequest.Password) && user.Password != this.HashPassword(userRequest.Password)) user.Password = this.HashPassword(userRequest.Password);
                    break;

                default:
                    return false;
            }

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserToAdministrator(int id)
        {
           User? user = await db.Users.Where( u => u.Id == id ).FirstOrDefaultAsync();
            if (user == null)
            {
                return false;
            }
            else
            {
                switch ((UserRoles)user.Role)
                {

                    case UserRoles.Painter:
                        await ChangeFromPainter(user.Id);
                        break;
                    case UserRoles.Journalist:
                        await ChangeFromJournalist(user.Id);
                        break;
                    case UserRoles.JuryMember:
                        await ChangeFromJuryMember(user.Id);
                        break;
                }
                user.Role = (int) UserRoles.Admin;
                await db.SaveChangesAsync();
                return true;
            }

        }

        public async Task<bool> UpdateUserToJournalist(int id, UpdateToJournalistRequestDTO request)
        {
            User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return false;
            }
            else
            {
                switch ((UserRoles)user.Role)
                {

                    case UserRoles.Painter:
                        await ChangeFromPainter(user.Id);
                        break;
                    case UserRoles.JuryMember:
                        await ChangeFromJuryMember(user.Id);
                        break;
                }
                user.Role = (int)UserRoles.Journalist;

                Journalist journalist = new Journalist
                {
                    UserId = user.Id,
                    Agency = request.agency
                };
                db.Journalists.Add(journalist);
                await db.SaveChangesAsync(); 

                JournalistVisit visit = new JournalistVisit
                {
                    JournalistId = journalist.Id,
                    ExhibitionId = request.exhibitionId,
                    Date = request.date
                };
                db.Visits.Add(visit);

                await db.SaveChangesAsync(); 
                return true;
            }
        }


        public async Task<bool> UpdateUserToJury(int id, string JMBG)
        {
            User? user = await db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return false; 
            }
            switch ((UserRoles)user.Role)
            {

                case UserRoles.Painter:
                    await ChangeFromPainter(user.Id);
                    break;
                case UserRoles.Journalist:
                    await ChangeFromJournalist(user.Id);
                    break;
            }
            user.Role = (int)UserRoles.JuryMember;

            JuryMember juryMember = new JuryMember
            {
                UserId = user.Id,
                JMBG = JMBG
            };

            db.JuryMembers.Add(juryMember);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserToPainter(int id, UpdateToPainterRequestDTO request)
        {
            User? user = await db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return false; 
            }

            switch ((UserRoles)user.Role)
            {
                case UserRoles.Journalist:
                    await ChangeFromJournalist(user.Id);
                    break;
                case UserRoles.JuryMember:
                    await ChangeFromJuryMember(user.Id);
                    break;
            }
            user.Role = (int)UserRoles.Painter;

            Painter painter = new Painter
            {
                UserId = user.Id,
                JMBG = request.JMBG,
                CityId = request.CityId
            };

            db.Painters.Add(painter);

            await db.SaveChangesAsync();

            return true; 
        }


        public async Task<bool> UpdateUserToVisitor(int id)
        {
            User? user = await db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return false;
            }
            else
            {
                switch ((UserRoles)user.Role)
                {

                    case UserRoles.Painter:
                        await ChangeFromPainter(user.Id);
                        break;
                    case UserRoles.Journalist:
                        await ChangeFromJournalist(user.Id);
                        break;
                    case UserRoles.JuryMember:
                        await ChangeFromJuryMember(user.Id);
                        break;
                }

                user.Role = (int) UserRoles.Visitor;
                await db.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> ChangeFromPainter(int id)
        {
            try
            {
                Painter? painter = await db.Painters.Where( p => p.UserId == id ).FirstOrDefaultAsync();
                if( painter != null)
                {
                    List<Artwork> artworks = await db.Artworks.Where( a => a.PainterId == id ).ToListAsync();
                    if (artworks.Any()) {
                        db.Artworks.RemoveRange(artworks);
                    }
                    db.Painters.Remove(painter);
                    await db.SaveChangesAsync();
                }
                    return true;

            }
            catch (Exception ex) {
                return false;
            }
        }

        public async Task<bool> ChangeFromJournalist(int id)
        {
            try
            {
                Journalist? journalist = await db.Journalists.Where( j => j.UserId == id ).FirstOrDefaultAsync();
                if (journalist != null) {
                    List<JournalistVisit> visits = await db.Visits.Where( v => v.JournalistId == id).ToListAsync();
                    if (visits.Any()) { 
                        db.Visits.RemoveRange(visits);
                    }
                    db.Journalists.Remove(journalist);
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ChangeFromJuryMember(int id)
        {
            try
            {

                JuryMember? juryMember = await db.JuryMembers.Where(j => j.UserId == id).FirstOrDefaultAsync();
                if (juryMember != null)
                {
                    List<ArtworkGrade> grades = await db.Grades.Where( a => a.JuryMemberId == juryMember.Id).ToListAsync();
                    if (grades.Any()) { 
                        db.Grades.RemoveRange(grades);
                    }
                    db.JuryMembers.Remove(juryMember);
                }
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) {
                return true;
            }
        }

        public async Task<Painter?> GetPainterData(int id)
        {
            try
            {
                Painter? painter = await db.Painters.Where( p => p.UserId == id).Include( p => p.City).FirstOrDefaultAsync();
                return painter;

            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<JuryMember?> GetJuryMemberData(int id)
        {
            try {
                JuryMember? juryMember = await db.JuryMembers.Where(j => j.UserId == id).FirstOrDefaultAsync();
                return juryMember;
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<User>> SearchUsersByEmail(string email)
        {
            return await db.Users.Where( u => u.Email.StartsWith(email) ).ToListAsync();
        }

        public async Task<List<User>> GetUsersByRole(int role)
        {
            return await db.Users.Where(u => u.Role == role).ToListAsync();
        }

        public async Task<JuryMember?> GetJuryMemberWithJMBG(string JMBG)
        {
           return await db.JuryMembers.Where( j => j.JMBG == JMBG).FirstOrDefaultAsync();
        }

        public async Task<Painter?> GetPainterWithJMBG(string JMBG)
        {
            return await db.Painters.Where(p => p.JMBG == JMBG).FirstOrDefaultAsync();
        }

        public async Task<Journalist?> GetJournalistData(int id)
        {
            return await db.Journalists.Where( j => j.UserId== id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetPaintersUser(int painterId)
        {
            Painter? painter = await db.Painters.Where(p => p.Id == painterId).FirstOrDefaultAsync();
            if (painter == null)
            {
                return null;
            }
            else
            {
                return await db.Users.Where( u => u.Id == painter.UserId ).FirstOrDefaultAsync();
            }
        }
    }
}
