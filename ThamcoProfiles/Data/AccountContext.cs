using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThamcoProfiles.Models;

namespace ThamcoProfiles.Data
{
    public class AccountContext : DbContext
    {
        public AccountContext (DbContextOptions<AccountContext> options)
            : base(options)
        {
        }

        public DbSet<ThamcoProfiles.Models.User> User { get; set; } = default!;
    }
}
