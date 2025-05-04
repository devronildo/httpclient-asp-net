using System.ComponentModel.DataAnnotations;

namespace ProjectHttpClient.Dto.Usuario
{
    public class UsuarioEdicaoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Digite o nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Digite o Sobrenome")]
        public string Sobrenome { get; set; }

        [Required(ErrorMessage = "Digite o Usuario")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Digite o Email")]
        public string Email { get; set; }
 
        public DateTime DataAlteracao { get; set; } = DateTime.Now;


    }
}
