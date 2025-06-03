using System.ComponentModel.DataAnnotations;

namespace DTO.Accounts;

public class CreateAccountDto
{
    
    // Username shouldn’t start with numbers.
    // Password should have length at least 12, and have at least one small letter, one capital letter, one number and one symbol.
    
    [Required]
    [MaxLength(60)]
    [RegularExpression("^[A-Za-z][A-Za-z0-9]{3,}$")]
    public string Username { get; set; }
    
    [Required]
    [MaxLength(32)]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^A-Za-z0-9]).{12,}$")]
    public string Password { get; set; }
    
    public string EmployeeName { get; set; }
    public string RoleName { get; set; }
    
}