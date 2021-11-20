using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface IAccrualRepository
    {
        void Update(Accrual tblAccrual);
        void Delete(Accrual tblAccrual);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<dynamic>> GetAccrualErrorAsync();
    }
}