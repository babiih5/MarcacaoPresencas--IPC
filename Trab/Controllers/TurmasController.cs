using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trab.Data;
using Trab.Models;

namespace Trab.Controllers
{
    public class TurmasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TurmasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Turmas
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Turmas.Include(t => t.Professor);
            var userName = User.Identity?.Name;
            var professorid = _context.Professores.FirstOrDefault(p => p.Email == userName)?.Id;
            var TurmasFiltradas = applicationDbContext.Where(t => t.IdProf == professorid);

            return View(await TurmasFiltradas.ToListAsync());
        }

        // GET: Turmas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turma = await _context.Turmas
                .Include(t => t.Professor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (turma == null)
            {
                return NotFound();
            }

            return View(turma);
        }

        [Authorize(Roles = "Professor")]
        // GET: Turmas/Create
        public IActionResult Create()
        {
            ViewData["IdProf"] = new SelectList(_context.Professores, "Id", "Email");
            return View();
        }

        [Authorize(Roles = "Professor")]
        // POST: Turmas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Cadeira,Nome,DiaSemana,HorarioInicio,HorasFim,Sala,IdProf")] Turma turma, string horarioInicio, string horasFim)
        {
            if (ModelState.IsValid)
            {
                var userName = User.Identity?.Name;
                var professor = _context.Professores.FirstOrDefault(p => p.Email == userName);

                if (professor == null) {
                    return NotFound();
                }
                turma.IdProf = professor.Id;
                turma.HorarioInicio = TimeSpan.Parse(horarioInicio);
                turma.HorasFim = TimeSpan.Parse(horasFim);

                _context.Add(turma);
                await _context.SaveChangesAsync();


                //Criar aulas na tabela Aulas no dia da semana correspondente até uma certa data (ex: 06/06/2025)
                DateOnly dataInicio = DateOnly.FromDateTime(DateTime.Now);
                DateOnly dataFim = new DateOnly(2025, 6, 6);
                DateOnly dataAtual = dataInicio;

                var dayMapping = new Dictionary<int, string>
                {
                    { 1, "Segunda" },
                    { 2, "Terça" },
                    { 3, "Quarta" },
                    { 4, "Quinta" },
                    { 5, "Sexta" }
                };

                while (dataAtual <= dataFim)
                {
                    
                    //Criar aula para o dia da semana correspondente
                    if (dayMapping.FirstOrDefault(x => x.Value == turma.DiaSemana).Key == (int)dataAtual.DayOfWeek)
                    {
                        Aula aula = new Aula
                        {
                            TurmaId = turma.Id,
                            DataAula = dataAtual,
                            
                        };
                        _context.Aulas.Add(aula);
                    }

                    dataAtual = dataAtual.AddDays(1);
              
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["IdProf"] = new SelectList(_context.Professores, "Id", "Email", turma.IdProf);
            return View(turma);
        }

        // GET: Turmas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turma = await _context.Turmas.FindAsync(id);
            if (turma == null)
            {
                return NotFound();
            }
            ViewData["IdProf"] = new SelectList(_context.Professores, "Id", "Email", turma.IdProf);
            return View(turma);
        }

        // POST: Turmas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cadeira,Nome,DiaSemana,HorarioInicio,HorasFim,Sala,IdProf")] Turma turma)
        {
            if (id != turma.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(turma);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TurmaExists(turma.Id))
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
            ViewData["IdProf"] = new SelectList(_context.Professores, "Id", "Email", turma.IdProf);
            return View(turma);
        }

        // GET: Turmas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turma = await _context.Turmas
                .Include(t => t.Professor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (turma == null)
            {
                return NotFound();
            }

            return View(turma);
        }

        // POST: Turmas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var turma = await _context.Turmas.FindAsync(id);
            if (turma != null)
            {
                _context.Turmas.Remove(turma);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Ativar Presenças
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> AtivarPresencas(int? id)
        {
       
            if (id== null)
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
                //Guardar todos os alunos da turma na tabela presencas através da tabela AlunosTurma com o estado de falta
                var today = DateTime.Now.Date; // Get just the date part without time
                var AlunosGuardados = await _context.Presencas
                    .Where(p => p.IdTurma == turma.Id && p.Data.Date == today)
                    .AnyAsync();

                // Only add new records if there are no existing records for this turma today
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


        private bool TurmaExists(int id)
        {
            return _context.Turmas.Any(e => e.Id == id);
        }

      

    }


}
