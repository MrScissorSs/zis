using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zis.Models;

namespace zis.Controllers
{
    public class CitasController : Controller
    {
      
        private readonly AppDbContext db;

        public CitasController(AppDbContext db)
        {
            this.db = db;
        }

        public IActionResult IndexCitas()
        {
            var cits = db.Cita.ToList();
            return View(cits);
        }

        public IActionResult AddCitas() { return View(); }

        [HttpPost]
        public IActionResult AddCitas(Citas c)
        {
            if (ModelState.IsValid)
            {
                db.Cita.Add(c);
                db.SaveChanges();
                return RedirectToAction(nameof(IndexCitas));
            }
            return View(c);
        }

        





    }
}
