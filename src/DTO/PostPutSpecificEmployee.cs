namespace DTO;

public class PostPutSpecificEmployee
{
    
    public PostPutEmployeeDto Person { get; set; } = null!;
    
    public Decimal Salary { get; set; }
    public int PositionId { get; set; }
}