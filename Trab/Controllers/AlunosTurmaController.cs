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

            // Store the turma ID in ViewBag for the "Back" button
            ViewBag.IdTurma = alunoTurma.IdTurma;
            ViewBag.TurmaNome = alunoTurma.Turma?.Nome;
            ViewBag.CadeiraNome = alunoTurma.Turma?.Cadeira;

            // Pass the Aluno object to the view instead of AlunoTurma
            return View(alunoTurma.Aluno);
        }



        // GET: AlunosTurma/Create
        public IActionResult Create(int idTurma)
        {
            Console.WriteLine($"IdTurma recebido: {idTurma}"); // Para debug

            // Passa a turma automaticamente
            ViewBag.IdTurma = idTurma;

            // Carrega a lista de alunos para ambas as direções
            ViewBag.IdAluno = new SelectList(_context.Alunos, "Al", "Nome");

            // Lista de alunos para JSON
            ViewBag.AlunosJson = System.Text.Json.JsonSerializer.Serialize(
                _context.Alunos.Select(a => new { a.Al, a.Nome }).ToList()
            );

            return View();
        }


        // POST: AlunosTurma/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create([Bind("IdTurma,IdAluno")] AlunoTurma alunoTurma)
        {
            var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.Al == alunoTurma.IdAluno);
            Console.WriteLine($"IdTurma recebido: {alunoTurma.IdAluno}");

            if (aluno == null)
            {
                ModelState.AddModelError("IdAluno", "Aluno não encontrado com este número mecanográfico.");
                ViewBag.IdTurma = alunoTurma.IdTurma;
                ViewBag.IdAluno = new SelectList(_context.Alunos, "Al", "Nome", alunoTurma.IdAluno);
                ViewBag.AlunosJson = System.Text.Json.JsonSerializer.Serialize(
                    _context.Alunos.Select(a => new { a.Al, a.Nome }).ToList()
                );
                return View(alunoTurma);
            }

            // Verificar se o aluno já está inscrito nesta turma
            bool alunoJaInscrito = await _context.AlunoTurmas
                .AnyAsync(at => at.IdTurma == alunoTurma.IdTurma && at.IdAluno == aluno.Id);

            if (alunoJaInscrito)
            {
                ModelState.AddModelError("", "Este aluno já está inscrito nesta turma.");
                ViewBag.IdTurma = alunoTurma.IdTurma;
                ViewBag.IdAluno = new SelectList(_context.Alunos, "Al", "Nome", alunoTurma.IdAluno);
                ViewBag.AlunosJson = System.Text.Json.JsonSerializer.Serialize(
                    _context.Alunos.Select(a => new { a.Al, a.Nome }).ToList()
                );
                return View(alunoTurma);
            }

            if (ModelState.IsValid)
            {
                alunoTurma.IdAluno = aluno.Id;
                _context.Add(alunoTurma);
                await _context.SaveChangesAsync();
                Console.WriteLine("AlunoTurma adicionado com sucesso!");

                return RedirectToAction(nameof(Index), new { id = alunoTurma.IdTurma });
            }

            ViewBag.IdAluno = new SelectList(_context.Alunos, "Al", "Nome", alunoTurma.IdAluno);
            ViewBag.AlunosJson = System.Text.Json.JsonSerializer.Serialize(
                _context.Alunos.Select(a => new { a.Al, a.Nome }).ToList()
            );
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

       

        // GET: AlunosTurma/DeleteDirect/5
        public async Task<IActionResult> DeleteDirect(int? id)
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

            // Store the turma ID before removing the record
            int turmaId = alunoTurma.IdTurma;

            _context.AlunoTurmas.Remove(alunoTurma);
            await _context.SaveChangesAsync();

            // Redirect back to the list of students for this specific turma
            return RedirectToAction(nameof(Index), new { id = turmaId });
        }


        private bool AlunoTurmaExists(int id)
        {
            return _context.AlunoTurmas.Any(e => e.Id == id);
        }
    }
}
