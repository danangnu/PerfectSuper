using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class SeedEmployees
    {
        public static async Task SeedEmployee(DataContext context, string RawData)
        {
            if (await context.tblEmployees.AnyAsync()) return;

            
        }
    }
}