using Login.Data;
using Login.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Login.Services
{
    public class CrudService
    {
        private readonly AparelhosPublicosDbContext _apContext;
        private readonly UsuarioDbContext _usuarioContext;

        /// <summary>
        /// Construtor com injeção de dependência do DbContext.
        /// O ciclo de vida recomendado é Scoped.
        /// </summary>
        /// <param name="apContext">Contexto do banco Aparelhos Públicos</param>
        public CrudService(AparelhosPublicosDbContext apContext, UsuarioDbContext usuarioContext)
        {
            _apContext = apContext;
            _usuarioContext = usuarioContext;
        }

        public void AlteraSenha(UsuarioModel model)
        {
            // 1. Busca o usuário pelo login enviado no modal
            var usuario = _usuarioContext.Usuario
                .FirstOrDefault(u => u.loginUsuario == model.loginUsuario);

            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado no sistema.");
            }

            // 2. Gera o hash da senha anterior digitada para conferir com a do banco
            var senhaAntigaCriptografada = CriaHash(model.senhaUsuario);

            if (usuario.securityHash != senhaAntigaCriptografada)
            {
                throw new Exception("A senha anterior está incorreta!");
            }

            // 3. Gera o hash da nova senha
            var novaSenhaCriptografada = CriaHash(model.novaSenha);

            // 4. Atualiza os dados do usuário
            usuario.emailUsuario = model.emailUsuario;
            usuario.telefoneUsuario = model.telefoneUsuario;
            usuario.securityHash = novaSenhaCriptografada; // Grava a nova senha criptografada

            // 5. IMPORTANTE: Como ele acabou de alterar a senha, desativa a flag de reset obrigatório
            usuario.resetaUsuario = "false";

            // 6. Salva as alterações no banco de dados
            _usuarioContext.Usuario.Update(usuario);
            _usuarioContext.SaveChanges();
        }




        //public void AlteraSenha(UsuarioModel model)
        //{
        //    var usuario = _usuarioContext.Usuario
        //        .FirstOrDefault(u => u.IdUsuario == model.IdUsuario);

        //    if (usuario == null)
        //        throw new Exception("Usuário não encontrado.");

        //    usuario.emailUsuario = model.emailUsuario;
        //    usuario.nomeUsuario = model.nomeUsuario;           

        //    usuario.emailUsuario = model.emailUsuario;
        //    usuario.nivelUsuario = model.nivelUsuario;


        //    if (!string.IsNullOrEmpty(model.senhaUsuario))
        //    {
        //        usuario.securityHash = CriaHash(model.senhaUsuario);
        //    }
        //    //_apContext.SaveChanges();
        //}

        //gerador de hash   
        public string CriaHash(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Converter a String para array de bytes, que é como a biblioteca trabalha.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            // Cria-se um StringBuilder para recompôr a string.
            StringBuilder sBuilder = new StringBuilder();
            // Loop para formatar cada byte como uma String em hexadecimal
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
