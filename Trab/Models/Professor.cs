using System.ComponentModel.DataAnnotations;

namespace Trab.Models
{
    public class Professor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Email { get; set; }
        public ICollection<Turma> Turmas { get; set; }
    }
}
