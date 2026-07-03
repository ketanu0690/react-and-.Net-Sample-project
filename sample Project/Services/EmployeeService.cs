using System.Text.Json;
using sample_Project.Model;

namespace sample_Project.Services;

public class EmployeeService : IEmployeeService
{
    private readonly string _filePath;
    private readonly object _lock = new();
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public EmployeeService(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "Data", "employees.json");
    }

    private List<Employee> Load()
    {
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Employee>>(json, JsonOptions) ?? [];
    }

    private void Save(List<Employee> employees)
    {
        var json = JsonSerializer.Serialize(employees, JsonOptions);
        File.WriteAllText(_filePath, json);
    }

    public List<Employee> GetActiveEmployees() => Load().Where(e => e.IsActive).ToList();

    public Employee? GetById(int id) => Load().FirstOrDefault(e => e.Id == id);

    public Employee Add(CreateEmployeeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Department))
            throw new ArgumentException("Name, email, and department are required.");

        lock (_lock)
        {
            var employees = Load();
            if (employees.Any(e => e.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Email already exists.");

            var employee = new Employee
            {
                Id = employees.Count > 0 ? employees.Max(e => e.Id) + 1 : 1,
                Name = request.Name.Trim(),
                Email = request.Email.Trim(),
                Department = request.Department.Trim(),
                Salary = request.Salary,
                IsActive = true
            };
            employees.Add(employee);
            Save(employees);
            return employee;
        }
    }

    public bool Deactivate(int id)
    {
        lock (_lock)
        {
            var employees = Load();
            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee is null) return false;
            employee.IsActive = false;
            Save(employees);
            return true;
        }
    }

    public List<Employee> GetByDepartment(string name) =>
        Load().Where(e => e.Department.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
}
