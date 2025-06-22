using Microsoft.AspNetCore.Mvc;
using MyAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly EmployeeDbContext _context;

    public EmployeeController(EmployeeDbContext context)
    {
        _context = context;
    }

    // ✅ GET all employees
    [HttpGet]
    public ActionResult<List<Employee>> GetEmployees()
    {
        return _context.Employees.ToList();
    }

    // ✅ GET single employee by ID
    [HttpGet("{id}")]
    public ActionResult<Employee> GetEmployee(int id)
    {
        var emp = _context.Employees.Find(id);
        if (emp == null) return NotFound();
        return emp;
    }

    // ✅ POST new employee
    [HttpPost("add")]
    public ActionResult<Employee> AddEmployees(Employee employee)
    {
        if (employee == null)
            return BadRequest("Employee data is null.");

        _context.Employees.Add(employee);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
    }

    // ✅ PUT - Update full employee
    [HttpPut("{id}")]
    public IActionResult UpdateEmployee(int id, Employee employee)
    {
        if (id != employee.EmployeeId)
            return BadRequest("ID mismatch.");

        _context.Entry(employee).State = EntityState.Modified;

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Employees.Any(e => e.EmployeeId == id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // ✅ PATCH - Partial update (e.g., only salary or name)
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchEmployee(int id, [FromBody] JsonElement patchData)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        // Example: Allow patching "salary" and "name"
        if (patchData.TryGetProperty("salary", out var salaryProp))
            employee.Salary = salaryProp.GetDecimal();

        if (patchData.TryGetProperty("name", out var nameProp))
            employee.Name = nameProp.GetString();

        await _context.SaveChangesAsync();
        return Ok(employee);
    }

    // ✅ DELETE employee
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return NotFound();

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
