namespace AppBackEnd.DTO
{
    public class UpdateUserRequestDTO
    {
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName  { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public int?  CityId { get; set; }
        public string? JMBG { get; set; }
        public string? Agency { get; set; }
    }
}
