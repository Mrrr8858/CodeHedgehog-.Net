using Backend_2.Exeptions;
using Backend_2.Models;

namespace Backend_2.Services
{
    public interface IRoleService
    {
        Role[] GetRole();
        Role GetById(int id);
        Task AddRole();
    }
    public class RoleService : IRoleService
    {
        private readonly TestContext _context;
        public RoleService(TestContext context)
        {
            _context = context;
        }
        public Role[] GetRole()
        {
            return _context.Roles.Select(x => new Role
            {
                RoleId = x.RoleId,
                Name = x.Name,

            }).ToArray();
        }

        public Role GetById(int id)
        {
            Role role = _context.Roles.Find(id);
            if(role is null)
            {
                throw new ObjectNotFoundExeption($"Роль с id = {id} не найдена");
            }

            return role;
        }

        public async Task AddRole()
        {
            await _context.Roles.AddAsync(new Role
            {
                RoleId = 0,
                Name = "student",
            });
            await _context.Roles.AddAsync(new Role
            {
                RoleId = 1,
                Name = "admin",
            });
            await _context.SaveChangesAsync();
        }
    }
}
