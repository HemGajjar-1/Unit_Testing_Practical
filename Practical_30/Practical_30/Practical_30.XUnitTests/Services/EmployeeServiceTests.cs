using Moq;
using Practical_30.Application.DTOs;
using Practical_30.Application.Interfaces;
using Practical_30.Application.Services;
using Practical_30.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical_30.XUnitTests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;

        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _employeeRepositoryMock = new Mock<IEmployeeRepository>();

            _unitOfWorkMock
                .Setup(x => x.Employees)
                .Returns(_employeeRepositoryMock.Object);

            _employeeService =
                new EmployeeService(_unitOfWorkMock.Object);
        }
        [Fact]
        public async Task GetByIdAsync_WhenEmployeeExists_ReturnsEmployeeDto()
        {
            var employee = new Employee
            {
                Id = 1,
                Name = "Hem",
                Salary = 50000,
                EmailId = "hem@gmail.com",
                DepartmentId = Department.IT,
                IsDeleted = false
            };
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(employee);


            var result = await _employeeService.GetByIdAsync(1);


            Assert.NotNull(result);
            Assert.Equal(1,result.Id);
            Assert.Equal("Hem",result.Name);
            Assert.Equal("hem@gmail.com",result.EmailId);
        }
        [Fact]
        public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNull()
        {
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Employee)null);

            var result = await _employeeService.GetByIdAsync(1);

            Assert.Null(result);
        }
        [Fact]
        public async Task GetByIdAsync_WhenEmployeeIsDeleted_ReturnsNull()
        {
            var employee = new Employee
            {
                Id = 1,
                Name = "Hem",
                IsDeleted = true
            };
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(employee);

            var result = await _employeeService.GetByIdAsync(1);

            Assert.Null(result);
        }
        [Fact]
        public async Task CreateAsync_ShouldAddEmployeeAndSaveChanges()
        {
            var dto = new EmployeeDto
            {
                Name = "Hem",
                Salary = 50000,
                DepartmentId = 1,
                EmailId = "hem@gmail.com"
            };

            var result = await _employeeService.CreateAsync(dto);

            Assert.Equal("Employee created successfully",result);

            _employeeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Employee>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task UpdateAsync_WhenEmployeeExists_ShouldUpdateEmployee()
        {
            var employee = new Employee
            {
                Id = 1,
                Name = "Old Name",
                Salary = 30000,
                DepartmentId = Department.IT,
                EmailId = "old@gmail.com",
                IsDeleted = false
            };
            var dto = new EmployeeDto
            {
                Id = 1,
                Name = "New Name",
                Salary = 50000,
                DepartmentId = 2,
                EmailId = "new@gmail.com"
            };
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(employee);

            var result = await _employeeService.UpdateAsync(dto);

            Assert.Equal("Employee updated successfully",result);
            Assert.Equal("New Name",employee.Name);
            Assert.Equal(50000,employee.Salary);
            Assert.Equal("new@gmail.com",employee.EmailId);

            _employeeRepositoryMock.Verify(x => x.Update(employee), Times.Once);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);

        }
        [Fact]
        public async Task UpdateAsync_WhenEmployeeNotFound_ReturnsNotFound()
        {
            var dto = new EmployeeDto
            {
                Id = 1
            };
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Employee)null);

            var result = await _employeeService.UpdateAsync(dto);

            Assert.Equal("Employee not found",result);
            _employeeRepositoryMock.Verify(x => x.Update(It.IsAny<Employee>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Never);
        }
        [Fact]
        public async Task UpdateAsync_WhenEmployeeIsDeleted_ReturnsNotFound()
        {
            var employee = new Employee
            {
                Id = 1,
                IsDeleted = true
            };
            var dto = new EmployeeDto
            {
                Id = 1
            };
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(employee);

            var result = await _employeeService.UpdateAsync(dto);

            Assert.Equal("Employee not found",result);
            _employeeRepositoryMock.Verify(x => x.Update(It.IsAny<Employee>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Never);
        }
        [Fact]
        public async Task DeleteAsync_WhenEmployeeExists_ShouldSoftDeleteEmployee()
        {
            var employee = new Employee
            {
                Id = 1,
                IsDeleted = false
            };
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(employee);
            var result = await _employeeService.DeleteAsync(1);

            Assert.Equal("Employee deleted successfully", result);
            Assert.True(employee.IsDeleted);

            _employeeRepositoryMock.Verify(x => x.Update(employee), Times.Once);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task DeleteAsync_WhenEmployeeNotFound_ReturnsNotFound()
        {
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Employee)null);

            var result = await _employeeService.DeleteAsync(1);

            Assert.Equal("Employee not found",result);

            _employeeRepositoryMock.Verify(x => x.Update(It.IsAny<Employee>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Never);
        }
        [Fact]
        public async Task DeleteAsync_WhenEmployeeAlreadyDeleted_ReturnsNotFound()
        {
            var employee = new Employee
            {
                Id = 1,
                IsDeleted = true
            };
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(employee);

            var result = await _employeeService.DeleteAsync(1);

            Assert.Equal("Employee not found", result);
            _employeeRepositoryMock.Verify(x => x.Update(It.IsAny<Employee>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Never);

        }
        [Fact]
        public async Task DeactivateAsync_WhenEmployeeExists_ShouldDeactivateEmployee()
        {
            var employee = new Employee
            {
                Id = 1,
                Status = true,
                IsDeleted = false
            };
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(employee);

            var result = await _employeeService.DeactivateAsync(1);

            Assert.Equal("Employee deactivated successfully",result);
            Assert.False(employee.Status);

            _employeeRepositoryMock.Verify(x => x.Update(employee), Times.Once);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
        }
    }
}
