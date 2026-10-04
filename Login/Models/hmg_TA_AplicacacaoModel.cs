using System.ComponentModel.DataAnnotations;

namespace Login.Models
{
    public class hmg_TA_AplicacacaoModel
    {

        [Key]
        public int idAplicacao { get; set; }
        public string? dscAplicacao { get; set; }
        public DateTime? dataExclusaoLogica { get; set; }
        public string? resenhaAplicacao { get; set; }
        public string? linkAplicacao { get; set; }
        public string? iconeAplicacao { get; set; }
        public string? nivelAplicacao { get; set; }
        public string? urlImagem { get; set; }       
    }
}
