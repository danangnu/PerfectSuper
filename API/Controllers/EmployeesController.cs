using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class EmployeesController : BaseApiController
    {
        private readonly DataContext _context;
        public EmployeesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employees>>> GetEmployees()
        {
            return await _context.tblEmployees.ToListAsync();
        }

        // api/accrual/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Employees>> GetEmployee(int id)
        {
            return await _context.tblEmployees.FindAsync(id);
        }

        [HttpPost("addcsv")]
        public async Task<IActionResult> SeedEmployee(IFormFile file)
        {

            using (var sreader = new StreamReader(file.OpenReadStream()))
            {
                string[] strlines = sreader.ReadToEnd().Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.None
                    );
                if (strlines[0].Contains("Allied Recruitment"))
                {
                    if (await _context.tblAccrual.AnyAsync())
                    {
                        using (var connection = _context.Database.GetDbConnection())
                        {
                            await connection.OpenAsync();
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "DELETE FROM [tblAccrual]";//"DELETE FROM [tblAccrual]";
                                var result = await command.ExecuteNonQueryAsync();
                                command.CommandText = "delete from sqlite_sequence where name='tblAccrual';";
                                //command.CommandText = "ALTER TABLE tblAccrual AUTO_INCREMENT = 1";
                                result = await command.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    var SuperFund = string.Empty;
                    var EmployerMemberNo = string.Empty;
                    var EmployerMember = string.Empty;
        
                    Regex patdate = new Regex(@"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$");
                    if (patdate.IsMatch(strlines[4]))
                    {
                        var entity = await _context.tblDataRecords.FirstOrDefaultAsync(item => item.FieldName == "Accrual");
                        if (entity != null)
                        {
                            //  entity.Desc = strlines[4];       
                            _context.SaveChanges();
                        }
                    }
                    for (int x = 1; x < strlines.Length-1; x++)
                    {
                        if (strlines[x].Contains("Superannuation Fund") || strlines[x].Contains("Employee Membership No"))
                        {
                            int i = x;                       
                            if (strlines[x].Contains("Superannuation Fund"))
                            {
                                SuperFund = strlines[x].Substring(strlines[x].LastIndexOf(',') + 1);                           
                                do
                                {
                                    i += 1;
                                    EmployerMember = strlines[i].TrimEnd();
                                    EmployerMember = strlines[i].TrimStart();
                                }
                                while (!EmployerMember.Contains("Employer Membership No"));
                                EmployerMemberNo = EmployerMember.Substring(EmployerMember.LastIndexOf(',') + 1);                            
                                do
                                {
                                    i += 1;
                                }
                                while (!strlines[i].Contains("Employee Membership No"));
                            }
                            var EmployeeMember = string.Empty;
                            EmployeeMember = strlines[i].TrimEnd();
                            EmployeeMember = strlines[i].TrimStart();
                            Regex regex = new Regex("\"(.*?)\"");
                            var matches = regex.Matches(EmployeeMember);
                            var EmployeeName = string.Empty;
                            var Lastname = string.Empty;
                            var Firstname = string.Empty;
                            if (matches.Count > 0)
                            {
                                EmployeeName = matches[0].Groups[1].ToString();
                                string[] words = EmployeeName.Split(',');
                                Lastname = words[0].TrimEnd();
                                Lastname = Lastname.TrimStart();
                                Firstname = words[1].TrimStart();
                                Firstname = Firstname.TrimEnd();
                            }
                            var EmployeeMemberNo = EmployeeMember.Substring(EmployeeMember.LastIndexOf(',') + 1);                        
                            var Spouse = string.Empty;
                            do
                            {
                                i += 1;
                                Spouse = strlines[i].TrimEnd();
                                Spouse = strlines[i].TrimStart();
                            }
                            while (!Spouse.Contains("Superannuation -"));

                            //Spouse
                            int y = i;
                            var Spouses = string.Empty;
                            do
                            {
                                y = y - 1;
                                Spouses = strlines[y].TrimEnd();
                                Spouses = strlines[y].TrimStart();
                            }
                            while (!(Spouses.Contains(EmployeeName)) && !(Spouses.Contains("Spouse")));
                            var SpouseTot = string.Empty;
                            if (Spouses.Contains("Spouse"))
                            {
                                matches = regex.Matches(Spouses);
                                if (matches.Count > 0)
                                {
                                    SpouseTot = matches[0].Groups[1].ToString().Substring(matches[0].Groups[1].ToString().LastIndexOf('$') + 1);
                                    if (SpouseTot.IndexOf(',') > 0)
                                        SpouseTot = SpouseTot.Remove(SpouseTot.IndexOf(','), 1);
                                }
                                else
                                {
                                    SpouseTot = Spouses.Substring(Spouses.LastIndexOf('$') + 1);
                                    if (SpouseTot.IndexOf(',') > 0)
                                        SpouseTot = SpouseTot.Remove(SpouseTot.IndexOf(','), 1);
                                    do
                                    {
                                        i += 1;
                                    }
                                    while (!strlines[i].Contains("Superannuation -"));
                                }
                            }

                            //Salary Sacrifice
                            y = i;
                            var Others = string.Empty;
                            do
                            {
                                y = y - 1;
                                Others = strlines[y].TrimEnd();
                                Others = strlines[y].TrimStart();
                            }
                            while (!(Others.Contains(EmployeeName)) && !(Others.Contains("Salary Sacrifice")));
                            var SacrificeTot = string.Empty;
                            if (Others.Contains("Salary Sacrifice"))
                            {
                                matches = regex.Matches(Others);
                                if (matches.Count > 0)
                                {
                                    SacrificeTot = matches[0].Groups[1].ToString().Substring(matches[0].Groups[1].ToString().LastIndexOf('$') + 1);
                                    if (SacrificeTot.IndexOf(',') > 0)
                                        SacrificeTot = SacrificeTot.Remove(SacrificeTot.IndexOf(','), 1);
                                }
                                else
                                {
                                    SacrificeTot = Others.Substring(Others.LastIndexOf('$') + 1);
                                    if (SacrificeTot.IndexOf(',') > 0)
                                    SacrificeTot = SacrificeTot.Remove(SacrificeTot.IndexOf(','), 1);
                                    do
                                    {
                                        i += 1;
                                    }
                                    while (!strlines[i].Contains("Superannuation -"));
                                }
                            }

                            //Redundancy
                            y = i;
                            var Redundancy = string.Empty;
                            do
                            {
                                y = y - 1;
                                Redundancy = strlines[y].TrimEnd();
                                Redundancy = Redundancy.TrimStart();
                            }
                            while (!(Redundancy.Contains(EmployeeName)) && !(Redundancy.Contains("Redundancy")));
                            var RedundancyTot = string.Empty;
                            if (Redundancy.Contains("Redundancy"))
                            {
                                matches = regex.Matches(Redundancy);
                                if (matches.Count > 0)
                                {
                                    RedundancyTot = matches[0].Groups[1].ToString().Substring(matches[0].Groups[1].ToString().LastIndexOf('$') + 1);
                                    if (RedundancyTot.IndexOf(',') > 0)
                                        RedundancyTot = RedundancyTot.Remove(RedundancyTot.IndexOf(','), 1);
                                }
                                else
                                {
                                    RedundancyTot = Redundancy.Substring(Redundancy.LastIndexOf('$') + 1);
                                    if (RedundancyTot.IndexOf(',') > 0)
                                        RedundancyTot = RedundancyTot.Remove(RedundancyTot.IndexOf(','), 1);
                                    do
                                    {
                                        i += 1;
                                    }
                                    while (!strlines[i].Contains("Superannuation -"));
                                }
                            }

                            //Productivity
                            y = i;
                            var Productivity = string.Empty;
                            do
                            {
                                y = y - 1;
                                Productivity = strlines[y].TrimEnd();
                                Productivity = Productivity.TrimStart();
                            }
                            while (!(Productivity.Contains(EmployeeName)) && !(Productivity.Contains("Productivity")));
                            var ProductivityTot = string.Empty;
                            if (Productivity.Contains("Productivity"))
                            {
                                matches = regex.Matches(Productivity);
                                if (matches.Count > 0)
                                {
                                    ProductivityTot = matches[0].Groups[1].ToString().Substring(matches[0].Groups[1].ToString().LastIndexOf('$') + 1);
                                    if (ProductivityTot.IndexOf(',') > 0)
                                        ProductivityTot = ProductivityTot.Remove(ProductivityTot.IndexOf(','), 1);
                                }
                                else
                                {
                                    ProductivityTot = Productivity.Substring(Productivity.LastIndexOf('$') + 1);
                                    if (ProductivityTot.IndexOf(',') > 0)
                                        ProductivityTot = ProductivityTot.Remove(ProductivityTot.IndexOf(','), 1);
                                    do
                                    {
                                        i += 1;
                                    }
                                    while (!strlines[i].Contains("Superannuation -"));
                                }
                            }

                            //Employer Additional
                            y = i;
                            var EmployerAdditional = string.Empty;
                            do
                            {
                                y = y - 1;
                                EmployerAdditional = strlines[y].TrimEnd();
                                EmployerAdditional = EmployerAdditional.TrimStart();
                            }
                            while (!(EmployerAdditional.Contains(EmployeeName)) && !(EmployerAdditional.Contains("Employer Additional")));
                            var EmployerAdditionalTot = string.Empty;
                            if (EmployerAdditional.Contains("Employer Additional"))
                            {
                                matches = regex.Matches(EmployerAdditional);
                                if (matches.Count > 0)
                                {
                                    EmployerAdditionalTot = matches[0].Groups[1].ToString().Substring(matches[0].Groups[1].ToString().LastIndexOf('$') + 1);
                                    if (EmployerAdditionalTot.IndexOf(',') > 0)
                                        EmployerAdditionalTot = EmployerAdditionalTot.Remove(EmployerAdditionalTot.IndexOf(','), 1);
                                }
                                else
                                {
                                    EmployerAdditionalTot = EmployerAdditional.Substring(EmployerAdditional.LastIndexOf('$') + 1);
                                    if (EmployerAdditionalTot.IndexOf(',') > 0)
                                        EmployerAdditionalTot = EmployerAdditionalTot.Remove(EmployerAdditionalTot.IndexOf(','), 1);
                                    do
                                    {
                                        i += 1;
                                    }
                                    while (!strlines[i].Contains("Superannuation -"));
                                }
                            }

                            //EmployeeAdditional
                            y = i;
                            var EmployeeAdditional = string.Empty;
                            do
                            {
                                y = y - 1;
                                EmployeeAdditional = strlines[y].TrimEnd();
                                EmployeeAdditional = EmployeeAdditional.TrimStart();
                            }
                            while (!(EmployeeAdditional.Contains(EmployeeName)) && !(EmployeeAdditional.Contains("Employee Additional")));
                            var EmployeeAdditionalTot = string.Empty;
                            if (EmployeeAdditional.Contains("Employee Additional"))
                            {
                                matches = regex.Matches(EmployeeAdditional);
                                if (matches.Count > 0)
                                {
                                    EmployeeAdditionalTot = matches[0].Groups[1].ToString().Substring(matches[0].Groups[1].ToString().LastIndexOf('$') + 1);
                                    if (EmployeeAdditionalTot.IndexOf(',') > 0)
                                        EmployeeAdditionalTot = EmployeeAdditionalTot.Remove(EmployeeAdditionalTot.IndexOf(','), 1);
                                }
                                else
                                {
                                    EmployeeAdditionalTot = EmployeeAdditional.Substring(EmployeeAdditional.LastIndexOf('$') + 1);
                                    if (EmployeeAdditionalTot.IndexOf(',') > 0)
                                        EmployeeAdditionalTot = EmployeeAdditionalTot.Remove(EmployeeAdditionalTot.IndexOf(','), 1);
                                    do
                                    {
                                        i += 1;
                                    }
                                    while (!strlines[i].Contains("Superannuation -"));
                                }
                            }

                            var SuperGuaranteeTot = string.Empty;
                            if (Spouse.Contains("Superannuation -"))
                            {
                                matches = regex.Matches(Spouse);
                                if (matches.Count > 0)
                                {
                                    SuperGuaranteeTot = matches[0].Groups[1].ToString().Substring(matches[0].Groups[1].ToString().LastIndexOf('$') + 1);
                                    if (SuperGuaranteeTot.IndexOf(',') > 0)
                                        SuperGuaranteeTot = SuperGuaranteeTot.Remove(SuperGuaranteeTot.IndexOf(','), 1);                               
                                }
                                else
                                {
                                    SuperGuaranteeTot = Spouse.Substring(Spouse.LastIndexOf('$') + 1);
                                    if (SuperGuaranteeTot.IndexOf(',') > 0)
                                        SuperGuaranteeTot = SuperGuaranteeTot.Remove(SuperGuaranteeTot.IndexOf(','), 1);                               
                                }
                            }

                            var accrual = new Accrual();
                            accrual.MemberID = EmployeeMemberNo;
                            accrual.LastName = Lastname;
                            accrual.FirstName = Firstname;
                            accrual.SuperFund = SuperFund;
                            accrual.USI = EmployerMemberNo;
                            accrual.SuperGuarantee = Decimal.Parse(SuperGuaranteeTot);
                            if (string.IsNullOrEmpty(SpouseTot))
                                accrual.Spouse = 0.00;
                            else
                                accrual.Spouse = Math.Round(Double.Parse(SpouseTot),2);
                            if (string.IsNullOrEmpty(SacrificeTot))
                                accrual.SalarySacrifice = 0.00;
                            else
                                accrual.SalarySacrifice = Math.Round(Double.Parse(SacrificeTot),2);
                            if (string.IsNullOrEmpty(RedundancyTot))
                                accrual.Redundancy = 0.00;
                            else
                                accrual.Redundancy = Math.Round(Double.Parse(RedundancyTot),2);
                            if (string.IsNullOrEmpty(ProductivityTot))
                                accrual.Productivity = 0.00;
                            else
                                accrual.Productivity = Math.Round(Double.Parse(ProductivityTot),2);
                            if (string.IsNullOrEmpty(EmployerAdditionalTot))
                                accrual.EmployerAdditional = 0.00;
                            else
                                accrual.EmployerAdditional = Math.Round(Double.Parse(EmployerAdditionalTot),2);
                            if (string.IsNullOrEmpty(EmployeeAdditionalTot))
                                accrual.EmployeeAdditional = 0.00;
                            else
                                accrual.EmployeeAdditional = Math.Round(Double.Parse(EmployeeAdditionalTot),2);
                            _context.tblAccrual.Add(accrual);
                            x = i;
                        }
                    }
                } else {
                    if (await _context.tblEmployees.AnyAsync())
                    {
                        using (var connection = _context.Database.GetDbConnection())
                        {
                            await connection.OpenAsync();
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "DELETE FROM tblEmployees";//"DELETE FROM [tblEmployees]";
                                var result = await command.ExecuteNonQueryAsync();
                                //command.CommandText = "ALTER TABLE tblEmployees AUTO_INCREMENT = 1";
                                command.CommandText = "delete from sqlite_sequence where name='tblEmployees';";
                                result = await command.ExecuteNonQueryAsync();
                                //command.CommandText = "DELETE FROM tblPayroll";
                                command.CommandText = "DELETE FROM [tblPayroll]";
                                result = await command.ExecuteNonQueryAsync();
                                //command.CommandText = "ALTER TABLE tblPayroll AUTO_INCREMENT = 1";
                                command.CommandText = "delete from sqlite_sequence where name='tblPayroll';";
                                result = await command.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    foreach (var line in strlines)
                    {
                        DateTime? sdate = new DateTime();
                    sdate = Convert.ToDateTime("1971-01-01 00:00:00");

                    
                    if (line == "{}")             
                    {
                        continue;   
                    }

                    string[] data = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);
                        Regex _isNumber = new Regex(@"^\d+$");
                        if (data.ElementAtOrDefault(2) != null)
                        {
                            if (_isNumber.IsMatch(data.ElementAtOrDefault(2).ToString()))
                            {
                                if (data.Length == 133)
                                {
                                    var employee = new Employees();
                                    employee.PayrollID = Int32.Parse(data.ElementAtOrDefault(2).ToString());
                                    employee.FamilyName = data.ElementAtOrDefault(0).ToString().TrimEnd().TrimStart();
                                    employee.GivenName = data.ElementAtOrDefault(1).ToString().TrimEnd().TrimStart();
                                    employee.Address1 = data.ElementAtOrDefault(4).ToString().Replace("\"", "");
                                    employee.Address2 = data.ElementAtOrDefault(19).ToString().Replace("\"", "");
                                    employee.Address3 = data.ElementAtOrDefault(33).ToString().Replace("\"", "");
                                    employee.Address4 = data.ElementAtOrDefault(47).ToString().Replace("\"", "");
                                    employee.Locality = data.ElementAtOrDefault(8).ToString();
                                    employee.State = data.ElementAtOrDefault(9).ToString();
                                    employee.Postcode = data.ElementAtOrDefault(10).ToString();
                                    employee.Country = data.ElementAtOrDefault(11).ToString();
                                    employee.Email = data.ElementAtOrDefault(16).ToString();
                                    employee.Phone1 = data.ElementAtOrDefault(12).ToString();
                                    employee.Phone2 = data.ElementAtOrDefault(13).ToString();
                                    employee.CustomList2 = data.ElementAtOrDefault(82).ToString();
                                    _context.tblEmployees.Add(employee);
                            
                                    var payroll = new Payroll();
                                    payroll.PayrollID = Int32.Parse(data.ElementAtOrDefault(2).ToString());
                                    payroll.MemberID = data.ElementAtOrDefault(117).ToString();
                                    payroll.Fullname = data.ElementAtOrDefault(1).ToString().TrimEnd().TrimStart() + " " + data.ElementAtOrDefault(0).ToString().TrimEnd().TrimStart();
                                    if (!string.IsNullOrEmpty(data.ElementAtOrDefault(107).ToString()))
                                    {
                                        DateTime oDate = Convert.ToDateTime(data.ElementAtOrDefault(107).ToString());
                                        payroll.DOB = String.Format("{0:dd/MM/yyyy}", oDate);
                                    } else {
                                        payroll.DOB = data.ElementAtOrDefault(107).ToString();
                                    }
                                    payroll.Gender = data.ElementAtOrDefault(108).ToString();
                                    payroll.TFN = data.ElementAtOrDefault(118).ToString();
                                    if (!string.IsNullOrEmpty(data.ElementAtOrDefault(109).ToString()))
                                    {
                                        DateTime dDate;
                                        if (DateTime.TryParse(data.ElementAtOrDefault(109).ToString(), out dDate))
                                        {
                                            DateTime oDate = Convert.ToDateTime(data.ElementAtOrDefault(109).ToString());
                                            payroll.EmpStartDate = String.Format("{0:dd/MM/yyyy}", oDate);
                                        } else {
                                            payroll.EmpStartDate = data.ElementAtOrDefault(109).ToString();
                                        }                               
                                    } else {
                                        payroll.EmpStartDate = null;
                                    }  
                                    if (!string.IsNullOrEmpty(data.ElementAtOrDefault(110).ToString()))
                                    {
                                        DateTime dDate;
                                        if (DateTime.TryParse(data.ElementAtOrDefault(110).ToString(), out dDate))
                                        {
                                            DateTime oDate = Convert.ToDateTime(data.ElementAtOrDefault(110).ToString());
                                            payroll.EmpEndDate = String.Format("{0:dd/MM/yyyy}", oDate);
                                        } else
                                        {
                                            payroll.EmpEndDate = data.ElementAtOrDefault(110).ToString();
                                        }
                                    } else {
                                        payroll.EmpEndDate = null;
                                    }            
                                    if (!string.IsNullOrEmpty(data.ElementAtOrDefault(125).ToString()))
                                    {
                                        if (data.ElementAtOrDefault(125).ToString().Equals("T"))
                                        {
                                            payroll.EmpCategory = "Temporary";
                                        } else if (data.ElementAtOrDefault(125).ToString().Equals("P"))
                                        {
                                            payroll.EmpCategory = "Permanent";
                                        }                          
                                    } else {
                                        payroll.EmpCategory = null;
                                    }           
                                    if (!string.IsNullOrEmpty(data.ElementAtOrDefault(126).ToString()))
                                    {
                                        if (data.ElementAtOrDefault(126).ToString().Equals("C"))
                                        {
                                            payroll.EmpStatus = "Casual";
                                        } else if (data.ElementAtOrDefault(126).ToString().Equals("F"))
                                        {
                                            payroll.EmpStatus = "Full Time";
                                        } else if (data.ElementAtOrDefault(126).ToString().Equals("O"))
                                        {
                                            payroll.EmpStatus = "Other";
                                        } else if (data.ElementAtOrDefault(126).ToString().Equals("P"))
                                        {
                                            payroll.EmpStatus = "Part Time";
                                        }                              
                                    } else {
                                        payroll.EmpStatus = null;
                                    }                                  
                                    payroll.SuperUID = null;
                                    _context.tblPayroll.Add(payroll);
                                } else {
                                    var employee = new Employees();
                                    employee.PayrollID = Int32.Parse(data.ElementAtOrDefault(2).ToString());
                                    employee.FamilyName = data.ElementAtOrDefault(0).ToString().TrimEnd().TrimStart();
                                    employee.GivenName = data.ElementAtOrDefault(1).ToString().TrimEnd().TrimStart();
                                    employee.Address1 = data.ElementAtOrDefault(4).ToString().Replace("\"", "") + ", " + data.ElementAtOrDefault(5).ToString().Replace("\"", "");
                                    employee.Address2 = data.ElementAtOrDefault(19).ToString().Replace("\"", "");
                                    employee.Address3 = data.ElementAtOrDefault(33).ToString().Replace("\"", "");
                                    employee.Address4 = data.ElementAtOrDefault(47).ToString().Replace("\"", "");
                                    employee.Locality = data.ElementAtOrDefault(8).ToString();
                                    employee.State = data.ElementAtOrDefault(9).ToString();
                                    employee.Postcode = data.ElementAtOrDefault(10).ToString();
                                    employee.Country = data.ElementAtOrDefault(11).ToString();
                                    employee.Email = data.ElementAtOrDefault(16).ToString();
                                    employee.Phone1 = data.ElementAtOrDefault(12).ToString();
                                    employee.Phone2 = data.ElementAtOrDefault(13).ToString();
                                    employee.CustomList2 = data.ElementAtOrDefault(82).ToString();
                                    _context.tblEmployees.Add(employee);                                  

                                    var payroll = new Payroll();
                                    payroll.PayrollID = Int32.Parse(data.ElementAtOrDefault(2).ToString());
                                    payroll.MemberID = data.ElementAtOrDefault(117+1).ToString();
                                    payroll.Fullname = data.ElementAtOrDefault(1).ToString().TrimEnd().TrimStart() + " " + data.ElementAtOrDefault(0).ToString().TrimEnd().TrimStart();
                                    if (!string.IsNullOrEmpty(data.ElementAtOrDefault(107+1).ToString()))
                                    {
                                        DateTime dDate;
                                        if (DateTime.TryParse(data.ElementAtOrDefault(107+1).ToString(), out dDate))
                                        {
                                            DateTime oDate = Convert.ToDateTime(data.ElementAtOrDefault(107+1).ToString());
                                            payroll.DOB = String.Format("{0:dd/MM/yyyy}", oDate);
                                        } else {
                                            payroll.DOB = data.ElementAtOrDefault(107+1).ToString();
                                        }                               
                                    } else {
                                        payroll.DOB = data.ElementAtOrDefault(107+1).ToString();
                                    }
                                    payroll.Gender = data.ElementAtOrDefault(108+1).ToString();
                                    payroll.TFN = data.ElementAtOrDefault(118+1).ToString();
                                    if (!string.IsNullOrEmpty(data.ElementAtOrDefault(109+1).ToString()))
                                    {
                                        DateTime dDate;
                                        if (DateTime.TryParse(data.ElementAtOrDefault(109+1).ToString(), out dDate))
                                        {
                                            DateTime oDate = Convert.ToDateTime(data.ElementAtOrDefault(109+1).ToString());
                                            payroll.EmpStartDate = String.Format("{0:dd/MM/yyyy}", oDate);
                                        } else {
                                            payroll.EmpStartDate = data.ElementAtOrDefault(109+1).ToString();
                                        }                                
                                    } else {
                                        payroll.EmpStartDate = null;
                                    }
                                    if (!string.IsNullOrEmpty(data.ElementAtOrDefault(110+1).ToString()))
                                    {
                                        DateTime dDate;
                                        if (DateTime.TryParse(data.ElementAtOrDefault(110+1).ToString(), out dDate))
                                        {
                                            DateTime oDate = Convert.ToDateTime(data.ElementAtOrDefault(110+1).ToString());
                                            payroll.EmpEndDate = String.Format("{0:dd/MM/yyyy}", oDate);
                                        } else {
                                            payroll.EmpEndDate = data.ElementAtOrDefault(110+1).ToString();
                                        }                                
                                    } else {
                                        payroll.EmpEndDate = null;
                                    }
                                    payroll.EmpCategory = data.ElementAtOrDefault(125+1).ToString();
                                    payroll.EmpStatus = data.ElementAtOrDefault(126+1).ToString();
                                    payroll.SuperUID = null;
                                    _context.tblPayroll.Add(payroll);   
                                }
                            }
                        }
                    }
                }         
            }
            if (await _context.SaveChangesAsync() > 0) return Ok();
            
            return BadRequest("Cannot add new file");
        }

        [HttpPost("addcsv/offline")]
        public async Task<IActionResult> SeedEmployee2([FromBody] string RawData)
        {
            string[] strlines = RawData.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.None
                    );
            
            if (strlines[0].Contains("Allied Recruitment"))
            {
                if (await _context.tblAccrual.AnyAsync())
                {
                    using (var connection = _context.Database.GetDbConnection())
                    {
                        await connection.OpenAsync();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "DELETE FROM [tblAccrual]";//"DELETE FROM [tblAccrual]";
                            var result = await command.ExecuteNonQueryAsync();
                            command.CommandText = "delete from sqlite_sequence where name='tblAccrual';";
                            //command.CommandText = "ALTER TABLE tblAccrual AUTO_INCREMENT = 1";
                            result = await command.ExecuteNonQueryAsync();
                        }
                    }
                }

                var SuperFund = string.Empty;
                var EmployerMemberNo = string.Empty;
                var EmployerMember = string.Empty;

                for (int x = 1; x < strlines.Length-1; x++)
                {
                    if (strlines[x].Contains("Superannuation Fund") || strlines[x].Contains("Super Guarantee"))
                    {
                        int i = x;                       
                        if (strlines[x].Contains("Superannuation Fund"))
                        {
                            SuperFund = strlines[x].Substring(strlines[x].LastIndexOf(',') + 1);                           
                            do
                            {
                                i += 1;
                                EmployerMember = strlines[i].TrimEnd();
                                EmployerMember = strlines[i].TrimStart();
                            }
                            while (!EmployerMember.Contains("Employer Membership"));
                            EmployerMemberNo = EmployerMember.Substring(EmployerMember.LastIndexOf(',') + 1);                            
                            do
                            {
                                i += 1;
                            }
                            while (!strlines[i].Contains("Super Guarantee"));
                        }
                        i-=1;
                        var EmployeeMember = string.Empty;
                        EmployeeMember = strlines[i].TrimEnd();
                        EmployeeMember = strlines[i].TrimStart();
                        Regex regex = new Regex("\"(.*?)\"");
                        var matches = regex.Matches(EmployeeMember);
                        var EmployeeName = string.Empty;
                        var Lastname = string.Empty;
                        var Firstname = string.Empty;
                        if (matches.Count > 0)
                        {
                            EmployeeName = matches[0].Groups[1].ToString();
                            string[] words = EmployeeName.Split(',');
                            Lastname = words[0].TrimEnd();
                            Lastname = Lastname.TrimStart();
                            Firstname = words[1].TrimStart();
                            Firstname = Firstname.TrimEnd();
                        }
                        var EmployeeMemberNo = "";                        
                        var Spouse = string.Empty;
                        do
                        {
                            i += 1;
                            Spouse = strlines[i].TrimEnd();
                            Spouse = strlines[i].TrimStart();
                        }
                        while (!Spouse.Contains("Super Guarantee"));

                        //Spouse
                        int y = i;
                        var Spouses = string.Empty;
                        var SpouseTot = string.Empty;

                        //Salary Sacrifice
                        y = i;
                        var Others = string.Empty;
                        var SacrificeTot = string.Empty;

                        //Redundancy
                        y = i;
                        var Redundancy = string.Empty;
                        var RedundancyTot = string.Empty;

                        //Productivity
                        y = i;
                        var Productivity = string.Empty;
                        var ProductivityTot = string.Empty;

                        //Employer Additional
                        y = i;
                        var EmployerAdditional = string.Empty;
                        var EmployerAdditionalTot = string.Empty;

                        //EmployeeAdditional
                        y = i;
                        var EmployeeAdditional = string.Empty;
                        var EmployeeAdditionalTot = string.Empty;

                        //Super Guarantee
                        var SuperGuaranteeTot = string.Empty;
                        if (Spouse.Contains("Super Guarantee"))
                        {
                            matches = regex.Matches(Spouse);
                            if (matches.Count > 0)
                            {
                                SuperGuaranteeTot = matches[0].Groups[1].ToString().Substring(matches[0].Groups[1].ToString().LastIndexOf('$') + 1);
                                if (SuperGuaranteeTot.IndexOf(',') > 0)
                                    SuperGuaranteeTot = SuperGuaranteeTot.Remove(SuperGuaranteeTot.IndexOf(','), 1);                               
                            }
                            else
                            {
                                SuperGuaranteeTot = Spouse.Substring(Spouse.LastIndexOf('$') + 1);
                                if (SuperGuaranteeTot.IndexOf(',') > 0)
                                    SuperGuaranteeTot = SuperGuaranteeTot.Remove(SuperGuaranteeTot.IndexOf(','), 1);                               
                            }
                        }

                        var accrual = new Accrual();
                        accrual.MemberID = EmployeeMemberNo;
                        accrual.LastName = Lastname;
                        accrual.FirstName = Firstname;
                        accrual.SuperFund = SuperFund;
                        accrual.USI = EmployerMemberNo;
                        accrual.SuperGuarantee = Decimal.Parse(SuperGuaranteeTot);
                        if (string.IsNullOrEmpty(SpouseTot))
                            accrual.Spouse = 0.00;
                        else
                            accrual.Spouse = Math.Round(Double.Parse(SpouseTot),2);
                        if (string.IsNullOrEmpty(SacrificeTot))
                            accrual.SalarySacrifice = 0.00;
                        else
                            accrual.SalarySacrifice = Math.Round(Double.Parse(SacrificeTot),2);
                        if (string.IsNullOrEmpty(RedundancyTot))
                            accrual.Redundancy = 0.00;
                        else
                            accrual.Redundancy = Math.Round(Double.Parse(RedundancyTot),2);
                        if (string.IsNullOrEmpty(ProductivityTot))
                            accrual.Productivity = 0.00;
                        else
                            accrual.Productivity = Math.Round(Double.Parse(ProductivityTot),2);
                        if (string.IsNullOrEmpty(EmployerAdditionalTot))
                            accrual.EmployerAdditional = 0.00;
                        else
                            accrual.EmployerAdditional = Math.Round(Double.Parse(EmployerAdditionalTot),2);
                        if (string.IsNullOrEmpty(EmployeeAdditionalTot))
                            accrual.EmployeeAdditional = 0.00;
                        else
                            accrual.EmployeeAdditional = Math.Round(Double.Parse(EmployeeAdditionalTot),2);
                        _context.tblAccrual.Add(accrual);
                        x = i;
                    }
                }
            } else {
                if (await _context.tblEmployees.AnyAsync())
                {
                    using (var connection = _context.Database.GetDbConnection())
                    {
                        await connection.OpenAsync();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "DELETE FROM [tblEmployees]";//"DELETE FROM [tblEmployees]";
                            var result = await command.ExecuteNonQueryAsync();
                            command.CommandText = "delete from sqlite_sequence where name='tblEmployees';";
                            //command.CommandText = "ALTER TABLE tblEmployees AUTO_INCREMENT = 1";
                            result = await command.ExecuteNonQueryAsync();
                            command.CommandText = "DELETE FROM [tblPayroll]";//"DELETE FROM [tblPayroll]";
                            result = await command.ExecuteNonQueryAsync();
                            command.CommandText = "delete from sqlite_sequence where name='tblPayroll';";
                            //command.CommandText = "ALTER TABLE tblPayroll AUTO_INCREMENT = 1";
                            result = await command.ExecuteNonQueryAsync();
                        }
                    }
                }

                DateTime? sdate = new DateTime();
                sdate = Convert.ToDateTime("1971-01-01 00:00:00");
                foreach (var line in strlines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line == "{}")             
                        {
                            continue;   
                        }

                        string[] data = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);
                        Regex _isNumber = new Regex(@"^\d+$");
                        if (_isNumber.IsMatch(data.ElementAtOrDefault(2).ToString()))
                        {
                            var employee = new Employees();
                            employee.PayrollID = Int32.Parse(data.ElementAtOrDefault(2).ToString());
                            employee.FamilyName = data.ElementAtOrDefault(0).ToString().TrimEnd().TrimStart();
                            employee.GivenName = data.ElementAtOrDefault(1).ToString().TrimEnd().TrimStart();
                            employee.Address1 = data.ElementAtOrDefault(5).ToString().Replace("\"", "");
                            employee.Address2 = data.ElementAtOrDefault(20).ToString().Replace("\"", "");
                            employee.Address3 = data.ElementAtOrDefault(35).ToString().Replace("\"", "");
                            employee.Address4 = data.ElementAtOrDefault(50).ToString().Replace("\"", "");
                            employee.Locality = data.ElementAtOrDefault(9).ToString();
                            employee.State = data.ElementAtOrDefault(10).ToString();
                            employee.Postcode = data.ElementAtOrDefault(11).ToString();
                            employee.Country = data.ElementAtOrDefault(12).ToString();
                            employee.Email = data.ElementAtOrDefault(133).ToString().Replace("\"", "");
                            employee.Phone1 = data.ElementAtOrDefault(13).ToString();
                            employee.Phone2 = data.ElementAtOrDefault(14).ToString();
                            _context.tblEmployees.Add(employee);

                            var payroll = new Payroll();
                            payroll.PayrollID = Int32.Parse(data.ElementAtOrDefault(2).ToString());
                            payroll.MemberID = data.ElementAtOrDefault(119).ToString();
                            payroll.Fullname = data.ElementAtOrDefault(1).ToString().TrimEnd().TrimStart() + " " + data.ElementAtOrDefault(0).ToString().TrimEnd().TrimStart();
                            if (!string.IsNullOrEmpty(data.ElementAtOrDefault(109).ToString()))
                            {
                                DateTime oDate = Convert.ToDateTime(data.ElementAtOrDefault(109).ToString());
                                payroll.DOB = String.Format("{0:dd/MM/yyyy}", oDate);
                            } else {
                                payroll.DOB = data.ElementAtOrDefault(109).ToString();
                            }
                            payroll.Gender = data.ElementAtOrDefault(110).ToString();
                            payroll.TFN = data.ElementAtOrDefault(120).ToString();
                            if (!string.IsNullOrEmpty(data.ElementAtOrDefault(111).ToString()))
                            {
                                DateTime dDate;
                                if (DateTime.TryParse(data.ElementAtOrDefault(111).ToString(), out dDate))
                                {
                                    DateTime oDate = Convert.ToDateTime(data.ElementAtOrDefault(111).ToString());
                                    payroll.EmpStartDate = String.Format("{0:dd/MM/yyyy}", oDate);
                                } else {
                                    payroll.EmpStartDate = data.ElementAtOrDefault(111).ToString();
                                }                               
                            } else {
                                payroll.EmpStartDate = null;
                            }  
                            if (!string.IsNullOrEmpty(data.ElementAtOrDefault(112).ToString()))
                            {
                                DateTime dDate;
                                if (DateTime.TryParse(data.ElementAtOrDefault(112).ToString(), out dDate))
                                {
                                    DateTime oDate = Convert.ToDateTime(data.ElementAtOrDefault(112).ToString());
                                    payroll.EmpEndDate = String.Format("{0:dd/MM/yyyy}", oDate);
                                } else {
                                    payroll.EmpEndDate = data.ElementAtOrDefault(112).ToString();
                                }
                                
                            } else {
                                payroll.EmpEndDate = null;
                            }                   
                            if (!string.IsNullOrEmpty(data.ElementAtOrDefault(128).ToString()))
                            {
                                if (data.ElementAtOrDefault(128).ToString().Equals("C"))
                                {
                                    payroll.EmpStatus = "Casual";
                                } else if (data.ElementAtOrDefault(128).ToString().Equals("F"))
                                {
                                    payroll.EmpStatus = "Full Time";
                                }                                
                            } else {
                                payroll.EmpStatus = null;
                            }                                  
                            payroll.SuperUID = null;
                            _context.tblPayroll.Add(payroll);
                        
                        }
                    }
                }
            }

            if (await _context.SaveChangesAsync() > 0) return Ok();

            return BadRequest("Failed to add new file");
        }

        [HttpDelete]
        public async Task<IActionResult> ClearData()
        {
            if (await _context.tblAccrual.AnyAsync())
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM [tblAccrual]";//"DELETE FROM [tblAccrual]";
                        var result = await command.ExecuteNonQueryAsync();
                        command.CommandText = "delete from sqlite_sequence where name='tblAccrual';";
                        //command.CommandText = "ALTER TABLE tblAccrual AUTO_INCREMENT = 1";
                        result = await command.ExecuteNonQueryAsync();
                    }
                }
            }

            if (await _context.tblEmployees.AnyAsync())
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM [tblEmployees]";//"DELETE FROM [tblEmployees]";
                        var result = await command.ExecuteNonQueryAsync();
                        command.CommandText = "delete from sqlite_sequence where name='tblEmployees';";
                        //command.CommandText = "ALTER TABLE tblEmployees AUTO_INCREMENT = 1";
                        result = await command.ExecuteNonQueryAsync();
                        command.CommandText = "DELETE FROM [tblPayroll]";//"DELETE FROM [tblPayroll]";
                        result = await command.ExecuteNonQueryAsync();
                        command.CommandText = "delete from sqlite_sequence where name='tblPayroll';";
                        //command.CommandText = "ALTER TABLE tblPayroll AUTO_INCREMENT = 1";
                        result = await command.ExecuteNonQueryAsync();
                    }
                }
            }
            return Ok();
        }
    }
}