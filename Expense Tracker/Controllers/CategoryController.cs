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
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

    

        // GET: Category/AddOrEdit
        //Este metodo esta editado para tener una sola vista que edite y agregue
        //El valor por defecto es 0, es decir si no esta pasando ningun objeto desde la vista
        //    si se quiere agregar uno, y si se quiere editar pasaria el id del objeto y se busca en la bd
        public IActionResult AddOrEdit( int id=0)
        {

            if(id == 0)
            return View( new Categoria());
            else
                return View(_context.Categories.Find(id));
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("CategoryId,Title,Icon,Type")] Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if(categoria.CategoryId==0)
                _context.Add(categoria);
                else
                    _context.Update(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

    
     

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categories.FindAsync(id);
            if (categoria != null)
            {
                _context.Categories.Remove(categoria);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

  
    }
}
