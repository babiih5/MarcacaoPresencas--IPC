using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Trab.Models
{
    public class AlunoTurma
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Turma")]
        public int IdTurma { get; set; }
        public Turma? Turma { get; set; }

        [ForeignKey("Aluno")]
        public int IdAluno { get; set; }
        public Aluno? Aluno { get; set; }
    }
}
