using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class AccrualController : BaseApiController
  {
    private readonly IAccrualRepository _accrualRepository;
    public AccrualController(IAccrualRepository accrualRepository)
    {
      _accrualRepository = accrualRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<dynamic>> GetSuperData()
    {
      return await _accrualRepository.GetAccrualAsync();
    }

    [HttpGet("f2")]
    public async Task<IEnumerable<dynamic>> GetSuperDataFilter()
    {
      return await _accrualRepository.GetAccrualErrorAsync();
    }

    [HttpGet("excel")]
    public async Task<IEnumerable<dynamic>> GetSuperExcelData()
    {
      return await _accrualRepository.GetAccrualExcelAsync();
    }

    [HttpGet("excel2")]
    public async Task<IEnumerable<dynamic>> GetSuperDataExcelFilter()
    {
      return await _accrualRepository.GetAccrualExcelFilterAsync();
    }
  }
}