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
        public DbSet<SoldTickets>SoldTickets { get; set; }  
        public DbSet<PasswordReset> PasswordReset { get; set; }
        public DbSet<EmailValidation> Emailvalidation { get; set; }
        public DbSet<CreatorAccount> CreatorAccount { get; set; }
        public DbSet<CreatorAccountRoles> AccountRoles { get; set; }  
        public DbSet<CreatorValidation> CreatorValidation { get; set; }
    }

}
