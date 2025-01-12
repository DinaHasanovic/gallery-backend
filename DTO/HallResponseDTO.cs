namespace AppBackEnd.DTO
{
    public class HallResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Surface { get; set; }
        public CityResponseDTO? City { get; set; }

    }
}
