using System.ComponentModel.DataAnnotations;

namespace Login.Models
{
    public class TP_EventoSiModel
    {
        [Key]
        public int idEvento { get; set; }
        public Nullable<int> EventoId { get; set; }
        public Nullable<System.DateTime> InicioEvento { get; set; }
        public Nullable<System.DateTime> fimEvento { get; set; }
        public Nullable<int> idGrupoOrganizador { get; set; }
        public Nullable<int> idOrganizador { get; set; }
        public Nullable<System.DateTime> dataCadastroEvento { get; set; }
        public Nullable<int> idCadastrador { get; set; }
        public Nullable<System.DateTime> dataExclusaoLogica { get; set; }
        public string latitudeString { get; set; }
        public string longitudeString { get; set; }
        public string nomeEvento { get; set; }
        public string dscEvento { get; set; }
        public string localEvento { get; set; }
        public Nullable<int> COD_UF { get; set; }
        public Nullable<int> COD_CIDADE { get; set; }
        public Nullable<int> COD_BAIRRO { get; set; }
        public string DSC_ENDERECO { get; set; }
        public string colorEvento { get; set; }
        public Nullable<bool> fullDayEvent { get; set; }
        public string dscEnderecoEvento { get; set; }
        public Nullable<int> COD_ESTIMATIVA { get; set; }
        public Nullable<int> COD_CATEGORIA_EVENTO { get; set; }
        public Nullable<int> COD_PAIS { get; set; }
        public Nullable<int> cod_Movimento { get; set; }
        public Nullable<int> divulgaAgencia { get; set; }
        public Nullable<int> divulgaPlantao { get; set; }
        public Nullable<int> caravana { get; set; }
        public Nullable<int> PublicoEfetivo { get; set; }
        public byte[] imgEvento { get; set; }
        public string midiaSocialEvento { get; set; }
        public Nullable<int> COD_SINDICATO { get; set; }
        public string responsavel { get; set; }
        public Nullable<int> COD_CONCENTRACAO { get; set; }
        public Nullable<int> PublicoPrevisto { get; set; }
        public Nullable<int> COD_ESTRUTURA { get; set; }
        public string obs_Evento { get; set; }
        public Nullable<int> COD_FONTE { get; set; }
        public Nullable<int> COD_PARTIDO { get; set; }
        public Nullable<int> COD_IDEOLOGIA { get; set; }
        public Nullable<int> COD_ORGANIZACAO { get; set; }
        public Nullable<int> COD_NIVELFEDERATIVO { get; set; }
        public Nullable<int> COD_DESFECHO { get; set; }
        public Nullable<int> COD_LOCALEVENTO { get; set; }
        public Nullable<int> COD_PosicaoIdeologica { get; set; }
        public Nullable<int> idEspecial { get; set; }
        public Nullable<int> COD_OCORRENCIA { get; set; }
        public Nullable<int> COD_FLAGRANTE { get; set; }
        public string numeroOcorrenciaEvento { get; set; }
        public string descricaoEvento { get; set; }
        public Nullable<int> idPubPrivado { get; set; }
        public Nullable<int> idRisco { get; set; }
        public Nullable<System.DateTime> dataAlteracaoEvento { get; set; }
        public string alterador { get; set; }
    }
}
