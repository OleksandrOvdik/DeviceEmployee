﻿using Models;

namespace DTO;

public class EmployeeByIdDto
{
    
    public PersonEmployeeDto Person { get; set; } = null!;
    public decimal Salary { get; set; }
   public string Position { get; set; } = null!;
   public DateTime HireDate { get; set; }
    
}