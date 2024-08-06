using Microsoft.EntityFrameworkCore;
using Tickets_selling_App.Migrations;
using Tickets_selling_App.Models;

namespace Tickets_selling_App
{
    public class Tkt_Dbcontext :DbContext
    {
        public Tkt_Dbcontext(DbContextOptions<Tkt_Dbcontext> options) : base(options)
        {

        }
        public DbSet<User> User { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketInstance> TicketInstances { get; set; }
        public DbSet<PasswordReset> PasswordReset { get; set; }
        public DbSet<Email_Validation> Emailvalidation { get; set; }
        public DbSet<CreatorAccount> CreatorAccount { get; set; }
        public DbSet<CreatorAccountRoles> AccountRoles { get; set; }    
    }

}
