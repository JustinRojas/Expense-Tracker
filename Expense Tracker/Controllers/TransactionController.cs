using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;

namespace Expense_Tracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var aplicationDbContext = _context.Transactions.Include(t => t.Categoria);
            return View(await aplicationDbContext.ToListAsync());
        }


        public IActionResult AddOrEdit()
        {
            //LLamamos la funcion que creamos en la parte final de este controller que devuelve una coleccion 
            //de las categorias que se encuentran en la bd
            PopulateCategories();
            return View( new Transaction());
        }

    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }



   

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void PopulateCategories()
        {
            //aca se le pasa las categorias existentes en la bd a la vairable CategoriesCollection que es una coleccion
            var CategoriesCollection = _context.Categories.ToList();
            // Se pone la categoria por defecto para que apareza en el DrowDownList
            Categoria DefaultCategory = new Categoria() { CategoryId = 0, Title = "Choose a Category" };
            //Insertamos en la coleccion de categorias la categoria que creamos en el paso anterior
            CategoriesCollection.Insert(0, DefaultCategory);
            //Lo pasamos al elemento ViewBag para poder pasar la lista al frontend
            ViewBag.Categories = CategoriesCollection;
        }

     
    }
}
