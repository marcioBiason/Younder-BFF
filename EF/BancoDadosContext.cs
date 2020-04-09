using Microsoft.EntityFrameworkCore;
using Younder_BFF.v1.Models;

namespace Younder_BFF.EF {
    /// <summary>
    /// DbContext
    /// </summary>
    public class BancoDadosContext : DbContext {
        /// <summary>
        /// DbContext
        /// </summary>
        public BancoDadosContext (DbContextOptions options) : base (options) { }

        /// <summary>
        /// Tabela que contem os Usuarios;
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; }
    }
}