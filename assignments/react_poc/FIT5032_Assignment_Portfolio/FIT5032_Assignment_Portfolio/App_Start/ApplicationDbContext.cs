using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Reflection;

namespace FIT5032_Assignment_Portfolio.App_Start
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> users { get; set; }

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, (IOptions<Duende.IdentityServer.EntityFramework.Options.OperationalStoreOptions>)operationalStoreOptions)
        {
        }
    }
}
