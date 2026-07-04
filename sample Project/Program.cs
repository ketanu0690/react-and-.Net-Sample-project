var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IEmployeeService, EmployeeService>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("http://localhost:5173", "http://localhost:5174").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.UseHttpsRedirection();

app.MapGet("/api/employees", (IEmployeeService service) => service.GetActiveEmployees());

app.MapGet("/api/employees/{id:int}", (int id, IEmployeeService service) =>
    service.GetById(id) is { } employee ? Results.Ok(employee) : Results.NotFound());

app.MapPost("/api/employees", (CreateEmployeeRequest request, IEmployeeService service) =>
{
    try
    {
        var employee = service.Add(request);
        return Results.Created($"/api/employees/{employee.Id}", employee);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(ex.Message);
    }
});

app.MapPut("/api/employees/{id:int}/deactivate", (int id, IEmployeeService service) =>
    service.Deactivate(id) ? Results.NoContent() : Results.NotFound());

app.MapGet("/api/departments/{name}/employees", (string name, IEmployeeService service) =>
    service.GetByDepartment(name));

app.Run();
