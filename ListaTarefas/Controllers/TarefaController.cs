using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ListaTarefas.DbRepository;
using ListaTarefas.Models;

namespace ListaTarefas.Controllers
{
    public class TarefaController : Controller
    {
        private readonly AppDbContext _context;

        public TarefaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter,string searchString, int? pageNumber)
        {
            ViewData["OrgazinacaoAtual"] = sortOrder;
            ViewData["OrganizarPorDesc"] = String.IsNullOrEmpty(sortOrder) ? "descricao" : "";
            ViewData["OrganizarPorData"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["FiltroAtual"] = searchString;

            var tarefas = from s in _context.Tarefa
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                tarefas = tarefas.Where(s => s.Descricao.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "descricao":
                    tarefas = tarefas.OrderByDescending(t => t.Descricao);
                    break;
                case "Date":
                    tarefas = tarefas.OrderBy(t => t.Data_criacao);
                    break;
                case "date_desc":
                    tarefas = tarefas.OrderByDescending(t => t.Data_criacao);
                    break;
                default:
                    tarefas = tarefas.OrderBy(t => t.Data_criacao);
                    break;
            }

            int pageSize = 5;
            return View(await PaginatedList<Tarefa>.CreateAsync(tarefas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Tarefa/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefa
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // GET: Tarefa/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tarefa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao,Data_criacao")] Tarefa tarefa)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(tarefa.Data_criacao.ToString()))
                {
                    tarefa.Data_criacao = DateTime.Now;
                }

                _context.Add(tarefa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tarefa);
        }

        // GET: Tarefa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefa.FindAsync(id);
            if (tarefa == null)
            {
                return NotFound();
            }
            return View(tarefa);
        }

        // POST: Tarefa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao,Data_criacao")] Tarefa tarefa)
        {
            if (id != tarefa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(tarefa.Data_criacao.ToString()))
                {
                    tarefa.Data_criacao = DateTime.Now;
                }

                try
                {
                    _context.Update(tarefa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarefaExists(tarefa.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tarefa);
        }

        // GET: Tarefa/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefa
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // POST: Tarefa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tarefa = await _context.Tarefa.FindAsync(id);
            _context.Tarefa.Remove(tarefa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarefaExists(int id)
        {
            return _context.Tarefa.Any(e => e.Id == id);
        }
    }
}
