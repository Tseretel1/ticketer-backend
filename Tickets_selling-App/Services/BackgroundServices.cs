using Microsoft.EntityFrameworkCore;

namespace Tickets_selling_App.Services
{
    public class BackgroundServices
    {
        private readonly Tkt_Dbcontext _context;
        public BackgroundServices(Tkt_Dbcontext context)
        {
            _context = context;
        }
        public void DeleteExpiredEmailValidations()
        {
            var ValidationsToRemove = _context.Emailvalidation.Where(x => x.Expiration < DateTime.Now);
            _context.Emailvalidation.RemoveRange(ValidationsToRemove);
            _context.SaveChanges();
        }
    }
}
