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

            using (var sreader = new StreamReader(file.OpenReadStream()))
            {
                while (!sreader.EndOfStream) 
                {
                    var line = sreader.ReadLine();
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
            if (await _context.SaveChangesAsync() > 0) return Ok();
            
            return BadRequest("Cannot add new file");
        }

        [HttpPost("addcsv/offline")]
        public async Task<IActionResult> SeedEmployee2([FromBody] string RawData)
        {
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

            string[] lines = RawData.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.None
                    );

            DateTime? sdate = new DateTime();
            sdate = Convert.ToDateTime("1971-01-01 00:00:00");
            foreach (var line in lines)
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
                        await _context.SaveChangesAsync();

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
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return Ok();
        }
    }
}