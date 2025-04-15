using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Trab.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Trab.Models.Aluno> Alunos { get; set; }
    public DbSet<Trab.Models.AlunoTurma> AlunoTurmas { get; set; }
    public DbSet<Trab.Models.Presenca> Presencas { get; set; }
    public DbSet<Trab.Models.Professor> Professores { get; set; }
    public DbSet<Trab.Models.Turma> Turmas { get; set; }

    public DbSet<Trab.Models.Aula> Aulas { get; set; }

}
