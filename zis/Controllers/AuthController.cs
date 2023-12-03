using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Security.Cryptography;
using zis.Models;

namespace zis.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task <IActionResult> LoginIn()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> LoginIn(LoginViewModel lvm)
        {
            var usuarios = _context.Usuarios.ToList();
            if (usuarios.Count == 0)
            {
                var superAdmin = new Usuario();
                superAdmin.Name = "Admin";
                superAdmin.Email = "admin@admin.cl";
                superAdmin.Rol = "SuperAdministrador";
                CreatePasswordHash("123456", out byte[] hash, out byte[] salt);
                superAdmin.PasswordHash = hash;
                superAdmin.PasswordSalt = salt;
                _context.Add(superAdmin);
                await _context.SaveChangesAsync();
            }

            var user = _context.Usuarios.FirstOrDefault(u => u.Email.Equals(lvm.Email));
            if (user == null)
            {
                ModelState.AddModelError("", "Usuario No existe");
                return View();
            }
            if (VerifyPassword(lvm.Password, user.PasswordHash, user.PasswordSalt))
            {
                var claims = new List<Claim>(){
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Rol)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties { IsPersistent = true});

                return RedirectToAction("index", "home");
            }
            return View();
        }

        [HttpGet]
        public async Task<RedirectToActionResult> LogOut()
        {
            await  HttpContext.SignOutAsync();
            return RedirectToAction("LoginIn", "Auth");
        }
        
        public IActionResult AddUsuarioCliente()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUsuarioCliente(CrearUserViewModel Uvm)
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
                _context.Usuarios.Add(U);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(LoginIn));

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

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }

}