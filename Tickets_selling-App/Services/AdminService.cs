using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Services
{
    public class AdminService : AdminInterface
    {
        private readonly Tkt_Dbcontext _context;

        public AdminService(Tkt_Dbcontext context)
        {
            _context = context;
        }
    }
}