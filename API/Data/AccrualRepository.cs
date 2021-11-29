using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace API.Data
{
  public class AccrualRepository : IAccrualRepository
  {
    private readonly DataContext _context;
    public AccrualRepository(DataContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<dynamic>> GetAccrualErrorAsync()
    {
       var query = await (from accrual in _context.tblAccrual
                        join employee in _context.tblEmployees on new { c1 = accrual.LastName, c2 = accrual.FirstName } equals new { c1 = employee.FamilyName, c2 = employee.GivenName + " " + employee.OtherGivenName } 
                        join payroll in _context.tblPayroll on employee.PayrollID equals payroll.PayrollID into supers
                        from r in supers.DefaultIfEmpty()
                        where accrual.SuperFund.ToLower().StartsWith("sunsuper") && r.MemberID == ""
                        select new {
                            USI = accrual.USI,
                            PayrollID = employee.PayrollID,
                            MemberID = r.MemberID,
                            LastName = accrual.LastName,
                            FirstName = accrual.FirstName,
                            DOB = r.DOB,
                            Gender = r.Gender,
                            // SuperGuarantee = accrual.SuperGuarantee.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                            SuperGuarantee = accrual.SuperGuarantee,
                            Spouse = accrual.Spouse,
                            SalarySacrifice = accrual.SalarySacrifice,
                            Redundancy = accrual.Redundancy,
                            Productivity = accrual.Productivity,
                            EmployerAdditional = accrual.EmployerAdditional,
                            EmployeeAdditional = accrual.EmployeeAdditional,
                            Address1 = employee.Address1,
                            Address2 = employee.Address2,
                            Address3 = employee.Address3,
                            Address4 = employee.Address4,
                            Locality = employee.Locality,
                            State = employee.State,
                            Postcode = employee.Postcode,
                            Country = employee.Country,
                            TFN = r.TFN,
                            EmpStartDate = r.EmpStartDate,
                            EmpEndDate = r.EmpEndDate,
                            EmpCategory = r.EmpCategory,
                            EmpStatus = r.EmpStatus,
                            CustomList2 = employee.CustomList2,
                            Email = employee.Email,
                            Phone1 = employee.Phone1,
                            Phone2 = employee.Phone2
                        }).ToListAsync();

        return query;
    }

    public async Task<IEnumerable<dynamic>> GetAccrualAsync()
    {
       var query = await (from accrual in _context.tblAccrual
                        join employee in _context.tblEmployees on new { c1 = accrual.LastName, c2 = accrual.FirstName } equals new { c1 = employee.FamilyName, c2 = employee.GivenName + " " + employee.OtherGivenName } 
                        join payroll in _context.tblPayroll on employee.PayrollID equals payroll.PayrollID into supers
                        from r in supers.DefaultIfEmpty()
                        select new {
                            USI = accrual.USI,
                            PayrollID = employee.PayrollID,
                            MemberID = r.MemberID,
                            LastName = accrual.LastName,
                            FirstName = accrual.FirstName,
                            DOB = r.DOB,
                            Gender = r.Gender,
                            // SuperGuarantee = accrual.SuperGuarantee.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                            SuperGuarantee = accrual.SuperGuarantee,
                            Spouse = accrual.Spouse,
                            SalarySacrifice = accrual.SalarySacrifice,
                            Redundancy = accrual.Redundancy,
                            Productivity = accrual.Productivity,
                            EmployerAdditional = accrual.EmployerAdditional,
                            EmployeeAdditional = accrual.EmployeeAdditional,
                            Address1 = employee.Address1,
                            Address2 = employee.Address2,
                            Address3 = employee.Address3,
                            Address4 = employee.Address4,
                            Locality = employee.Locality,
                            State = employee.State,
                            Postcode = employee.Postcode,
                            Country = employee.Country,
                            TFN = r.TFN,
                            EmpStartDate = r.EmpStartDate,
                            EmpEndDate = r.EmpEndDate,
                            EmpCategory = r.EmpCategory,
                            EmpStatus = r.EmpStatus,
                            CustomList2 = employee.CustomList2,
                            Email = employee.Email,
                            Phone1 = employee.Phone1,
                            Phone2 = employee.Phone2
                        }).ToListAsync();

        return query;
    }

    public async Task<IEnumerable<dynamic>> GetAccrualExcelAsync()
    {
       var query = await (from accrual in _context.tblAccrual
                        join employee in _context.tblEmployees on new { c1 = accrual.LastName, c2 = accrual.FirstName } equals new { c1 = employee.FamilyName, c2 = employee.GivenName + " " + employee.OtherGivenName } 
                        join payroll in _context.tblPayroll on employee.PayrollID equals payroll.PayrollID into supers
                        from r in supers.DefaultIfEmpty()
                        select new {
                            TFN = r.TFN,
                            GivenName = employee.GivenName,
                            OtherGivenName = employee.OtherGivenName,
                            LastName = employee.FamilyName,
                            DOB = r.DOB,
                            PostalorResidential  = "",                       
                            Address1 = employee.Address1,
                            Address2 = employee.Address2,
                            Locality = employee.Locality,
                            State = employee.State,
                            Postcode = employee.Postcode,
                            Country = employee.Country
                        }).ToListAsync();

        return query;
    }

    public async Task<IEnumerable<dynamic>> GetAccrualExcelFilterAsync()
    {
       var query = await (from accrual in _context.tblAccrual
                        join employee in _context.tblEmployees on new { c1 = accrual.LastName, c2 = accrual.FirstName } equals new { c1 = employee.FamilyName, c2 = employee.GivenName + " " + employee.OtherGivenName } 
                        join payroll in _context.tblPayroll on employee.PayrollID equals payroll.PayrollID into supers
                        from r in supers.DefaultIfEmpty()
                        where accrual.SuperFund.ToLower().StartsWith("sunsuper") && r.MemberID == ""
                        select new {
                            TFN = r.TFN,
                            GivenName = employee.GivenName,
                            OtherGivenName = employee.OtherGivenName,
                            LastName = employee.FamilyName,
                            DOB = r.DOB,
                            PostalorResidential  = "",                       
                            Address1 = employee.Address1,
                            Address2 = employee.Address2,
                            Locality = employee.Locality,
                            State = employee.State,
                            Postcode = employee.Postcode,
                            Country = employee.Country
                        }).ToListAsync();

        return query;
    }

    public async Task<bool> SaveAllAsync()
    {
      return await _context.SaveChangesAsync() > 0;
    }

    public void Update(Accrual tblAccrual)
    {
      _context.Entry(tblAccrual).State = EntityState.Modified;
    }
  }
}