using DTO;
using Microsoft.EntityFrameworkCore;
using Models;
using Repository.Interfaces;

namespace Repository;

public class EmployeeRepository : IEmployeeRepository
{
    
    private readonly MasterContext _context;

    public EmployeeRepository(MasterContext context)
    {
        _context = context;
    }
    
    

    public async Task<IEnumerable<Employee>> GetAllEmployees()
    {

        return await _context.Employees.
            Include(p => p.Person)
            .ToListAsync();

    }

    public async Task<Employee> GetEmployeeById(int id)
    {
        
        return (await _context.Employees.Include(p => p.Person).
            Include(ps => ps.Position).
            FirstOrDefaultAsync(e => e.Id == id))!;
        
    }

    public async Task<List<Position>> GetAllPositions()
    {
        var positions = _context.Positions.Select(p => new Position
        {
            Id = p.Id,
            Name = p.Name,
        });
        return await positions.ToListAsync();
    }

    public async Task<List<Role>> GetAllRoles()
    {
        var roles = _context.Roles.Select(r => new Role
        {
            Id = r.Id,
            Name = r.Name,
        });
        return await roles.ToListAsync();
    }
}