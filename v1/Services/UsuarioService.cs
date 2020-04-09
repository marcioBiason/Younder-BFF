using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Younder_BFF.EF;
using Younder_BFF.v1.Models;

namespace Younder_BFF.v1.Services {
    /// <summary>
    /// Interface User Service
    /// </summary>
    public interface IUsuarioService {
        /// <summary>
        /// Add
        /// </summary>
        void Add<T> (T entity) where T : class;

        /// <summary>
        /// Update
        /// </summary>
        void Update<T> (T entity) where T : class;

        /// <summary>
        /// Delete
        /// </summary>
        void Delete<T> (T entity) where T : class;

        /// <summary>
        /// Save
        /// </summary>
        Task<bool> SaveChangesAsync ();

        /// <summary>
        /// Get Users
        /// </summary>
        Task<IEnumerable<object>> GetAllUsersAsync ();

        /// <summary>
        /// GetUserByEmail
        /// </summary>
        Task<Usuario[]> GetUserByNameAsync (string name);

        /// <summary>
        /// GetUserById
        /// </summary>
        Task<Usuario> GetUserByIdAsync (int id);

    }
    /// <summary>
    /// Classe UserService
    /// </summary>
    public class UsuarioService : IUsuarioService {

        /// <summary>
        /// IntegradorSaudeBDContext
        /// </summary>
        private readonly BancoDadosContext _context;

        /// <summary>
        /// IntegradorSaudeBDContext
        /// </summary>
        private IConfiguration _configuration;

        /// <summary>
        /// IntegradorSaudeBDContext
        /// </summary>
        public UsuarioService (BancoDadosContext context, IConfiguration configuration) {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _configuration = configuration;
        }

        /// <summary>
        /// Add
        /// </summary>
        public void Add<T> (T entity) where T : class {
            _context.Add (entity);
        }
        /// <summary>
        /// Update
        /// </summary>
        public void Update<T> (T entity) where T : class {
            _context.Update (entity);
        }
        /// <summary>
        /// Delete
        /// </summary>
        public void Delete<T> (T entity) where T : class {
            _context.Remove (entity);
        }

        /// <summary>
        ///Save
        /// </summary>
        public async Task<bool> SaveChangesAsync () {
            return (await _context.SaveChangesAsync ()) > 0;
        }

        /// <summary>
        ///GetUsers
        /// </summary>
        public async Task<IEnumerable<object>> GetAllUsersAsync () {
            var query = await (from u in _context.Usuarios select new {
                    userId = u.UserId,
                        nome = u.Nome,
                        dataNascimento = u.DataNascimento,
                        email = u.Email,
                        senha = u.Senha,
                        sexo = u.Sexo
                }).OrderBy (f => f.userId)
                .ToArrayAsync ();
            return query;
        }

        /// <summary>
        ///GetUserAsyncByID
        /// </summary>
        public async Task<Usuario[]> GetUserByNameAsync (string name) {
            IQueryable<Usuario> query = _context.Usuarios;

            query = query.AsNoTracking ()
                .Where (n => n.Nome.Contains (name.Trim ()))
                .OrderBy (n => n.UserId);

            return await query.ToArrayAsync ();
        }

        /// <summary>
        ///GetUserAsyncByID
        /// </summary>
        public async Task<Usuario> GetUserByIdAsync (int id) {
            IQueryable<Usuario> query = _context.Usuarios;

            query = query.AsNoTracking ()
                .Where (c => c.UserId == id);
            return await query.FirstOrDefaultAsync ();
        }
    }
}