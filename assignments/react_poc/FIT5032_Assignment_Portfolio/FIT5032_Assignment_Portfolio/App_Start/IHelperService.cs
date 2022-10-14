using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace FIT5032_Assignment_Portfolio.App_Start
{
    public interface IHelperService
    {
        Task<PropertyValues> RetrieveEntity(DbUpdateConcurrencyException ex);
    }
}
