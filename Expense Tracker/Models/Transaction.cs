using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        //Esta es la forma de pasar una llave foranea
        public int CategoryId { get; set; }
        public Categoria Categoria { get; set; } //llave foranea
        public int Amount { get; set; }
        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryTitleWithIcon
        {
            get
            {
                return Categoria == null ? "": Categoria.Icon + " " + Categoria.Title;
            }
        }
    }
}
