using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Trab.Models
{
    public class Presenca
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Aluno")]
        public int IdAluno { get; set; }
        public Aluno? Aluno { get; set; }

        [ForeignKey("Turma")]
        public int IdTurma { get; set; }
        public Turma? Turma { get; set; }

        public DateTime Data { get; set; }
        public bool Estado { get; set; } // presente ou falta
    }
}
