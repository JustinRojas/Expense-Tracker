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
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            //Ultimos 7 dias
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            //se crea una lista de transacciones y se le pasa lo que se encuentra en la BD, haciendo filtro 
            //con la fecha de los ultimos 7 dias, incluyendo las las categorias debido a la clave foranea
            //se debe hacer de ese modo
            List<Models.Transaction> SelectedTransactions = await _context.Transactions.
                Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            //selecciona el total de Amount en dolares para  'Income' que se encuentren en la lista SelectedTransactions
            int TotalIncome = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);
            //se pasa el total a la Viewbag para ser llamado en la vista
            ViewBag.TotalIncome = String.Format(culture, "{0:C0}", TotalIncome);

            //selecciona el total de Amount en dolares para 'Expense' que se encuentren en la lista SelectedTransactions
            int TotalExpense = SelectedTransactions
                .Where( i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);

            //se pasa el total a la variable Viewbag.TotalExpense para ser llamado en la vista
            ViewBag.TotalExpense = String.Format(culture, "{0:C0}", TotalExpense);

            //Balance se hace la resta del dinero que tenemos, menos el que hemos gastado.
            int Balance = TotalIncome - TotalExpense;
            
            ViewBag.Balance = String.Format(culture, "{0:C0}",Balance);

            //Doughnut chart - expense by category
            ViewBag.DoughnutChartData =  SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    //se crean y se les da valor a las variables que se van a usar en el grafico
                    categoryTitleWithIcon = k.First().Category.Icon +" "+ k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = String.Format(culture, "{0:C0}", k.Sum(j => j.Amount)),
                    

                })
                .OrderByDescending(l => l.amount) //se ordena de dorma desc, para que en el grafico se vea mas ordenado
                .ToList();


            // selecciona las transacciones Income, de la tabla transactions y crea una lista de objetos de la clase SplineChartData
            //para utiulizar sus variables day-income y esto se repite para Expense
            //Spline Chart - Income vs Esxpense
            // Income

            List<SplineChartData> IncomeSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount)
                }).ToList();

            //Expense
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)
                }).ToList();

            //Combine   Income & Expense
            string[] Last7Days = Enumerable.Range(0, 7)
                  .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                  .ToArray();

            ViewBag.SplineChartData = from day in Last7Days
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                     join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };
            //Transacciones recientes

            ViewBag.RecentTransactions = await _context.Transactions
              .Include(i => i.Category)
              .OrderByDescending(j => j.Date)
              .Take(5)
              .ToListAsync();

            
            return View();
        }

        //se crea esta clase para poder usar en la vista los datos que se necesitan en el 
        //grafico splite
        public class SplineChartData
        {
            public string day;
            public int income;
            public int expense;
        }

        }

    }


