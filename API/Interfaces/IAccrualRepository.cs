using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface IAccrualRepository
    {
        void Update(Accrual tblAccrual);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<dynamic>> GetAccrualErrorAsync();
        Task<IEnumerable<dynamic>> GetAccrualAsync();
        Task<IEnumerable<dynamic>> GetAccrualExcelAsync();
        Task<IEnumerable<dynamic>> GetAccrualExcelFilterAsync();
    }
}