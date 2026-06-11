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

namespace Practical_30.NUnitTests.Services
{
    [TestFixture]
    public class EmployeeServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private EmployeeService _employeeService;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();

            _unitOfWorkMock.Setup(x => x.Employees).Returns(_employeeRepositoryMock.Object);
            _employeeService = new EmployeeService(_unitOfWorkMock.Object);
        }
        [Test]
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

         
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Hem"));
            Assert.That(result.EmailId, Is.EqualTo("hem@gmail.com"));
        }
        [Test]
        public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNull()
        {
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Employee)null);

            var result = await _employeeService.GetByIdAsync(1);

            Assert.That(result, Is.Null);
        }
        [Test]
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

            Assert.That(result, Is.Null);
        }
        [Test]
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

            Assert.That(result, Is.EqualTo("Employee created successfully"));

            _employeeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Employee>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
        }
        [Test]
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

            Assert.That(result, Is.EqualTo("Employee updated successfully"));
            Assert.That(employee.Name, Is.EqualTo("New Name"));
            Assert.That(employee.Salary, Is.EqualTo(50000));
            Assert.That(employee.EmailId, Is.EqualTo("new@gmail.com"));

            _employeeRepositoryMock.Verify(x => x.Update(employee), Times.Once);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
            
        }
        [Test]
        public async Task UpdateAsync_WhenEmployeeNotFound_ReturnsNotFound()
        {
            var dto = new EmployeeDto
            {
                Id = 1
            };
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Employee)null);

            var result = await _employeeService.UpdateAsync(dto);

            Assert.That(result, Is.EqualTo("Employee not found"));
            _employeeRepositoryMock.Verify(x => x.Update(It.IsAny<Employee>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Never);
        }
        [Test]
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

            Assert.That(result, Is.EqualTo("Employee not found"));
            _employeeRepositoryMock.Verify(x => x.Update(It.IsAny<Employee>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Never);
        }
        [Test]
        public async Task DeleteAsync_WhenEmployeeExists_ShouldSoftDeleteEmployee()
        {
            var employee = new Employee
            {
                Id = 1,
                IsDeleted = false
            };
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(employee);
            var result = await _employeeService.DeleteAsync(1);

            Assert.That(result, Is.EqualTo("Employee deleted successfully"));
            Assert.That(employee.IsDeleted, Is.True);

            _employeeRepositoryMock.Verify(x => x.Update(employee), Times.Once);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
        }
        [Test]
        public async Task DeleteAsync_WhenEmployeeNotFound_ReturnsNotFound()
        {
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Employee)null);

            var result = await _employeeService.DeleteAsync(1);

            Assert.That(result, Is.EqualTo("Employee not found"));

            _employeeRepositoryMock.Verify(x => x.Update(It.IsAny<Employee>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Never);
        }
        [Test]
        public async Task DeleteAsync_WhenEmployeeAlreadyDeleted_ReturnsNotFound()
        {
            var employee = new Employee
            {
                Id = 1,
                IsDeleted = true
            };
            _employeeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(employee);

            var result = await _employeeService.DeleteAsync(1);

            Assert.That(result, Is.EqualTo("Employee not found"));
            _employeeRepositoryMock.Verify(x => x.Update(It.IsAny<Employee>()),Times.Never);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(),Times.Never);

        }
        [Test]
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

            Assert.That(result, Is.EqualTo("Employee deactivated successfully"));
            Assert.That(employee.Status, Is.False);

            _employeeRepositoryMock.Verify(x => x.Update(employee), Times.Once);
            _unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
        }
    }
}
