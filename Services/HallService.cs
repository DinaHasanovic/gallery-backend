using AppBackEnd.Data;
using AppBackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AppBackEnd.Services
{
    public class HallService : IHallService
    {
        private readonly DatabaseContext database;
        private readonly IExhibitionService exhibitionService;
        public HallService(DatabaseContext db,IExhibitionService ex)
        {
            this.database = db;
            this.exhibitionService = ex;
        }

        public async Task<bool> CreateHall(Hall hall)
        {
            database.Halls.Add(hall);
            await database.SaveChangesAsync();
            return true;
        
        }

        public async Task<bool> DeleteHallById(int id)
        {
           Hall? hall = await database.Halls.Where( h =>  h.Id == id).FirstOrDefaultAsync();
            if (hall == null) {
                return false;
            }
            else
            {
                List<Exhibition> exhibitions = await database.Exhibitions.Where( e => e.HallId == id).ToListAsync();
                foreach (Exhibition e in exhibitions)
                {
                    await exhibitionService.DeleteExhibition(e.Id);
                }

                database.Halls.Remove(hall);
                await database.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<Hall>> GetAllHalls()
        {
            return await database.Halls.Include( h => h.City).ToListAsync();
        }

        public async Task<Hall?> GetHallById(int id)
        {
            return await database.Halls.Where(h => h.Id == id).Include( h => h.City).FirstOrDefaultAsync();
        }

        public async  Task<List<Hall>> GetHallByNameSearch(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<Hall>();
            }

            return await database.Halls
                .Where(h => h.Name.StartsWith(name))
                .Include(h => h.City)
                .ToListAsync();
        }

        public async Task<List<Hall>> GetHallsByCityId(int id)
        {
            return await database.Halls.Where(h => h.CityId == id).Include(h => h.City).ToListAsync();
        }

        public async Task<bool> UpdateHall(Hall hall)
        {
            Hall? hallToUpdate = await database.Halls.Where(h => h.Id == hall.Id).FirstOrDefaultAsync();

            if (hallToUpdate == null) { return false; }
            else
            {
                if (!string.IsNullOrWhiteSpace(hall.Name))
                {
                    hallToUpdate.Name = hall.Name;
                }
                if (hall.Surface > 0 && !float.IsNaN(hall.Surface))
                {
                    hallToUpdate.Surface =(float) Math.Round(hall.Surface,2);
                }

                if (hall.CityId.HasValue) 
                {
                    City? city = await database.Cities.Where(c => c.Id == hall.CityId).FirstOrDefaultAsync();
                    if (city == null) return false;
                    else
                    {
                    hallToUpdate.CityId = hall.CityId.Value;
                    }
                }
                await database.SaveChangesAsync();
                return true;
            }

        }
    }
}
