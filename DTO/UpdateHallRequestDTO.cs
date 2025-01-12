namespace AppBackEnd.DTO
{
    public class UpdateHallRequestDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public float? Surface { get; set; }
        public int? cityId { get; set; }
    }
}
