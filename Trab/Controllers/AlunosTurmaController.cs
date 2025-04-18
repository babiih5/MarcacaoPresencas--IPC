﻿using System;
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

        // GET: AlunosTurma/Index
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alunosDaTurma = await _context.AlunoTurmas
                .Where(at => at.IdTurma == id)
                .Include(at => at.Aluno)
                .Include(at => at.Turma)
                .ToListAsync();

            var turma = _context.Turmas.FirstOrDefault(t => t.Id == id);
            if (turma == null)
            {
                return NotFound();
            }
            ViewBag.TurmaNome = turma.Nome;
            ViewBag.CadeiraNome = turma.Cadeira;
            ViewBag.IdTurma = turma.Id;

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
        public IActionResult Create(int idTurma)
        {
            Console.WriteLine($"IdTurma recebido: {idTurma}"); // Para debug

            // Passa a turma automaticamente
            ViewBag.IdTurma = idTurma;

            // Carrega a lista de alunos no formato correto
            ViewBag.IdAluno = new SelectList(_context.Alunos, "Al", "Nome");

            return View();
        }


        // POST: AlunosTurma/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTurma,IdAluno")] AlunoTurma alunoTurma)
        {
            var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.Al == alunoTurma.IdAluno);
            Console.WriteLine($"IdTurma recebido: {alunoTurma.IdAluno}");


            if (ModelState.IsValid)
            {
                alunoTurma.IdAluno = aluno.Id;
                _context.Add(alunoTurma);
                await _context.SaveChangesAsync();
                Console.WriteLine("AlunoTurma adicionado com sucesso!");

                return RedirectToAction(nameof(Index), new { id = alunoTurma.IdTurma });
            }

            ViewBag.IdAluno = new SelectList(_context.Alunos, "Al", "Nome", alunoTurma.IdAluno);
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
