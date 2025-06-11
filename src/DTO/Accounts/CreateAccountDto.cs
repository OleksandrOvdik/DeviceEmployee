using System.ComponentModel.DataAnnotations;

namespace DTO.Accounts;

public class CreateAccountDto
{
    
    [Required]
    [MaxLength(60)]
    [RegularExpression("^[A-Za-z][A-Za-z0-9]{3,}$")]
    public string Username { get; set; }
    
    [Required]
    [MaxLength(32)]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^A-Za-z0-9]).{12,}$")]
    public string Password { get; set; }
    
    public int EmployeeId { get; set; }
    public int RoleId { get; set; }
    
}