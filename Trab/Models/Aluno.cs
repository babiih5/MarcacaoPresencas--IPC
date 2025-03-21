using System.ComponentModel.DataAnnotations;

namespace Trab.Models
{
    public class Aluno
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public int Al { get; set; }

        [Required]
        public string Email { get; set; }
        public ICollection<AlunoTurma> AlunoTurmas { get; set; }
    }
}
