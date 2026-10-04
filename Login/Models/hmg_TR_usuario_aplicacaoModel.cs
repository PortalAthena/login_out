using System.ComponentModel.DataAnnotations;

namespace Login.Models
{
    public class hmg_TR_usuario_aplicacaoModel
    {
        [Key]
        public int idTr_usuario_aplicacao { get; set; }
        public int? idAplicacao { get; set; }
        public int? idusuario { get; set; }
        public string? nomeUsuario { get; set; }
        public string? nivelUsuario { get; set; }
        public string? tokinUsuario { get; set; }
        public DateTime? dataAlteracaoUsuarioAplicacao { get; set; }
        public DateTime? dataExclusaoLogica { get; set; }
        public DateTime? dataCadastroUsuarioAplicacao { get; set; }
    }
}
