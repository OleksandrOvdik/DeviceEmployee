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

    public async Task<Employee> CreateEmployee(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<List<GetAllPositionsDto>> GetAllPositions()
    {
        var positions = _context.Positions.Select(p => new GetAllPositionsDto
        {
            Id = p.Id,
            Name = p.Name,
        });
        return await positions.ToListAsync();
    }

    public async Task<List<GetAllRolesDto>> GetAllRoles()
    {
        var roles = _context.Roles.Select(r => new GetAllRolesDto
        {
            Id = r.Id,
            Name = r.Name,
        });
        return await roles.ToListAsync();
    }

    public async Task<Position> GetPositionById(int id)
    {
        return await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
    }
}