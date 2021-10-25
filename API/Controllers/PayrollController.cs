using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class PayrollController : BaseApiController
    {
        private readonly DataContext _context;
        public PayrollController(DataContext context)
        {
            _context = context;
        }       

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payroll>>> GetPayrolls()
        {
            return await _context.tblPayroll.ToListAsync();           
        }

        // api/accrual/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Payroll>> GetPayroll(int id)
        {
            return await _context.tblPayroll.FindAsync(id);          
        }
    }
}