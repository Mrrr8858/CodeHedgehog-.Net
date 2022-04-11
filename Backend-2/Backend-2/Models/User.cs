namespace Backend_2.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }

    public class UserRegDto
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
    }
    public class UserLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int RoleId { get; set; }
    }
    public class UserRoleDto
    {
        public int RoleId { get; set; }
    }
    public class UserByIdDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Surname { get; set; }
        public int RoleId { get; set; }
    }
}
