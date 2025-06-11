using DTO;
using Models;

namespace Repository.Interfaces;

public interface IEmployeeRepository
{
    public Task<IEnumerable<Employee>> GetAllEmployees();
    
    public Task<Employee> GetEmployeeById(int id);
    
    Task<Employee> CreateEmployee(Employee employee);
    
    Task<List<GetAllPositionsDto>> GetAllPositions();
    Task<List<GetAllRolesDto>> GetAllRoles();
    
    Task<Position> GetPositionById(int id);
}