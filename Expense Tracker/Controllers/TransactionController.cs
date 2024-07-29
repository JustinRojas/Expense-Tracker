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
            var aplicationDbContext = _context.Transactions.Include(t => t.Category);
            return View(await aplicationDbContext.ToListAsync());
        }


        //public IActionResult AddOrEdit( int id=0)
        //{
        //    //LLamamos la funcion que creamos en la parte final de este controller que devuelve una coleccion 
        //    //de las categorias que se encuentran en la bd

        //    PopulateCategories();
        //    if(id == 0)
        //        return View(new Transaction());
        //    else
        //    return View( _context.Transactions.Find(id));
        //}
        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            PopulateCategories();
            if (id == 0)
                return View(new Transaction());
            else
                return View(_context.Transactions.Find(id));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.TransactionId == 0)
                    _context.Add(transaction);
                else
                    _context.Update(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //Como hay llave foranea la forma de seleccionar para crear un elemento es la siguiente
            // crea una lista seleccionando del contexto las donde el CategoryId en las tablas 
            // Categories y transactions coinciden
            PopulateCategories();
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
            Category DefaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
            //Insertamos en la coleccion de categorias la categoria que creamos en el paso anterior
            CategoriesCollection.Insert(0, DefaultCategory);
            //Lo pasamos al elemento ViewBag para poder pasar la lista al frontend
            ViewBag.Categories = CategoriesCollection;
        }

     
    }
}
