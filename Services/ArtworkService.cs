using AppBackEnd.Data;
using AppBackEnd.DTO;
using AppBackEnd.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace AppBackEnd.Services
{
    public class ArtworkService : IArtworkService
    {
        private readonly DatabaseContext database;

        public ArtworkService(DatabaseContext db)
        {
            this.database = db;
        }

        public async Task<bool> CreateArtwork(Artwork artwork)
        {
            try
            {
                User? user = await database.Users
                    .FirstOrDefaultAsync(u => u.Id == artwork.PainterId);
                if (user == null || user.Role != (int)UserRoles.Painter)
                {
                    return false;
                }

                Artwork? existingArtwork = await database.Artworks
                    .FirstOrDefaultAsync(a => a.UniqueCode == artwork.UniqueCode);
                if (existingArtwork != null)
                {
                    return false;
                }

                Painter? painter = await database.Painters
                    .FirstOrDefaultAsync(p => p.UserId == artwork.PainterId);
                if (painter == null)
                {
                    return false;
                }

                if (artwork.ThemeId.HasValue)
                {
                    var existingArtworkInTheme = await database.Artworks
                        .FirstOrDefaultAsync(a => a.ThemeId == artwork.ThemeId && a.PainterId == painter.Id);
                    if (existingArtworkInTheme != null)
                    {
                        Console.WriteLine($"Painter already has an artwork in theme ID {artwork.ThemeId}");
                        return false;
                    }
                }

                artwork.PainterId = painter.Id;

                database.Artworks.Add(artwork);
                await database.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating artwork: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteArtworkById(int artworkId)
        {
            try
            {

            Artwork? artwork = await database.Artworks.Where(a => a.Id == artworkId).FirstOrDefaultAsync();
            if (artwork != null) {
                database.Artworks.Remove(artwork);
                await database.SaveChangesAsync();
                return true;
            }
            
            return false;
            }
            catch(Exception ex) { return false; }
        }

        public async Task<List<Artwork>> GetAllArtworks()
        {
            
            return await database.Artworks.Include( a => a.Painter).Include( a => a.Painter.User).Include( a => a.Hall).Include( a => a.Hall != null? a.Hall.City: null).Include(a => a.Theme).ToListAsync();
            
        }

        public async Task<Artwork?> GetArtworkById(int id)
        {
            return await database.Artworks.Where(a => a.Id == id).Include(a => a.Painter.User).Include(a => a.Painter).Include(a => a.Hall).Include(a => a.Hall != null ? a.Hall.City : null).Include(a => a.Theme).FirstOrDefaultAsync();
        }

        public async Task<List<Artwork>> GetArtworksByArtistId(int artistId)
        {
            return await database.Artworks.Where(a => a.PainterId == artistId).Include(a => a.Painter).Include(a => a.Painter.User).Include(a => a.Hall).Include(a => a.Hall != null ? a.Hall.City : null).Include(a => a.Theme).ToListAsync();
        }

        public async Task<Artwork?> GetArtworkWithCode(string code)
        {
            return await database.Artworks.Where(a => a.UniqueCode == code).Include(a => a.Painter).Include(a => a.Painter.User).Include(a => a.Hall).Include(a => a.Hall != null ? a.Hall.City : null).Include(a => a.Theme).FirstOrDefaultAsync();
        }

        public async Task<List<Artwork>> SearchByCode(string code)
        {
            return await database.Artworks.Where(a => a.UniqueCode.StartsWith(code)).Include(a => a.Painter).Include(a => a.Painter.User).Include(a => a.Hall).Include(a => a.Hall != null ? a.Hall.City : null).Include(a => a.Theme).ToListAsync();
        }

        public async Task<List<Artwork>> SearchByTitle(string title)
        {
            return await database.Artworks.Where(a => a.Title.StartsWith(title)).Include(a => a.Painter).Include(a => a.Painter.User).Include(a => a.Hall).Include(a => a.Hall != null ? a.Hall.City : null).Include(a => a.Theme).ToListAsync();
        }

        public async Task<bool> UpdateArtwork(UpdateArtworkRequestDTO artwork)
        {
            try
            {
                var artworkToUpdate = await database.Artworks.Include(a => a.Painter).Include(a => a.Painter.User).
                    Include(a => a.Hall).Include(a => a.Hall != null ? a.Hall.City : null).Include(a => a.Theme)
                    .FirstOrDefaultAsync(a => a.Id == artwork.Id);

                if (artworkToUpdate == null) return false;


                if (!string.IsNullOrEmpty(artwork.UniqueCode) && artwork.UniqueCode != artworkToUpdate.UniqueCode)
                {
                    var existingArtwork = await database.Artworks
                        .FirstOrDefaultAsync(a => a.UniqueCode == artwork.UniqueCode && a.Id != artwork.Id);

                    if (existingArtwork != null) return false;

                    artworkToUpdate.UniqueCode = artwork.UniqueCode;
                }

                if (!string.IsNullOrEmpty(artwork.Dimensions)) artworkToUpdate.Dimensions = artwork.Dimensions;
                if (!string.IsNullOrEmpty(artwork.Title)) artworkToUpdate.Title = artwork.Title;

                if (artwork.ThemeId > 0 && artwork.ThemeId != artworkToUpdate.ThemeId)
                {
                    var existingArtworkInTheme = await database.Artworks
                        .Where(a => a.ThemeId == artwork.ThemeId && a.PainterId == artworkToUpdate.PainterId && a.Id != artwork.Id)
                        .FirstOrDefaultAsync();

                    if (existingArtworkInTheme != null)
                    {
                        Console.WriteLine($"Painter already has an artwork in theme ID {artwork.ThemeId}");
                        return false;
                    }

                    var theme = await database.Themes.FindAsync(artwork.ThemeId);
                    if (theme == null) return false;

                    artworkToUpdate.ThemeId = theme.Id;
                }

                if (artwork.HallId > 0 && artwork.HallId != artworkToUpdate.HallId)
                {
                    var hall = await database.Halls.FindAsync(artwork.HallId);
                    if (hall == null) return false;

                    artworkToUpdate.HallId = hall.Id;
                }

                await database.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating artwork: {ex.Message}");
                return false;
            }
        }

    }
}
