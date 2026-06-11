using Practical_30.Application.Interfaces;
using Practical_30.Infrastructure.Data;
using Practical_30.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical_30.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IEmployeeRepository Employees { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Employees = new EmployeeRepository(_context);
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
