using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrsnlMgmt.Data
{
    public class EmployeeRole
    {
        public int Id { get; set; }

        [Required, StringLength(50), Index(IsUnique = true)]
        public string Name { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

    }
}
