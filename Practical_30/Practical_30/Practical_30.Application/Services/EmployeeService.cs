using Practical_30.Application.DTOs;
using Practical_30.Application.Interfaces;
using Practical_30.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical_30.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _unitOfWork.Employees.GetActiveEmployeesAsync();
            return employees.Select(e=> new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Salary = e.Salary,
                DepartmentId = (int)e.DepartmentId,
                EmailId = e.EmailId
            });
        }
        public async Task<EmployeeDto> GetByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);    
            if(employee == null || employee.IsDeleted)
            {
                return null;
            }
            return new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Salary = employee.Salary,
                DepartmentId = (int)employee.DepartmentId,
                EmailId = employee.EmailId
            };
        }
        public async Task<string> CreateAsync(EmployeeDto dto)
        {
            var employee = new Employee
            {
                Name = dto.Name,
                Salary = dto.Salary,
                DepartmentId = (Department)dto.DepartmentId,
                EmailId = dto.EmailId,
                JoiningDate = DateTime.Now,
                Status = true,
                IsDeleted = false
            };
            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.CompleteAsync();
            return "Employee created successfully";
        }
        public async Task<string> UpdateAsync(EmployeeDto dto)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(dto.Id);
            if(employee == null || employee.IsDeleted)
            {
                return "Employee not found";
            }
            employee.Name = dto.Name;
            employee.Salary = dto.Salary;
            employee.DepartmentId = (Department)dto.DepartmentId;
            employee.EmailId = dto.EmailId;
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.CompleteAsync();
            return "Employee updated successfully";
        }
        public async Task<string> DeleteAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if(employee == null || employee.IsDeleted)
            {
                return "Employee not found";
            }
            employee.IsDeleted = true;
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.CompleteAsync();
            return "Employee deleted successfully";
        }
        public async Task<string> DeactivateAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if(employee == null || employee.IsDeleted)
            {
                return "Employee not found";
            }
            employee.Status = false;
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.CompleteAsync();
            return "Employee deactivated successfully";
        }
    }
}
