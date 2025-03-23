using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trab.Data;
using Trab.Models;

namespace Trab.Controllers
{
    public class AlunosTurmaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlunosTurmaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AlunosTurma
        public async Task<IActionResult> Index(int? id)
        {
            var alunosDaTurma = _context.AlunoTurmas
            .Where(at => at.IdTurma == id)
            .Include(at => at.Aluno)  // Garante que os dados do Aluno sejam carregados
            .Include(at => at.Turma)  // Garante que os dados da Turma sejam carregados
            .ToList();

            var turma = alunosDaTurma.First().Turma;
            ViewBag.TurmaNome = turma.Nome;
            ViewBag.CadeiraNome = turma.Cadeira;

            return View(alunosDaTurma);
        }

        // GET: AlunosTurma/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alunoTurma = await _context.AlunoTurmas
                .Include(a => a.Aluno)
                .Include(a => a.Turma)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alunoTurma == null)
            {
                return NotFound();
            }

            return View(alunoTurma);
        }

        // GET: AlunosTurma/Create
        public IActionResult Create()
        {
            ViewData["IdAluno"] = new SelectList(_context.Alunos, "Id", "Email");
            ViewData["IdTurma"] = new SelectList(_context.Turmas, "Id", "Cadeira");
            return View();
        }

        // POST: AlunosTurma/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdTurma,IdAluno")] AlunoTurma alunoTurma)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alunoTurma);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAluno"] = new SelectList(_context.Alunos, "Id", "Email", alunoTurma.IdAluno);
            ViewData["IdTurma"] = new SelectList(_context.Turmas, "Id", "Cadeira", alunoTurma.IdTurma);
            return View(alunoTurma);
        }

        // GET: AlunosTurma/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alunoTurma = await _context.AlunoTurmas.FindAsync(id);
            if (alunoTurma == null)
            {
                return NotFound();
            }
            ViewData["IdAluno"] = new SelectList(_context.Alunos, "Id", "Email", alunoTurma.IdAluno);
            ViewData["IdTurma"] = new SelectList(_context.Turmas, "Id", "Cadeira", alunoTurma.IdTurma);
            return View(alunoTurma);
        }

        // POST: AlunosTurma/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdTurma,IdAluno")] AlunoTurma alunoTurma)
        {
            if (id != alunoTurma.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alunoTurma);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlunoTurmaExists(alunoTurma.Id))
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
            ViewData["IdAluno"] = new SelectList(_context.Alunos, "Id", "Email", alunoTurma.IdAluno);
            ViewData["IdTurma"] = new SelectList(_context.Turmas, "Id", "Cadeira", alunoTurma.IdTurma);
            return View(alunoTurma);
        }

        // GET: AlunosTurma/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alunoTurma = await _context.AlunoTurmas
                .Include(a => a.Aluno)
                .Include(a => a.Turma)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alunoTurma == null)
            {
                return NotFound();
            }

            return View(alunoTurma);
        }

        // POST: AlunosTurma/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alunoTurma = await _context.AlunoTurmas.FindAsync(id);
            if (alunoTurma != null)
            {
                _context.AlunoTurmas.Remove(alunoTurma);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlunoTurmaExists(int id)
        {
            return _context.AlunoTurmas.Any(e => e.Id == id);
        }
    }
}
