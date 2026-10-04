using System.ComponentModel.DataAnnotations;

namespace Login.Models
{
    public class UsuarioModel
    {
        [Key]
        public int IdUsuario { get; set; }
        public string? nomeUsuario { get; set; }
        public string? loginUsuario { get; set; }
        public string? nivelUsuario { get; set; }
        public int? COD_ORGAO { get; set; }
        public int? idNivelSieven { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? emailUsuario { get; set; }

        [Required(ErrorMessage = "Informe sua senha atual")]
        public string? senhaUsuario { get; set; }

        [Required(ErrorMessage = "Telefone é obrigatório")]
        public string? telefoneUsuario { get; set; }

        [Required(ErrorMessage = "Informe a nova senha")]
        [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres")]
        public string? novaSenha { get; set; }

        public string? securityHash { get; set; }

        public string? resetaUsuario { get; set; }

        [Required(ErrorMessage = "Confirme a nova senha")]
        [Compare("novaSenha", ErrorMessage = "As senhas não conferem")]
        [DataType(DataType.Password)]
        public string? ConfirmaSenha { get; set; }

        public int? idCienteSieven { get; set; }
       

    }
}




