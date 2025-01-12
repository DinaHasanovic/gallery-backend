using AppBackEnd.Data;
using AppBackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBackEnd.Services
{
    public class ThemeService : IThemeService
    {
        private readonly DatabaseContext db;

        public ThemeService(DatabaseContext db)
        {
            this.db = db;
        }


        public async Task<bool> CreateTheme(String themeName)
        {
            try
            {
                Theme theme = new Theme
                {
                    Name = themeName
                };
                db.Themes.Add(theme);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }

        public async Task<bool> DeleteThemeById(int id)
        {
            try
            {

                Theme? themeToDelete = await db.Themes.Where(t => t.Id == id).Include(t => t.Artworks).FirstOrDefaultAsync();
                if (themeToDelete == null)
                {
                    return false;
                }
                else
                {
                    List<Artwork> artworks = await db.Artworks.Where( a => a.ThemeId == themeToDelete.Id ).ToListAsync();
                    if (artworks.Any())
                    {
                        db.Artworks.RemoveRange(artworks);
                    }
                    db.Themes.Remove(themeToDelete);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex) {
                return false;
            }
        }

        public async Task<List<Theme>> GetAllThemes()
        {
            return await db.Themes.Include(t => t.Artworks).ToListAsync();
        }

        public async Task<Theme?> GetThemeById(int id)
        {
            return await db.Themes.Where(t => t.Id == id).Include(t => t.Artworks).FirstOrDefaultAsync();        
        }


        public async Task<bool> UpdateTheme(int id,string Name)
        {
            try
            {
                Theme? themeToUpdate = await db.Themes.Where( t => t.Id == id).FirstOrDefaultAsync();
                if (themeToUpdate == null) {
                    return false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(Name.Trim()))
                    {
                        themeToUpdate.Name = Name;
                        await db.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch(Exception ex) 
            {
                return false;
            }
        }
    }
}
