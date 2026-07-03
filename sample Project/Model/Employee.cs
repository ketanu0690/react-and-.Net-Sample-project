namespace sample_Project.Model;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Department { get; set; } = "";
    public decimal Salary { get; set; }
    public bool IsActive { get; set; } = true;
}

public record CreateEmployeeRequest(string Name, string Email, string Department, decimal Salary);
