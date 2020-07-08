namespace PrsnlMgmt.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PrsnlMgmt.Data.PrsnlMgmtDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PrsnlMgmtDbContext context)
        {
            var jeffrey = SeedEmployee(context, null, "001", "Jeffrey", "Wells", "Director");
            var victor = SeedEmployee(context, jeffrey, "002", "Victor", "Atkins", "Director");
            var kelli = SeedEmployee(context, jeffrey, "003", "Kelli", "Hamilton", "Director");

            SeedEmployee(context, victor, "004", "Adam", "Braun", "IT", "Support");
            SeedEmployee(context, victor, "005", "Brian", "Cruz", "Accounting");
            SeedEmployee(context, victor, "006", "Kristen", "Floyd", "Analyst", "Sales");

            SeedEmployee(context, kelli, "007", "Lois", "Martinez", "Support");
            SeedEmployee(context, kelli, "008", "Michael", "Lind", "Analyst");
            SeedEmployee(context, kelli, "009", "Eric", "Bay", "IT", "Sales");
            SeedEmployee(context, kelli, "010", "Brandon", "Young", "Accounting");

        }

        private Employee SeedEmployee(PrsnlMgmtDbContext context,
            Employee mgr, string empId, string fn, string ln,
            params string[] roleNames)
        {
            // try to locate preexisting employee with specified employee id
            var empObj = context.Employees
                .Include(nameof(Employee.Roles))
                .SingleOrDefault(e => e.EmployeeId == empId);

            if (empObj == null)
            {
                // create new emp object and add to context
                empObj = new Employee { 
                    EmployeeId = empId, 
                    Roles = new HashSet<EmployeeRole>() 
                };
                context.Employees.Add(empObj);
            }

            // update the emp record
            empObj.Manager = mgr;
            empObj.FirstName = fn;
            empObj.LastName = ln;

            // remove any roles not specified in roleNames
            var rolesToRemoveList = empObj.Roles
                .Where(r => !roleNames.Contains(r.Name))
                .ToList();

            foreach (var role in rolesToRemoveList)
                empObj.Roles.Remove(role);

            // add any roles not already added to employee
            var roleNamesToAddList = roleNames
                .Where(n => !empObj.Roles.Any(r => r.Name == n))
                .ToList();

            foreach (var roleName in roleNamesToAddList)
            {
                // use existing role if available
                var role = context.EmployeeRoles
                    .SingleOrDefault(r => r.Name == roleName);

                if (role == null)
                {
                    // create a new role
                    role = new EmployeeRole { Name = roleName };
                    context.EmployeeRoles.Add(role);
                }

                empObj.Roles.Add(role);
            }

            context.SaveChanges();

            return empObj;
        }
    }
}
