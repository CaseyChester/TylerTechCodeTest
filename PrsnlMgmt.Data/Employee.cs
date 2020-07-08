using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrsnlMgmt.Data
{
    public class Employee
    {
        public int Id { get; set; }

        public virtual Employee Manager { get; set; }
        public int? ManagerId { get; set; }

        [Required, StringLength(10), Index(IsUnique = true)]
        public string EmployeeId { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        public virtual ICollection<EmployeeRole> Roles { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}