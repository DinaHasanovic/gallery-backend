using AppBackEnd.Data;

namespace AppBackEnd.Interfaces
{
    public interface IHallService
    {
        public Task<List<Hall>> GetAllHalls();
        public Task<Hall?> GetHallById(int id);
        public Task<bool> CreateHall(Hall hall);
        public Task<bool> UpdateHall(Hall hall);
        public Task<bool> DeleteHallById(int id);
        public Task<List<Hall>> GetHallsByCityId(int id);
        public Task<List<Hall>> GetHallByNameSearch(string name);
      }
}
