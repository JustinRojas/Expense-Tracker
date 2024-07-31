using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Linq;
using System.Globalization;
using System.Transactions;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context; 
        public DashboardController(ApplicationDbContext context)
        {
            _context = context; 
        }
        public async Task<IActionResult> Index()
        {
            //Ultimos 7 dias
            DateTime StarDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            //se crea una lista de transacciones y se le pasa lo que se encuentra en la BD, haciendo filtro 
            //con la fecha de los ultimos 7 dias, 
            List<Models.Transaction> SelectedTransactions = await _context.Transactions.
                Include(x => x.Category)
                .Where(y => y.Date >= StarDate && y.Date <= EndDate)
                .ToListAsync();

            //selecciona el total de Amount en dolares para  'Income' que se encuentren en la lista SelectedTransactions
            int TotalIncome = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);

            //se pasa el total a la Viewbag para ser llamado en la vista
            ViewBag.TotalIncome = TotalIncome.ToString("C0");  

            //selecciona el total de Amount en dolares para 'Expense' que se encuentren en la lista SelectedTransactions
            int TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);

            //se pasa el total a la variable Viewbag.TotalExpense para ser llamado en la vista
            ViewBag.TotalExpense = TotalExpense.ToString("C0");
           
            //Balance se hace la resta del dinero que tenemos, menos el que hemos gastado.
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture, "{0:C0}",Balance);

            return View();
        }
    }
}
