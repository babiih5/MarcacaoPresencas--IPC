using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trab.Models
{
    public class Aula
    {

        [Key]
        public int Id { get; set; }

        [ForeignKey("Turma")]
        public int TurmaId { get; set; }
        public Turma? Turma { get; set; }

        [Required]
        public DateOnly DataAula { get; set; }



    }
}
