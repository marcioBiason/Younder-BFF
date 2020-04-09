using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Younder_BFF.v1.Models;

namespace Younder_BFF.v1.Models {

    /// <summary>
    /// Classe de Usuarios
    /// </summary>
    public class Usuario {

        /// <summary>
        /// Id do usuario
        /// </summary>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Data de Nascimento
        /// </summary>
        public DateTime DataNascimento { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Senha
        /// </summary>
        public string Senha { get; set; }

        /// <summary>
        /// Sexo do Usuario
        /// </summary>
        public string Sexo { get; set; }
    }
}