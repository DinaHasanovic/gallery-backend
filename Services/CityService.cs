using AppBackEnd.Data;
using AppBackEnd.DTO;
using AppBackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBackEnd.Services
{
    public class CityService : ICityService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IHallService _hallService;


        public CityService(DatabaseContext db,IHallService hs)
        {
            this._databaseContext = db;
            this._hallService = hs;
        }

        public async Task<bool> CreateCity(City city)
        {
            City? check = await _databaseContext.Cities.Where( c => c.PTT == city.PTT ).FirstOrDefaultAsync();
            if(check != null)
            {
                return false;
            }

            City createdCity = new()
            {
                Name = city.Name,
                PTT = city.PTT
            };
            _databaseContext.Cities.Add(createdCity);
            await _databaseContext.SaveChangesAsync();
            return true;
            

        }

        public async Task<bool> DeleteCityById(int id)
        {
            City? city = await _databaseContext.Cities.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (city == null)
            {
                return false;
            }
            else
            {
                List<Hall> halls = await _databaseContext.Halls.Where(c => c.CityId == city.Id).ToListAsync();
                foreach (Hall hall in halls)
                {
                    await _hallService.DeleteHallById(hall.Id);
                }


                _databaseContext.Cities.Remove(city);
                await _databaseContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteCityByPTT(string PTT)
        {
            City? city = await _databaseContext.Cities.Where( c => c.PTT == PTT).FirstOrDefaultAsync();
            if (city == null) {
                return false;
            }
            else
            {
                List<Hall> halls = await _databaseContext.Halls.Where(c => c.CityId == city.Id).ToListAsync();
                foreach (Hall hall in halls)
                {
                    await _hallService.DeleteHallById(hall.Id);
                }
                _databaseContext.Cities.Remove(city);
                await _databaseContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<City>> GetCities()
        {
            return await _databaseContext.Cities.ToListAsync();
        }

        public async Task<City?> GetCityById(int id)
        {
            return await _databaseContext.Cities.Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<City?> GetCityByPTT(string PTT)
        {
            return await _databaseContext.Cities.Where( c => c.PTT == PTT).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateCity(City city)
        {
            City? cityToUpdate = await _databaseContext.Cities.Where(c => c.Id == city.Id).FirstOrDefaultAsync();
            if (cityToUpdate == null)
            {
                return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(city.PTT))
                {
                    City? check = await _databaseContext.Cities.Where(c => c.PTT == city.PTT).FirstOrDefaultAsync();
                    if (check != null && check.Id != city.Id)
                    {
                        return false;
                    }
                    else
                    {
                        cityToUpdate.PTT = city.PTT;
                    }
                }
                if (!string.IsNullOrEmpty(city.Name)) cityToUpdate.Name = city.Name;
                await _databaseContext.SaveChangesAsync();
                return true;
            }
        }
    }
}
