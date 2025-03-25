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
        //Descobrir o id na tabela "Aluno" do aluno logado
        var userName = User.Identity?.Name;
        int username_convertido = Convert.ToInt32(userName);

        var alunoId = _context.Alunos.FirstOrDefault(a => a.Al == username_convertido)?.Id;

        //Descobrir na tabela "AlunoTurma" as turmas do aluno logado e qual delas tem o PresençasAtivas = true
        var turmas = _context.AlunoTurmas.Where(at => at.IdAluno == alunoId).Include(at => at.Turma).ToList();

        var turmaAtiva = turmas.FirstOrDefault(t => t.Turma.PresencasAtivas);

        if (turmaAtiva != null)
        {
            ViewData["Cadeira"] = turmaAtiva.Turma.Cadeira;
            ViewData["Turma"] = turmaAtiva.Turma.Nome;
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
