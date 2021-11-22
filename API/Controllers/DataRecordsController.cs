using System;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class DataRecordsController : BaseApiController
    {
        private readonly DataContext _context;
        public DataRecordsController(DataContext context)
        {
            _context = context;
        }

        // api/datarecords/fname
        [HttpGet("{fname}")]
        public async Task<DataRecords> GetAccrual(string fname)
        {
            return await _context.tblDataRecords.FirstOrDefaultAsync(u => u.FieldName.ToLower() == fname.ToLower());     
        }

        [HttpPost("updateEmp")]
        public async Task<IActionResult> UpdateEmp()
        {
            var entity = await _context.tblDataRecords.FirstOrDefaultAsync(item => item.FieldName == "Employees");
            if (entity != null)
            {
                entity.Updated = DateTime.Now;       
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost("updateAcr")]
        public async Task<IActionResult> UpdateAcr()
        {
            var entity = await _context.tblDataRecords.FirstOrDefaultAsync(item => item.FieldName == "Accrual");
            if (entity != null)
            {
                entity.Updated = DateTime.Now;       
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost("updateDesc/{fn}/{value}")]
        public async Task<ActionResult<DataRecords>> UpdateDesc(string fn, int value)
        {
            var entity = await _context.tblDataRecords.FirstOrDefaultAsync(item => item.FieldName == fn);
            if (entity != null)
            {
                entity.Updated = DateTime.Now;
                entity.MYOB = value;
                if (value == 0)
                    entity.Desc = "using MyOB Online";
                else 
                    entity.Desc = "using MyOB Desktop";
                if (await _context.SaveChangesAsync() > 0) return entity;
            }
            return BadRequest("Error save DataRecords");
        }
    }
}