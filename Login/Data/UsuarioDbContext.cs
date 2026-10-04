using Login.Models;
using Microsoft.EntityFrameworkCore;

namespace Login.Data
{
    public class UsuarioDbContext : DbContext
    {
        public UsuarioDbContext(DbContextOptions<UsuarioDbContext> options)
            : base(options)
        {
        }
        // 🔵 MAPEIA A TABELA Usuario
        public DbSet<UsuarioModel> Usuario { get; set; }
         
    }
}
