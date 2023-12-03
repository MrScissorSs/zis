using Microsoft.AspNetCore.Mvc;
using zis.Models;

namespace zis.Controllers
{
    public class ProductoController : Controller
    {
        private readonly AppDbContext db;

        public ProductoController(AppDbContext db)
        {
            this.db = db;
        }

        public IActionResult IndexProductos()
        {
            var prods = db.Productos.ToList();
            return View(prods);
        }

        public IActionResult AddProducto() { return View(); }

        [HttpPost]
        public IActionResult AddProducto(Producto P)
        {
            if (ModelState.IsValid)
            {
                db.Productos.Add(P);
                db.SaveChanges();
                return RedirectToAction(nameof(IndexProductos));
            }
            return View(P);
        }

        [HttpGet]
        public IActionResult EliminarProducto(int Id)
        {
            var product = db.Productos.FirstOrDefault(x => x.Id == Id);
            if (product == null) return NotFound();
            db.Productos.Remove(product);
            db.SaveChanges();
            return RedirectToAction(nameof(IndexProductos));
        }



    }
}
