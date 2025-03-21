using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Trab.Models
{
    public class Turma
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Cadeira { get; set; }

        [Required]
        public string? Nome { get; set; }

        [Required]
        public TimeSpan? HorarioInicio { get; set; }

        [Required]
        public TimeSpan? HorasFim { get; set; }

        [ForeignKey("Professor")]
        public int IdProf { get; set; }
        public Professor? Professor { get; set; }
        public ICollection<AlunoTurma>? AlunoTurmas { get; set; }
        public ICollection<Presenca>? Presencas { get; set; }
    }
}
