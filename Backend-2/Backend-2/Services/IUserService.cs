using Backend_2.Exeptions;
using Backend_2.Models;
using System.Security.Claims;
using System.Web.Helpers;

namespace Backend_2.Services
{
    public interface IUserService
    {
        UserDto[] GetUser();
        Task Add(UserRegDto user);
        Task AddRole(UserRoleDto role, int id);
        UserByIdDto GetUserById(int id);
        Task DeleteUser(int id);
        Task Edit(UserRegDto user, int id);
        UserLogin[] GetUserLogin();
        bool CheckUsername(string username);
        Task AddRefreshToken(string username, string token);
        User GetUserByUserName(string username);
        Task Logout(string username);
    }
    public class UserService : IUserService
    {
        private readonly TestContext _context;
        public UserService(TestContext context)
        {
            _context = context;
        }

        public async Task Add(UserRegDto user)
        {
            await _context.User.AddAsync(new User
            {
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username,
                Password = Crypto.HashPassword(user.Password),
                RoleId = 1
            });
            await _context.SaveChangesAsync();

        }

        public async Task AddRole(UserRoleDto role, int id)
        {
            var user = _context.User.Find(id);
            if (user is null)
            {
                throw new ObjectNotFoundExeption("Такой пользователь не найден");
            }
            user.RoleId = role.RoleId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            var user = _context.User.Find(id);
            if (user is null)
            {
                throw new ObjectNotFoundExeption("Такой пользователь не найден");
            }
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(UserRegDto user, int id)
        {
            var user1 = _context.User.Find(id);
            if (user1 is null)
            {
                throw new ObjectNotFoundExeption("Такой пользователь не найден");
            }
            user1.Surname = user.Surname;
            user1.Username = user.Username;
            user1.Password = user.Password;
            user1.Name = user.Name;
            await _context.SaveChangesAsync();
        }

        public UserDto[] GetUser()
        {
            return _context.User.Select(x => new UserDto
            {
                Id = x.Id,
                Username = x.Username,
                RoleId = x.RoleId,

            }).ToArray();
        }
        public UserLogin[] GetUserLogin()
        {
            return _context.User.Select(x => new UserLogin
            {
                Username = x.Username,
                Password = x.Password,
                RoleId = x.RoleId,

            }).ToArray();
        }

        public UserByIdDto GetUserById(int id)
        {
            var user = _context.User.Find(id);
            return new UserByIdDto
            {
                Id = user.Id,
                Username = user.Username,
                RoleId = user.RoleId,
                Name = user.Name,
                Surname = user.Surname,
            };
        }

        public bool CheckUsername(string username)
        {
            var user = _context.User.Where(x => x.Username == username).ToArray();
            if (user.Length > 0)
            {
                return true;
            }
            return false;
        }

       

        public async Task AddRefreshToken(string username, string token)
        {
            var user = _context.User.FirstOrDefault(x => x.Username == username);
            user.RefreshToken = token;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _context.SaveChangesAsync();
        }
        public User GetUserByUserName(string username)
        {
            return _context.User.FirstOrDefault(x => x.Username == username);
        }

        public async Task Logout(string username)
        {
            var user = _context.User.FirstOrDefault(x => x.Username == username);
            user.RefreshToken = null;
            await _context.SaveChangesAsync();
        }
    }
}
