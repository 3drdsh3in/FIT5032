using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace FIT5032_Assignment_Portfolio.App_Start
{
    public class HelperService : IHelperService
    {
        public HelperService()
        {
        }

        public async Task<PropertyValues> RetrieveEntity(DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Single();
            var clientUser = (ApplicationUser)entry.Entity;
            return await entry.GetDatabaseValuesAsync();
        }
    }
}
