using System;

namespace API.Entities
{
    public class Payroll
    {
        public int Id { get; set; }
        public int PayrollID { get; set; }
        public string MemberID { get; set; }
        public string Fullname { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string TFN { get; set; }
        public string EmpStartDate { get; set; }
        public string EmpEndDate { get; set; }
        public string EmpStatus { get; set; }
        public string SuperUID { get; set; }
    }
}