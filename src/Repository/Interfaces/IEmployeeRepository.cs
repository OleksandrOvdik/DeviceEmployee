using DTO;
using Models;

namespace Repository.Interfaces;

public interface IEmployeeRepository
{
    public Task<IEnumerable<Employee>> GetAllEmployees();
    
    public Task<Employee> GetEmployeeById(int id);
    
    Task<List<Position>> GetAllPositions();
    Task<List<Role>> GetAllRoles();
}