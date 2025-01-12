using AppBackEnd.Data;

namespace AppBackEnd.Interfaces
{
    public interface IThemeService
    {
        public Task<List<Theme>> GetAllThemes();
        public Task<Theme?> GetThemeById(int id);
        public Task<bool> CreateTheme(String themeName);
        public Task<bool> UpdateTheme(int id,string Name);
        public Task<bool> DeleteThemeById(int id);
    }
}
