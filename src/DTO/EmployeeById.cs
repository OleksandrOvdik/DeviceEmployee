using Models;

namespace DTO;

public class EmployeeById
{
    
    public PersonEmployee Person { get; set; }
    public decimal Salary { get; set; }
   public Position Position { get; set; }
    public DateTime HireDate { get; set; }
    
}