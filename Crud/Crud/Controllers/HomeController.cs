using Crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Crud.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        sajerman_crudContext _context = new sajerman_crudContext();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }        
        
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        async public Task<IActionResult> Cadastrar([FromForm]Usuario usuarioCadastrar)
        {
            if(usuarioCadastrar.Nome == null || usuarioCadastrar.Sobrenome == null || usuarioCadastrar.Cpf == null || 
                usuarioCadastrar.Endereco == null || usuarioCadastrar.Numero == 0 ||
                usuarioCadastrar.Complemento == null || usuarioCadastrar.Cidade == null || usuarioCadastrar.Estado == null)
            {
                ViewBag.Error = "Todos os campos devem ser preenchidos";
                return View();
            }
            if(usuarioCadastrar.DataNascimento < new DateTime(1900, 1, 1))
            {
                ViewBag.ErrorDate = "A Data de Nascimento não pode ser inferior a 01/01/1900";
                return View();
            }
            try
            {
                await _context.AddAsync(usuarioCadastrar);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                var t = e.Message;
            }
            ViewBag.Success = "Registro Salvo com Sucesso";
            return View();
        }

        async public Task<IActionResult> ListarCadastrados()
        {
            var usuarios = await _context.Usuarios.ToListAsync();

            return View(usuarios);
        }

        async public Task<IActionResult> EditarUsuario(int id)
        {
            var usuario = await _context.Usuarios.Where(b=>b.Id == id).FirstOrDefaultAsync();
            if(usuario == null)
            {
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        [HttpPost]
        async public Task<IActionResult> EditarUsuario([FromForm] Usuario usuarioEditar)
        {
            if (usuarioEditar.Nome == null || usuarioEditar.Sobrenome == null || usuarioEditar.Cpf == null ||
                    usuarioEditar.Endereco == null || usuarioEditar.Numero == 0 ||
                    usuarioEditar.Complemento == null || usuarioEditar.Cidade == null || usuarioEditar.Estado == null)
            {
                ViewBag.Error = "Todos os campos devem ser preenchidos";
                return View();
            }
            if (usuarioEditar.DataNascimento < new DateTime(1900, 1, 1))
            {
                ViewBag.ErrorDate = "A Data de Nascimento não pode ser inferior a 01/01/1900";
                return View();
            }

            SqlConnection conn = new SqlConnection(_context.Database.GetConnectionString());

            string query = "update Usuarios set " +
                $"nome = '{usuarioEditar.Nome}', " +
                $"sobrenome = '{usuarioEditar.Sobrenome}', " +
                $"cpf = '{usuarioEditar.Cpf}', " +
                $"dataNascimento = '{usuarioEditar.DataNascimento.ToString("yyyy-MM-dd")}', " +
                $"cep = '{usuarioEditar.Cep}', " +
                $"endereco = '{usuarioEditar.Endereco}', " +
                $"numero = {usuarioEditar.Numero}, " +
                $"complemento = '{usuarioEditar.Complemento}', " +
                $"cidade = '{usuarioEditar.Cidade}', " +
                $"estado = '{usuarioEditar.Estado}' " +
                $"where id = {usuarioEditar.Id}";
            
            try
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = query;
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                var message = e.Message;
            }
            finally
            {
                conn.Close();
            }

            return RedirectToAction("Index");
        }
    }
}
