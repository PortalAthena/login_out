using Login.Data;
using Login.Models;
using Microsoft.EntityFrameworkCore;

namespace Login.Services
{
    /// <summary>
    /// Serviço responsável por operações relacionadas
    /// às aplicações vinculadas ao usuário.
    /// 
    /// Camada: Service (Regra de Negócio / Acesso a Dados)
    /// 
    /// Observação:
    /// Este serviço depende do AparelhosPublicosDbContext
    /// e deve ser registrado como AddScoped no Program.cs.
    /// </summary>
    public class ListaService
    {
        private readonly AparelhosPublicosDbContext _apContext;

        /// <summary>
        /// Construtor com injeção de dependência do DbContext.
        /// O ciclo de vida recomendado é Scoped.
        /// </summary>
        /// <param name="apContext">Contexto do banco Aparelhos Públicos</param>
        public ListaService(AparelhosPublicosDbContext apContext)
        {
            _apContext = apContext;
        }

        /// <summary>
        /// Retorna a lista de aplicações vinculadas a um usuário específico.
        /// 
        /// Regras aplicadas:
        /// - Filtra pelo Id do usuário
        /// - Ignora registros com exclusão lógica (dataExclusaoLogica != null)
        /// - Realiza join com tabela de aplicações
        /// - Ordena pelo Id da aplicação
        /// 
        /// Método assíncrono para melhor escalabilidade.
        /// </summary>
        /// <param name="idUsuario">Identificador do usuário logado</param>
        /// <returns>Lista de aplicações associadas ao usuário</returns>
        public async Task<List<ListTrUsuarioAplicacaoModel>>
            ListarAplicacoesUsuarioAsync(int idUsuario)
        {
            var query = _apContext.hmg_TR_usuario_Aplicacao
                .Where(usuario =>
                    usuario.idusuario == idUsuario &&
                    usuario.dataExclusaoLogica == null)

                .Join(
                    _apContext.hmg_TA_Aplicacao,
                    usuarioApp => usuarioApp.idAplicacao,
                    aplicacao => aplicacao.idAplicacao,

                    (usuarioApp, aplicacao) => new ListTrUsuarioAplicacaoModel
                    {
                        nivelAplicacao = aplicacao.nivelAplicacao,
                        iconeAplicacao = aplicacao.iconeAplicacao,
                        idAplicacao = aplicacao.idAplicacao,
                        idusuario = usuarioApp.idusuario,
                       // nomeUsuario = usuarioApp.nomeUsuario,
                        dsc_Aplicacao = aplicacao.resenhaAplicacao,
                        tituloAplicacao = aplicacao.dscAplicacao,
                        linkAplicacao = aplicacao.linkAplicacao,
                        urlImagem = aplicacao.urlImagem
                    }
                )
                .OrderBy(aplicacao => aplicacao.tituloAplicacao);

            return await query.ToListAsync();
        }
    }
}
