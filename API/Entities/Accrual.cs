using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Accrual
    {
        public int Id { get; set; }
        public string MemberID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string SuperFund { get; set; }
        public string USI { get; set; }
        [DataType("decimal(18 ,2)")]
        public decimal SuperGuarantee { get; set; }
        public double Spouse { get; set; }
        public double SalarySacrifice { get; set; }
        public double Redundancy { get; set; }
        public double Productivity { get; set; }
        public double EmployerAdditional { get; set; }
        public double EmployeeAdditional { get; set; }
    }
}