using Microsoft.AspNetCore.Mvc;
using Practical_30.Application.DTOs;
using Practical_30.Application.Interfaces;

namespace Practical_30.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpGet]
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                var employees = await _employeeService.GetAllAsync();
                return Ok(employees);
            }
            var employee = await _employeeService.GetByIdAsync(id.Value);
            if(employee == null)
            {
                return NotFound("Employee not found");
            }
            return Ok(employee);
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeDto dto)
        {
            var result = await _employeeService.CreateAsync(dto);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> Update(EmployeeDto dto)
        {
            var result = await _employeeService.UpdateAsync(dto);
            if(result == "Employees not found")
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _employeeService.DeleteAsync(id);
            if (result == "Employee not found")
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpPatch("Deactivate/{id}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _employeeService.DeactivateAsync(id);
            if(result == "Employee not found")
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
