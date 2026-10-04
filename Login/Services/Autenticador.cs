using System.Security.Cryptography;
using System.Text;

namespace Login.Services
{
    public class Autenticador
    {
 
       

        //gerador de hash   
        public float CriaRandom()
        {
            float numeroRandomico;
            Random random = new Random();
            numeroRandomico = random.Next(1, 9999999);
            return numeroRandomico;
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

        public void GravaAcesso(int idUsuario, string atividade, string nivel)
        {
            


        }





        //Validador de email.............................
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }


}