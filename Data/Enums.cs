namespace AppBackEnd.Data
{
    public enum UserRoles //definiše pet različitih uloga korisnika u aplikaciji.
    {
        Admin = 4, //admin
        Painter = 3, //slikar
        JuryMember = 2, //clanzirija
        Journalist = 1,//novinar
        Visitor = 0 //posetilac
    }
}
