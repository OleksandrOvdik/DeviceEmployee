using DTO;
using Repository;
using Services.Interfaces;

namespace Services;

public class EmployeeService : IEmployeeService
{
    
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }
    

    public async Task<List<EmployeeDto>> GetAllEmployees()
    {
        
        var employees = await _employeeRepository.GetAllEmployees();
        var obj = new List<EmployeeDto>();

        foreach (var employee in employees)
        {
            obj.Add(new EmployeeDto()
            {
                Id = employee.Id,
                Name = $"{employee.Person.FirstName} {employee.Person.MiddleName} {employee.Person.LastName}"
            });
        }
        
        return obj;
        
    }

    public Task<EmployeeDto> GetEmployeeById(int id)
    {
        throw new NotImplementedException();
    }
}