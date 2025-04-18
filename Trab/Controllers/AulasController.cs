﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trab.Data;
using Trab.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Trab.Controllers
{
    public class AulasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AulasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Aulas
        public async Task<IActionResult> Index(string cadeira, string turma, DateTime? data)
        {
            var userName = User.Identity?.Name;
            var professorId = _context.Professores.FirstOrDefault(p => p.Email == userName)?.Id;


            var applicationDbContext = _context.Aulas.Include(a => a.Turma).AsQueryable();


            // Filtrar aulas por professor
            if (professorId.HasValue)
            {
                applicationDbContext = applicationDbContext.Where(a => a.Turma.IdProf == professorId.Value);
            }

            bool filtersApplied = false;

            if (!string.IsNullOrEmpty(cadeira))
            {
                applicationDbContext = applicationDbContext.Where(a => a.Turma.Cadeira == cadeira);
                filtersApplied = true;
            }
               
            if (!string.IsNullOrEmpty(turma))
            {
                applicationDbContext = applicationDbContext.Where(a => a.Turma.Nome == turma);
                filtersApplied = true;
            }


            if (data.HasValue)
            {
                var dataOnly = DateOnly.FromDateTime(data.Value);
                applicationDbContext = applicationDbContext.Where(a => a.DataAula == dataOnly);
            }
            else
            {
                // Sorting logic based on whether filters are applied
                var today = DateOnly.FromDateTime(DateTime.Today);

                if (filtersApplied)
                {
                    // If filtered by Turma or Cadeira: past Aulas first, ordered by date
                    applicationDbContext = applicationDbContext
                        .OrderBy(a => a.DataAula >= today) // Past first (true comes before false)
                        .ThenBy(a => a.DataAula);
                }
                else
                {
                    // If no filters: today's and future Aulas first, ordered by date
                    applicationDbContext = applicationDbContext
                        .OrderBy(a => a.DataAula < today) // Today and future first (false comes before true)
                        .ThenBy(a => a.DataAula);
                }
            }

            // ViewBag para os filtros - filtrado por professor
            ViewBag.Cadeiras = await _context.Turmas
                .Where(t => t.IdProf == professorId)
                .Select(t => t.Cadeira)
                .Distinct()
                .ToListAsync();

            ViewBag.Turmas = await _context.Turmas
                .Where(t => t.IdProf == professorId)
                .Select(t => t.Nome)
                .Distinct()
                .ToListAsync();



            return View(await applicationDbContext.ToListAsync());

        }

        // GET: Aulas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aula = await _context.Aulas
                .Include(a => a.Turma)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aula == null)
            {
                return NotFound();
            }

            return View(aula);
        }

        // GET: Aulas/Create
        public IActionResult Create()
        {
            ViewData["TurmaId"] = new SelectList(_context.Turmas, "Id", "Cadeira");
            return View();
        }

        // POST: Aulas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TurmaId,DataAula")] Aula aula)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TurmaId"] = new SelectList(_context.Turmas, "Id", "Cadeira", aula.TurmaId);
            return View(aula);
        }

        // GET: Aulas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aula = await _context.Aulas.FindAsync(id);
            if (aula == null)
            {
                return NotFound();
            }
            ViewData["TurmaId"] = new SelectList(_context.Turmas, "Id", "Cadeira", aula.TurmaId);
            return View(aula);
        }

        // POST: Aulas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TurmaId,DataAula")] Aula aula)
        {
            if (id != aula.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AulaExists(aula.Id))
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
            ViewData["TurmaId"] = new SelectList(_context.Turmas, "Id", "Cadeira", aula.TurmaId);
            return View(aula);
        }

        // GET: Aulas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aula = await _context.Aulas
                .Include(a => a.Turma)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aula == null)
            {
                return NotFound();
            }

            return View(aula);
        }

        // POST: Aulas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aula = await _context.Aulas.FindAsync(id);
            if (aula != null)
            {
                _context.Aulas.Remove(aula);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //Ativar Presenças
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> AtivarPresencas(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            //Mudar o atributo PresencasAtivas para true
            var turma = await _context.Turmas.FindAsync(id);

            if (turma == null)
            {
                return NotFound();
            }

            if (turma.PresencasAtivas == true)
            {

                turma.PresencasAtivas = false;
                _context.Update(turma);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                turma.PresencasAtivas = true;
                _context.Update(turma);
                await _context.SaveChangesAsync();

                //Guardar todos os alunos da turma na tabela presencas através da tabela AlunosTurma com o estado de falta
                var today = DateTime.Now.Date; 
                var AlunosGuardados = await _context.Presencas
                    .Where(p => p.IdTurma == turma.Id && p.Data.Date == today)
                    .AnyAsync();

                if (!AlunosGuardados)
                {
                    var alunos = await _context.AlunoTurmas.Where(at => at.IdTurma == turma.Id).ToListAsync();
                    foreach (var aluno in alunos)
                    {
                        Presenca presenca = new Presenca
                        {
                            IdAluno = aluno.IdAluno,
                            IdTurma = turma.Id,
                            Data = DateTime.Now,
                            Estado = false
                        };
                        _context.Add(presenca);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

        }


        private bool AulaExists(int id)
        {
            return _context.Aulas.Any(e => e.Id == id);
        }
    }
}
