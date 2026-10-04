using Login.Models;
using Microsoft.EntityFrameworkCore;

public class AparelhosPublicosDbContext : DbContext
{
    public AparelhosPublicosDbContext(DbContextOptions<AparelhosPublicosDbContext> options)
        : base(options) { }


    // 🔵 MAPEIA A TABELAS DO BANCO APARELHOSPUBLICOS
    public DbSet<hmg_TA_AplicacacaoModel> hmg_TA_Aplicacao { get; set; }
    public DbSet<hmg_TR_usuario_aplicacaoModel> hmg_TR_usuario_Aplicacao { get; set; }    

}
