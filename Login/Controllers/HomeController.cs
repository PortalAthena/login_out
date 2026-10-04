using Login.Data;
using Login.Models;
using Login.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Login.Controllers
{
    /// <summary>
    /// Controller principal responsável por:
    /// - Exibir tela inicial
    /// - Realizar autenticação
    /// - Exibir menu do usuário autenticado
    /// - Realizar logout
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UsuarioDbContext _context;
        private readonly ListaService _listaService;
        private readonly CrudService _crudService; // 👈 Estancia a rotina de CRUD Gerais

        // Classe responsável por gerar hash da senha
        private readonly Autenticador _autHash = new Autenticador();

        /// <summary>
        /// Construtor com injeção de dependência.
        /// Recebe:
        /// - Logger
        /// - Contexto do banco Administracao (usuários)
        /// - Service responsável pelas listagens do módulo
        /// </summary>
        public HomeController(
            ILogger<HomeController> logger,
            UsuarioDbContext context,
            ListaService listaService,
            CrudService crudService)
        {
            _logger = logger;
            _context = context;
            _listaService = listaService;
            _crudService = crudService;
        }

        /// <summary>
        /// Tela inicial do sistema (pública).
        /// </summary>
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Menu");
            }

            return View();
        }


        /// <summary>
        /// Menu do usuário autenticado.
        /// Recupera o IdUsuario salvo no cookie (Claim)
        /// e busca as aplicações permitidas para o usuário.   
        /// </summary>
        [Authorize]
        public async Task<IActionResult> Menu()
        {
            var claim = User.FindFirst("IdUsuario");

            if (claim == null)
                return RedirectToAction("Login", "Conta");

            int idUsuario = int.Parse(claim.Value);

            var lista = await _listaService.ListarAplicacoesUsuarioAsync(idUsuario);

            return View(lista);
        }

        /// <summary>
        /// Realiza autenticação do usuário.
        /// - Valida login e senha
        /// - Gera Claims
        /// - Cria cookie de autenticação
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Login(string Login, string Password, string returnUrl = null)
        {
            var senhaCripta = _autHash.CriaHash(Password);

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.loginUsuario == Login && u.securityHash == senhaCripta);

            if (usuario == null)
            {
                TempData["Aviso"] = "Usuário ou senha inválidos...";
                return RedirectToAction("Index", "Home");
            }

            // 🔴 NOVO: Verifica se o usuário precisa resetar a senha
            // 🔴 ALTERE ESTA PARTE NO SEU MÉTODO LOGIN
            if (usuario.resetaUsuario.Equals("true") )
            {
                TempData["UsuarioParaResetar"] = usuario.loginUsuario;
                TempData["AbrirModalResetar"] = true; // Mantém como bool aqui
                return RedirectToAction("Index", "Home");
            }

            // Restante do código de login normal...
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario.nomeUsuario),
        new Claim("NivelUsuario", usuario.nivelUsuario.ToString()),
        new Claim("IdUsuario", usuario.IdUsuario.ToString()),
        new Claim("OrgaoUsuario", usuario.COD_ORGAO.ToString()),
         new Claim("idNivelSieven", usuario.idNivelSieven.ToString()),
        // new Claim("idCienteSieven", usuario.idCienteSieven.ToString())
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = false };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            if (usuario.nivelUsuario == "98989")
                return RedirectToAction("Menu");

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Realiza logout do usuário.
        /// Remove o cookie de autenticação.
        /// </summary>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarSenha(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return Json(new { success = false, message = "Por favor, informe o seu login de acesso." });
            }

            // 1. Busca o usuário no banco pelo Login (ajuste conforme o seu contexto/tabela)
            var usuario = _context.Usuario.FirstOrDefault(u => u.loginUsuario == login);

            if (usuario == null)
            {
                // Por segurança, muitas vezes é bom retornar sucesso genérico para não expor se o login existe, 
                // mas você pode retornar uma mensagem amigável se preferir:
                return Json(new { success = true, message = "Se o login existir, as instruções foram enviadas." + usuario.nomeUsuario });
            }

            // 2. Verifica se o usuário possui um número de celular cadastrado
            if (string.IsNullOrWhiteSpace(usuario.telefoneUsuario))
            {
                return Json(new { success = false, message = "Não há um telefone celular cadastrado para este usuário. Contate o suporte." });
            }

            // 3. Gera uma senha temporária ou código de recuperação
            string novaSenhaTemporaria = GerarSenhaTemporaria(); // Crie um método auxiliar para gerar ou use um token

            // Opcional: Salve a nova senha temporária criptografada/atualizada no banco para este usuário
            usuario.securityHash = CriaHash(novaSenhaTemporaria); // Exemplo de hash
            usuario.resetaUsuario = "true"; // Força ele a trocar no próximo login se desejar
            _context.SaveChanges();

            // 4. Prepara a mensagem de SMS
            string mensagemSms = $"Portal Athena: Sua senha temporária de recuperacao é: {novaSenhaTemporaria}. Acesse https://si.ssp.df.gov.br e use-a para entrar.Altere sua senha após o login.";

            // 5. Dispara o SMS usando o seu método SmsDev
            string resultadoSms = await EnviarSmsDevAsync(usuario.telefoneUsuario, mensagemSms);

            if (resultadoSms == "OK")
            {
                return Json(new { success = true, message = "Instruções enviadas com sucesso para o seu celular cadastrado!" });
            }
            else
            {
                // Retorna o erro retornado pela API da SmsDev para depuração
                return Json(new { success = false, message = $"Erro ao enviar SMS: {resultadoSms}" });
            }
        }
        /// Tela para alterar senha (pode ser acessada mesmo sem autenticação,mas a senha deve ser lembrada par aalteração)
        public IActionResult Changeyourpassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Menu");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AlteraUsuario([FromBody] UsuarioModel model) // 👈 ADICIONE O [FromBody] AQUI
        {
            // Se o model continuar chegando nulo, tratamos para evitar exceção
            if (model == null)
                return Json(new { success = false, message = "Nenhum dado foi recebido pelo servidor." });

            if (string.IsNullOrWhiteSpace(model.emailUsuario))
                return Json(new { success = false, message = "Email é obrigatório!" });

            if (string.IsNullOrWhiteSpace(model.telefoneUsuario))
                return Json(new { success = false, message = "Telefone é obrigatório!" });

            if (string.IsNullOrWhiteSpace(model.senhaUsuario))
                return Json(new { success = false, message = "Senha Anterior é obrigatória!" });

            if (string.IsNullOrWhiteSpace(model.novaSenha))
                return Json(new { success = false, message = "Crie uma nova senha!" });

            if (!ModelState.IsValid)
            {
                var erro = ModelState.Values
                           .SelectMany(v => v.Errors)
                           .FirstOrDefault()?.ErrorMessage;

                return Json(new { success = false, message = erro });
            }

            try
            {
                _crudService.AlteraSenha(model);

                // Ajustei a mensagem de sucesso para refletir a alteração de senha
                return Json(new { success = true, message = "Senha alterada com sucesso!!!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private string GerarSenhaTemporaria(int tamanho = 8)
        {
            // Caracteres permitidos na senha temporária
            const string letrasMaiusculas = "ABCDEFGHJKLMNPQRSTUVWXYZ"; // Removidas letras muito parecidas como I, O para evitar confusão
            const string letrasMinusculas = "abcdefghijkmnpqrstuvwxyz"; // Removida a letra 'l'
            const string numeros = "23456789";                         // Removidos 0 e 1 para evitar confusão com O e I
            const string especiais = "@#$%*!";                          // Caracteres especiais seguros para URL/SMS

            // Garante pelo menos um de cada tipo para atender a políticas rígidas de segurança
            char[] senha = new char[tamanho];

            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[4];

                // Função auxiliar interna para gerar um índice seguro aleatório
                int ObterIndiceAleatorio(int maximo)
                {
                    rng.GetBytes(buffer);
                    uint valor = BitConverter.ToUInt32(buffer, 0);
                    return (int)(valor % (uint)maximo);
                }

                // Preenche obrigatoriamente com os tipos básicos (se o tamanho permitir)
                int index = 0;
                if (tamanho >= 4)
                {
                    senha[index++] = letrasMaiusculas[ObterIndiceAleatorio(letrasMaiusculas.Length)];
                    senha[index++] = letrasMinusculas[ObterIndiceAleatorio(letrasMinusculas.Length)];
                    senha[index++] = numeros[ObterIndiceAleatorio(numeros.Length)];
                    senha[index++] = especiais[ObterIndiceAleatorio(especiais.Length)];
                }

                // Preenche o restante do tamanho com caracteres de todos os conjuntos misturados
                string todosOsCaracteres = letrasMaiusculas + letrasMinusculas + numeros + especiais;
                for (int i = index; i < tamanho; i++)
                {
                    senha[i] = todosOsCaracteres[ObterIndiceAleatorio(todosOsCaracteres.Length)];
                }

                // Embaralha os caracteres para que a senha não comece sempre com o padrão fixo (Maiúscula, Minúscula, Número, Especial)
                for (int i = senha.Length - 1; i > 0; i--)
                {
                    int j = ObterIndiceAleatorio(i + 1);
                    char temp = senha[i];
                    senha[i] = senha[j];
                    senha[j] = temp;
                }
            }

            return new string(senha);
        }

        // Coloque o seu Token do smsDEV aqui (ou leia do Web.config)
        private readonly string _smsDevToken = "94X6PJ37UUS6IHJZO9AUU2DRY0X1Z3JD1WH7MF675GOEQLHXIN6MLPS75U7ONMKWR9D5XP6TQY8LOVOGO3NKTCL7ROW92Q3XQ2KNBOQ9ME7AKHOTXU9GY63IY9FZG9RH";

        private async Task<string> EnviarSmsDevAsync(string numeroDestino, string mensagem)
        {
            string numeroLimpo = System.Text.RegularExpressions.Regex.Replace(numeroDestino, @"[^\d]", "");
            string apiUrl = "https://api.smsdev.com.br/v1/send";

            using (var client = new System.Net.Http.HttpClient())
            {
                var campos = new System.Collections.Generic.Dictionary<string, string>
        {
            { "key", _smsDevToken },
            { "type", "9" }, // Se persistir o erro com saldo 10, experimente mudar para "1" aqui
            { "number", numeroLimpo },
            { "msg", mensagem }
        };

                var conteudo = new System.Net.Http.FormUrlEncodedContent(campos);

                try
                {
                    var response = await client.PostAsync(apiUrl, conteudo);

                    if (response.IsSuccessStatusCode)
                    {
                        string respostaString = await response.Content.ReadAsStringAsync();

                        if (respostaString.Contains("\"situacao\":\"OK\"") || respostaString.Contains("OK"))
                        {
                            return "OK";
                        }

                        // Retorna a mensagem de erro específica do painel (ex: chave inválida, rota bloqueada, etc.)
                        return respostaString;
                    }
                    return "Erro de comunicação HTTP: Status " + response.StatusCode;
                }
                catch (Exception ex)
                {
                    return "Falha física/Internet ao conectar na API: " + ex.Message;
                }
            }
        }


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
