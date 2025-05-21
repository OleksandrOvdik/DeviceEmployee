using DTO;

namespace Services.Interfaces;

public interface IEmployeeService
{

    public Task<List<EmployeeDto>> GetAllEmployees();
    
    public Task<EmployeeDto> GetEmployeeById(int id);


}