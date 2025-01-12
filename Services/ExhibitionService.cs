using AppBackEnd.Data;
using AppBackEnd.DTO;
using AppBackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBackEnd.Services
{
    public class ExhibitionService : IExhibitionService
    {
        private readonly DatabaseContext db;

        public ExhibitionService(DatabaseContext db)
        {
            this.db = db;            
        }

        public async Task<bool> AddPictureToExhibition(int exhibitionId, int artworkId)
        {
            try
            {

                Exhibition? exhibition = await db.Exhibitions.Where(e => e.Id == exhibitionId).FirstOrDefaultAsync();
                if (exhibition == null)
                {
                    return false;
                }
                else
                {
                    Artwork? artwork = await db.Artworks.Where(a => a.Id == artworkId).FirstOrDefaultAsync();
                    if (artwork == null)
                    {
                        return false;
                    }
                    else
                    {
                        exhibition.Artworks.Add(artwork);
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> CreateExhibition(Exhibition exhibition)
        {
            try
            {
                db.Exhibitions.Add(exhibition);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }

        public async Task<bool> DeleteExhibition(int id)
        {
            try
            {

                Exhibition? exhibition = await db.Exhibitions.Where(e => e.Id == id).FirstOrDefaultAsync();
                if (exhibition == null)
                {
                    return false;
                }
                else
                {
                    db.Exhibitions.Remove(exhibition);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        
        }

        public async Task<List<Artwork>> GetAllArtworksOfExhibition(int exhibitionId)
        {
            try
            {

                Exhibition? exhibition = await db.Exhibitions.Where(e => e.Id == exhibitionId).FirstOrDefaultAsync();
                if (exhibition == null)
                {
                    return new List<Artwork>();
                }
                {
                    List<Artwork> artworks = new List<Artwork>();
                    foreach (Artwork artwork in exhibition.Artworks)
                    {
                        Artwork? art = await db.Artworks.Where(a => a.Id == artwork.Id).Include( a => a.Hall).Include(a => a.Painter).
                            Include(a => a.Painter.User).Include(a =>a.Hall!=null? a.Hall.City:null).FirstOrDefaultAsync();
                        if (art != null)
                        {
                            artworks.Add(artwork);
                        }
                        else
                        {
                            artworks.Add(new Artwork());
                        }
                    }

                    return artworks;
                }
            }
            catch (Exception ex) { 
                Console.Write(ex.Message);
                return new List<Artwork>();

            }
        }

        public async Task<List<Exhibition>> GetAllExhibitions()
        {
            return await db.Exhibitions.Include( e => e.Visits ).Include(e => e.Artworks).ToListAsync();
        }

        public async Task<Exhibition?> GetExhibitionById(int id)
        {
            return await db.Exhibitions.Where( e => e.Id == id).Include(e => e.Visits).Include(e => e.Artworks).FirstOrDefaultAsync();
        }

        public async Task<List<Exhibition>> GetExhibitionsWithBetweenDates(DateTime? minDate, DateTime? maxDate)
        {
            try
            {
                var query = db.Exhibitions.AsQueryable();

                if (minDate.HasValue)
                {
                    query = query.Where(e => e.StartDate >= minDate.Value);
                }

                if (maxDate.HasValue)
                {
                    query = query.Where(e => e.EndDate <= maxDate.Value);
                }

                return await query
                    .Include(e => e.Visits)
                    .Include(e => e.Artworks)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Exhibition>();
            }
        }


        public async Task<List<Exhibition>> GetExhibitionsWithPlaces(string PTT)
        {
            try
            {
                if (string.IsNullOrEmpty(PTT))
                    return new List<Exhibition>();

                return await db.Exhibitions
                    .Where(e => e.Hall != null && e.Hall.City!=null && e.Hall.City.PTT == PTT)
                    .Include(e => e.Visits)
                    .Include(e => e.Artworks)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Exhibition>();
            }
        }


        public async Task<List<Exhibition>> GetExhibitionsWithTitle(string title)
        {
            try
            {
                if (string.IsNullOrEmpty(title))
                    return new List<Exhibition>();

                return await db.Exhibitions
                    .Where(e => e.Title.StartsWith(title))
                    .Include(e => e.Visits)
                    .Include(e => e.Artworks)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Exhibition>();
            }
        }


        public async Task<bool> RemovePictureFromExhibition(int exhibitionId, int artworkId)
        {
            try
            {

                Exhibition? exhibition = await db.Exhibitions.Where(e => e.Id == exhibitionId).FirstOrDefaultAsync();
                if (exhibition == null)
                {
                    return false;
                }
                else
                {
                    Artwork? artwork = exhibition.Artworks.Where( a => a.Id == artworkId).FirstOrDefault();
                    if (artwork == null)
                    {
                        return false;
                    }
                    else
                    {
                        exhibition.Artworks.Remove(artwork);
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateExhibition(UpdateExhibitionRequestDTO request)
        {
            try
            {

                Exhibition? exhibition = await db.Exhibitions.Where(e => e.Id == request.Id).FirstOrDefaultAsync();
                if (exhibition == null)
                {
                    return false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.Title) && request.Title != exhibition.Title) { exhibition.Title = request.Title; }
                    if (request.EndDate!=null && !string.IsNullOrEmpty(request.EndDate.ToString()) && request.EndDate != exhibition.EndDate) { exhibition.EndDate =(DateTime)request.EndDate; }
                    if (request.StartDate != null && !string.IsNullOrEmpty(request.StartDate.ToString()) && request.StartDate != exhibition.StartDate) { exhibition.StartDate = (DateTime)request.StartDate; }
                    if (request.HallId != null && request.HallId > 0 && request.HallId != exhibition.HallId ) { exhibition.HallId = request.HallId; }
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
    }
}
