using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using MyAPI.Models;

namespace MyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public EmployeeController(EmployeeDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────────────────────
        // GET /api/Employee   →  all employees
        // ─────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var list = await _context.Employees.ToListAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────
        // GET /api/Employee/{id}   →  single employee
        // ─────────────────────────────────────────────────────────────
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            try
            {
                var emp = await _context.Employees.FindAsync(id);
                return emp is null ? NotFound() : Ok(emp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────
        // POST /api/Employee/add   →  create new employee
        // ─────────────────────────────────────────────────────────────
        [HttpPost("add")]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            if (employee is null) return BadRequest("Employee data is null.");

            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetEmployee),
                                       new { id = employee.EmployeeId }, employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────
        // PUT /api/Employee/{id}   →  full update
        // ─────────────────────────────────────────────────────────────
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.EmployeeId) return BadRequest("ID mismatch.");

            try
            {
                _context.Entry(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return !_context.Employees.Any(e => e.EmployeeId == id)
                       ? NotFound()
                       : StatusCode(500, "Concurrency error updating record.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────
        // PATCH /api/Employee/{id}   →  partial update
        // ─────────────────────────────────────────────────────────────
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchEmployee(int id, [FromBody] JsonElement body)
        {
            try
            {
                var emp = await _context.Employees.FindAsync(id);
                if (emp is null) return NotFound();

                if (body.TryGetProperty("salary", out var s)) emp.Salary = s.GetDecimal();
                if (body.TryGetProperty("name",   out var n)) emp.Name   = n.GetString();

                await _context.SaveChangesAsync();
                return Ok(emp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────
        // DELETE /api/Employee/{id}
        // ─────────────────────────────────────────────────────────────
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var emp = await _context.Employees.FindAsync(id);
                if (emp is null) return NotFound();

                _context.Employees.Remove(emp);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }
    }
}
