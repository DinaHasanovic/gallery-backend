using AppBackEnd.Data;
using AppBackEnd.DTO;

namespace AppBackEnd.Interfaces
{
    public interface ICityService
    {
        public Task<List<City>> GetCities();
        public Task<City?> GetCityById(int id);
        public Task<City?> GetCityByPTT(string PTT);
        public Task<bool> CreateCity(City city);
        public Task<bool> UpdateCity(City city);
        public Task<bool> DeleteCityById(int id);
        public Task<bool> DeleteCityByPTT(string PTT);

    }
}
