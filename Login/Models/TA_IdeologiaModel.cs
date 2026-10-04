using System.ComponentModel.DataAnnotations;

namespace Login.Models
{
    public class TA_IdeologiaModel
    {
        [Key]
        public int COD_IDEOLOGIA { get; set; }
        public string? dsc_Ideologia { get; set; }
        public DateTime? dataExclusaoLogicaIdeologia { get; set; }


        

    }
}
