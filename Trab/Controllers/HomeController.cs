using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trab.Data;
using Trab.Models;

namespace Trab.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        //Ir buscar as aulas do dia de hoje
        if (User.Identity.IsAuthenticated && User.IsInRole("Professor"))
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var userNameProf = User.Identity?.Name;
            var professorId = _context.Professores.FirstOrDefault(p => p.Email == userNameProf)?.Id;

            if (professorId.HasValue)
            {
                var todaysClasses = _context.Aulas
                    .Include(a => a.Turma)
                    .Where(a => a.DataAula == today && a.Turma.IdProf == professorId.Value)
                    .ToList();

                return View(todaysClasses);
            }
        }

        //Verificar se o usuário logado é um aluno
        if (!User.Identity.IsAuthenticated || !User.IsInRole("Aluno"))
        {
            return View();
        }

        //Descobrir o id na tabela "Aluno" do aluno logado
        var userName = User.Identity?.Name;
        int username_convertido = Convert.ToInt32(userName);

        var alunoId = _context.Alunos.FirstOrDefault(a => a.Al == username_convertido)?.Id;

        //Descobrir na tabela "AlunoTurma" as turmas do aluno logado e qual delas tem o PresençasAtivas = true
        var turmas = _context.AlunoTurmas.Where(at => at.IdAluno == alunoId).Include(at => at.Turma).ToList();

        var turmaAtiva = turmas.FirstOrDefault(t => t.Turma?.PresencasAtivas == true);

        if (turmaAtiva != null)
        {
            //Levar os estado da presença do aluno logado para a View caso seja true
            var presencas = _context.Presencas
                .Where(p => p.IdAluno == alunoId && p.IdTurma == turmaAtiva.Turma.Id).ToList();

            var presente = presencas.FirstOrDefault(p => p.Estado == true);

            ViewData["Cadeira"] = turmaAtiva.Turma.Cadeira;
            ViewData["Turma"] = turmaAtiva.Turma.Nome;
            ViewData["Presente"] = presente;
        }
        else
        {
            ViewData["MensagemSemAula"] = "Não há aulas com presenças ativas de momento.";
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
