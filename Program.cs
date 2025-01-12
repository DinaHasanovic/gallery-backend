using AppBackEnd.Data;
using AppBackEnd.Interfaces;
using AppBackEnd.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Dodavanje osnovnih servisa
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>(o => o.UseSqlServer(configuration.GetConnectionString("LocalConnection")));

// Registracija servisa
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IHallService, HallService>();
builder.Services.AddScoped<IArtworkService, ArtworkService>();
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<IExhibitionService, ExhibitionService>();
builder.Services.AddScoped<IArtworkGradeService, ArtworkGradeService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddAutoMapper(typeof(Program));

// Konfiguracija JWT autentifikacije
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Auth:Secret"])),
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.RequireHttpsMetadata = false; // Ako koristiš HTTP u razvojnom okruženju
});

// Konfiguracija CORS-a
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigins", builder =>
//    {
//        builder.WithOrigins("http://localhost:4200") // Frontend URL
//               .AllowAnyHeader()
//               .AllowAnyMethod()
//               .AllowCredentials();
//    });
//});

builder.Services.AddCors();

var app = builder.Build();

// Konfiguracija aplikacije
//app.UseCors("AllowSpecificOrigins");

app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Preporučuje se u produkciji za HTTPS
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllers();

app.Run();
