using sample_Project.Model;

namespace sample_Project.Services;

public interface IEmployeeService
{
    List<Employee> GetActiveEmployees();
    Employee? GetById(int id);
    Employee Add(CreateEmployeeRequest request);
    bool Deactivate(int id);
    List<Employee> GetByDepartment(string name);
}
