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
    public class PresencasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PresencasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Presencas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Presencas.Include(p => p.Aluno).Include(p => p.Turma);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Presencas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presenca = await _context.Presencas
                .Include(p => p.Aluno)
                .Include(p => p.Turma)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (presenca == null)
            {
                return NotFound();
            }

            return View(presenca);
        }

        // GET: Presencas/Create
        public IActionResult Create()
        {
            ViewData["IdAluno"] = new SelectList(_context.Alunos, "Id", "Email");
            ViewData["IdTurma"] = new SelectList(_context.Turmas, "Id", "Cadeira");
            return View();
        }

        // POST: Presencas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdAluno,IdTurma,Data,Estado")] Presenca presenca)
        {
            //Guardar todos os alunos da turma na tabela presença através da tabela AlunosTurma com o estado de falta
            if (ModelState.IsValid)
            {
                var alunos = _context.AlunoTurmas.Where(at => at.IdTurma == presenca.IdTurma).ToList();
                foreach (var aluno in alunos)
                {
                    presenca.IdAluno = aluno.IdAluno;
                    presenca.Estado = false;
                    _context.Add(presenca);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }



            return View(presenca);
        }

        // GET: Presencas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presenca = await _context.Presencas.FindAsync(id);
            if (presenca == null)
            {
                return NotFound();
            }
            ViewData["IdAluno"] = new SelectList(_context.Alunos, "Id", "Email", presenca.IdAluno);
            ViewData["IdTurma"] = new SelectList(_context.Turmas, "Id", "Cadeira", presenca.IdTurma);
            return View(presenca);
        }

        // POST: Presencas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdAluno,IdTurma,Data,Estado")] Presenca presenca)
        {
            if (id != presenca.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(presenca);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PresencaExists(presenca.Id))
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
            ViewData["IdAluno"] = new SelectList(_context.Alunos, "Id", "Email", presenca.IdAluno);
            ViewData["IdTurma"] = new SelectList(_context.Turmas, "Id", "Cadeira", presenca.IdTurma);
            return View(presenca);
        }

        // GET: Presencas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presenca = await _context.Presencas
                .Include(p => p.Aluno)
                .Include(p => p.Turma)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (presenca == null)
            {
                return NotFound();
            }

            return View(presenca);
        }

        // POST: Presencas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var presenca = await _context.Presencas.FindAsync(id);
            if (presenca != null)
            {
                _context.Presencas.Remove(presenca);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Marcar a presença do aluno logado
        public async Task<IActionResult> MarcarPresenca(int id)
        {
            //Encontrar o aluno logado
            var userName = User.Identity?.Name;
            int username_convertido = Convert.ToInt32(userName);
            var alunoId = _context.Alunos.FirstOrDefault(a => a.Al == username_convertido)?.Id;

            //Descobrir na tabela "AlunoTurma" as turmas do aluno logado e qual delas tem o PresençasAtivas = true
            var turmas = _context.AlunoTurmas.Where(at => at.IdAluno == alunoId).Include(at => at.Turma).ToList();
            var turmaAtiva = turmas.FirstOrDefault(t => t.Turma.PresencasAtivas);


            if (turmaAtiva == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //Encontrar a presença do aluno logado na turma
            var presenca = await _context.Presencas.FirstOrDefaultAsync(p => p.IdAluno == alunoId && p.IdTurma == turmaAtiva.Turma.Id);
           Console.WriteLine("PRES: " + presenca.Id);
            if (presenca != null) {
                Console.WriteLine("ESTOU AQUI");
                presenca.Estado = true;
                _context.Update(presenca);
                await _context.SaveChangesAsync();
            }


            //Redirecionar para o Index do Home
            return RedirectToAction("Index", "Home");
        }

        private bool PresencaExists(int id)
        {
            return _context.Presencas.Any(e => e.Id == id);
        }
    }
}
