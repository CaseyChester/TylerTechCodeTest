using PrsnlMgmt.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PrsnlMgmt.Services.DataService
{
    /// <summary>
    /// Provides personnel management data services.
    /// </summary>
    public class PrsnlMgmtDataService
    {
        private readonly Func<PrsnlMgmtDbContext> _createContext;

        public PrsnlMgmtDataService(Func<PrsnlMgmtDbContext> createContext)
        {
            _createContext = createContext;
        }

        public async Task<List<EmployeeRole>> GetAllEmployeeRolesAsync()
        {
            using (var ctx = _createContext())
            {
                return await ctx.EmployeeRoles.AsNoTracking().ToListAsync();
            }
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            using (var ctx = _createContext())
            {
                return await ctx.Employees.AsNoTracking().ToListAsync();
            }
        }

        public async Task<List<Employee>> GetAllManagersAsync()
        {
            using (var ctx = _createContext())
            {
                return await ctx.Employees
                    .AsNoTracking()
                    .Where(emp => emp.Manager != null)
                    .Select(emp => emp.Manager)
                    .Distinct()
                    .ToListAsync();
            }
        }

        public async Task<List<Employee>> GetEmployeesByManagerIdAsync(int mgrId)
        {
            using (var ctx = _createContext())
            {
                return await ctx.Employees
                    .AsNoTracking()
                    .Where(emp => emp.Manager.Id == mgrId)
                    .ToListAsync();
            }
        }

        public Employee GetEmployeeByEmployeeId(string employeeId)
        {
            using (var ctx = _createContext())
            {
                return ctx.Employees
                    .AsNoTracking()
                    .SingleOrDefault(emp => emp.EmployeeId == employeeId);
            }
        }

        public async Task<List<Employee>> GetUnmanagedEmployeesAsync()
        {
            using (var ctx = _createContext())
            {
                return await ctx.Employees
                    .AsNoTracking()
                    .Where(emp => emp.Manager == null)
                    .ToListAsync();
            }
        }

        public async Task InsertEmployeeAsync(Employee newEmp)
        {
            using (var ctx = _createContext())
            {
                // attach the manager to the context if present
                if (newEmp.Manager != null && newEmp.Manager.Id > 0)
                    ctx.Employees.Attach(newEmp.Manager);

                // attach any roles to the context
                foreach(var role in newEmp.Roles)
                {
                    if (role.Id > 0) 
                        ctx.EmployeeRoles.Attach(role);
                }
                var emp = ctx.Employees.Add(newEmp);
                await ctx.SaveChangesAsync();
            }
        }
    }
}