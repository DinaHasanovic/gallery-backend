using AppBackEnd.Data;

namespace AppBackEnd.Interfaces
{
    public interface IThemeService
    { //kreiranje, ažuriranje, brisanje i pretragu tema.
        public Task<List<Theme>> GetAllThemes();
        public Task<Theme?> GetThemeById(int id);
        public Task<bool> CreateTheme(String themeName);
        public Task<bool> UpdateTheme(int id,string Name);
        public Task<bool> DeleteThemeById(int id);
    }
}
