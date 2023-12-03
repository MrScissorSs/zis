using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using zis.Models;

namespace zis.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly AppDbContext db;

        public UsuarioController(AppDbContext db)
        {
            this.db = db;
        }

        [HttpGet]

        public IActionResult IndexUsuario()
        {
            var Users = db.Usuarios.ToList();
            return View(Users);
        }

        public IActionResult AddUsuario()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUsuario(CrearUserViewModel Uvm)
        {

            if (ModelState.IsValid)
            {
                Usuario U = new Usuario();
                U.Name = Uvm.Nombre;
                U.Email = Uvm.Email;
                CreatePasswordHash(Uvm.Password, out byte[] passwordHash, out byte[] passwordSalt);
                U.PasswordHash = passwordHash;
                U.PasswordSalt = passwordSalt;
                U.Rol = Uvm.Rol;
                db.Usuarios.Add(U);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(IndexUsuario));

            }
            else
            {
                ModelState.AddModelError("", "Error en agregar, Por favor verifique sus datos.");
                return View(Uvm);
            }

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));


            }
        }
    }
}
