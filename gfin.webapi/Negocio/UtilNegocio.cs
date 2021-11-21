using gfin.webapi.Negocio.Erros;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace gfin.webapi.Negocio
{
    public class UtilNegocio
    {
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);
        private static int[] diasMes = new int[] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static string[] meses = new string[] { "", "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };

        /// <summary>
        /// Retorna o nome do mês, conforme o número informado.
        /// </summary>
        /// <param name="mes">Número do mês para recuperar o seu nome.</param>
        /// <returns></returns>
        public static string ObterNomeMes(int mes)
        {
            if (mes < 1 || mes > 12)
            {
                throw new NegocioException("Mês (" + mes + ") inválido.");
            }
            return meses[mes];
        }

        /// <summary>
        /// Retorna o nome do dia da demanda de uma data informada.
        /// </summary>
        /// <param name="data">Data para recuperação do nome do dia da semana.</param>
        /// <returns></returns>
        public static string ObterNomeDiaSemana(DateTime data)
        {

            string nomeDiaSemana = "";
            var diaSemana = data.DayOfWeek;
            switch (diaSemana)
            {
                case DayOfWeek.Sunday:
                    nomeDiaSemana = "Domingo";
                    break;
                case DayOfWeek.Monday:
                    nomeDiaSemana = "Segunda-feira";
                    break;
                case DayOfWeek.Tuesday:
                    nomeDiaSemana = "Terça-feira";
                    break;
                case DayOfWeek.Wednesday:
                    nomeDiaSemana = "Quarta-feira";
                    break;
                case DayOfWeek.Thursday:
                    nomeDiaSemana = "Quinta-feira";
                    break;
                case DayOfWeek.Friday:
                    nomeDiaSemana = "Sexta-feira";
                    break;
                case DayOfWeek.Saturday:
                    nomeDiaSemana = "Sábado";
                    break;
            }
            return nomeDiaSemana;
        }

        /// <summary>
        /// Separa os nomes (opcional) e e-mails de uma string concatenada por ',' ou pelo separador informado. 
        /// Ex: Nome (email@dominio.com) ficando assim: Nome - email@dominio.com;
        /// Será retornado um objeto Dictionary[key = string:email][value = string:nome];
        /// </summary>
        /// <param name="emails">String com os nomes (opcional) e e-mails concatenados por ',' ou pelo separador informado;</param>
        /// <param name="separador">Separador dos valores de nome e e-mail, default = ','</param>
        /// <returns></returns>
        public static Dictionary<string, string> SepararEmails(string emails, char separador = ',')
        {
            Dictionary<string, string> listResult = new Dictionary<string, string>();
            var listEmails = emails.Split(separador);
            foreach (var item in listEmails)
            {
                string nome = string.Empty;
                string email = item.Replace(";", string.Empty);
                //Verificar se exite um nome para o e-mail.
                if (item.Contains("(") && item.Contains(")"))
                {
                    int idxFim = item.IndexOf("(");
                    nome = item.Substring(0, idxFim);
                    email = item.Remove(0, idxFim).Replace("(", string.Empty).Replace(")", string.Empty).Replace(";", string.Empty);
                }
                listResult.Add(email.Trim(), nome.Trim());
            }
            return listResult;
        }

        /// <summary>
        /// Recupera os dias de um mês, sem verificar o ano bissexto;
        /// </summary>
        /// <param name="mes">Número do mês para retornar os dias.</param>
        /// <returns></returns>
        public static int ObterDiasMes(int mes)
        {
            if (mes < 1 && mes > 12) return 0;
            return diasMes[mes];
        }

        /// <summary>
        /// Verifica um nome completo e reduz o nome para facilitar a leitura.
        /// </summary>
        /// <param name="nomeCompleto">Nome completo para reduzir.</param>
        /// <returns></returns>
        public static string ObterNomeReduzido(string nomeCompleto)
        {
            string[] nomes = nomeCompleto.Split(' ');
            return ConverterPrimeiraLetraEmMaiusculo(nomes[0]);
        }

        /// <summary>
        /// Recupera os dias de o mês informado, verificando se é ano bissexto, para ajustar quanto o mês for "Fevereiro".
        /// </summary>
        /// <param name="mes">Número do mês para retornar os dias.</param>
        /// <param name="ano">Número do ano para identificar se é bissexto.</param>
        /// <returns></returns>
        public static int ObterDiasMes(int mes, int ano)
        {
            int dias = ObterDiasMes(mes);
            if (dias == 0) return 0;
            if (mes == 2)
                if ((ano % 400) == 0 || ((ano % 4 == 0) && (ano % 100 != 0))) dias = 29;
            return dias;
        }

        /// <summary>
        /// Converte a primeira letra da string em maiúsculo.
        /// </summary>
        /// <param name="strValor">String para conversão da primeira maiúscula.</param>
        /// <returns></returns>
        public static string ConverterPrimeiraLetraEmMaiusculo(string strValor)
        {
            return char.ToUpper(strValor[0]) + strValor.Substring(1);
        }

        /// <summary>
        /// Gerador de identificador (ID) utilizando o método randomico.
        /// </summary>
        /// <returns></returns>
        public static int GerarIdRandom()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            string strId = DateTime.Now.ToString("yyMM");
            strId += random.Next(1001, 9999);
            return Int32.Parse(strId);
        }

        /// <summary>
        /// Calcula a próxima data, a partir da quantidade de dias informado, com a possibilidade de somente dia útil (segunda a sexta);
        /// Atenção, não será verificado feriados.
        /// </summary>
        /// <param name="data">Data de origem para calculo da próxima data.</param>
        /// <param name="qtdDias">Quantidade de dias a ser acrescido.</param>
        /// <param name="hasDiaUtil">(opcional) indicador para retornar apenas dia útil (segunda a sexta).</param>
        /// <returns></returns>
        public static DateTime ObterProximaData(DateTime data, int qtdDias, bool hasDiaUtil = false)
        {
            DateTime novaData = data.AddDays(qtdDias);
            if (hasDiaUtil)
            {//Verificar dia útil.
                if (novaData.DayOfWeek == DayOfWeek.Saturday)
                    novaData = novaData.AddDays(2);
                else if (novaData.DayOfWeek == DayOfWeek.Sunday)
                    novaData = novaData.AddDays(1);
            }
            return novaData;
        }

        /// <summary>
        /// Realizar a validação do CNPJ.
        /// </summary>
        /// <param name="numeroCnpj">Número do CNPJ para validação.</param>
        /// <returns></returns>
        public static bool ValidarCNPJ(string numeroCnpj)
        {
            string _cnpj = numeroCnpj.Replace(".", "");
            _cnpj = _cnpj.Replace("/", "");
            _cnpj = _cnpj.Replace("-", "");

            int[] digitos, soma, resultado;
            int nrDig;
            string ftmt;
            bool[] cnpjOk;

            ftmt = "6543298765432";
            digitos = new int[14];
            soma = new int[2];
            soma[0] = 0;
            soma[1] = 0;
            resultado = new int[2];
            resultado[0] = 0;
            resultado[1] = 0;
            cnpjOk = new bool[2];
            cnpjOk[0] = false;
            cnpjOk[1] = false;
            try
            {

                for (nrDig = 0; nrDig < 14; nrDig++)
                {
                    digitos[nrDig] = int.Parse(_cnpj.Substring(nrDig, 1));
                    if (nrDig <= 11)
                        soma[0] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig + 1, 1)));
                    if (nrDig <= 12)
                        soma[1] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig, 1)));
                }

                for (nrDig = 0; nrDig < 2; nrDig++)
                {
                    resultado[nrDig] = (soma[nrDig] % 11);
                    if ((resultado[nrDig] == 0) || (
                         resultado[nrDig] == 1))
                        cnpjOk[nrDig] = (digitos[12 + nrDig] == 0);
                    else
                        cnpjOk[nrDig] = (digitos[12 + nrDig] == (11 - resultado[nrDig]));
                }
                return (cnpjOk[0] && cnpjOk[1]);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validação de CPF informado.
        /// </summary>
        /// <param name="numeroCpf">Número do CPF para validação.</param>
        /// <returns></returns>
        public static bool ValidarCPF(string numeroCpf)
        {

            if (numeroCpf.Equals("000.000.000-00") || numeroCpf.Equals("111.111.111-11") || numeroCpf.Equals("222.222.222-22") || numeroCpf.Equals("333.333.333-33")
                || numeroCpf.Equals("444.444.444-44") || numeroCpf.Equals("555.555.555-55") || numeroCpf.Equals("666.666.666-66")
                || numeroCpf.Equals("777.777.777-77") || numeroCpf.Equals("888.888.888-88") || numeroCpf.Equals("999.999.999-99"))
                return false;
            else
            {
                int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                string tempCpf;
                string digito;
                int soma;
                int resto;
                numeroCpf = numeroCpf.Trim();
                numeroCpf = numeroCpf.Replace(".", "").Replace("-", "");
                if (numeroCpf.Length != 11)
                    return false;
                tempCpf = numeroCpf.Substring(0, 9);
                soma = 0;

                for (int i = 0; i < 9; i++)
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
                resto = soma % 11;
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;
                digito = resto.ToString();
                tempCpf = tempCpf + digito;
                soma = 0;
                for (int i = 0; i < 10; i++)
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
                resto = soma % 11;
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;
                digito = digito + resto.ToString();
                return numeroCpf.EndsWith(digito);
            }
        }

        /// <summary>
        /// Validar se o e-mail informado é válido.
        /// </summary>
        /// <param name="email">E-mail a ser validado.</param>
        /// <returns></returns>
        public static bool ValidarEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;
            try
            {
                return Regex.IsMatch(email,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Verificar se a senha do usuário é válida.
        /// </summary>
        /// <param name="senha">Senha a ser validada.</param>
        /// <returns></returns>
        public static bool ValidarSenha(string senha)
        {
            if (String.IsNullOrEmpty(senha))
                return false;
            try
            {
                return Regex.IsMatch(senha,
                      @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// Responsável pela geração de Hash (SHA512) de senha informada, com a informação de 
        /// salt (opcional), valor randomico adicionado a senha para fortalece-la.
        /// </summary>
        /// <param name="password">Senha que será convertida em HASH.</param>
        /// <param name="salt">Valor utilizado para fortalecer a senha informada, utilizar método: GerarSaltPassword() para criação de SALT aleatório.</param>
        /// <returns></returns>
        public static string GerarHashPassword(string password, string salt)
        {
            HashAlgorithm hash = new SHA512Managed();
            var hsh = hash.ComputeHash(Encoding.UTF8.GetBytes(password + salt).ToArray());
            for (var i = 0; i < 16; i++)
            {
                hsh = hash.ComputeHash(Encoding.UTF8.GetBytes(password + salt).ToArray());
            }
            return Convert.ToBase64String(hsh);
        }

        /// <summary>
        /// Responsável pela geração de um valor aleatório (SALT) utilizado juntamente com a senha para fortalece-la.<br/>
        /// Atenção >> Necessário armazenar o salt gerado para ser utilizado para confirmação de senha do usuário.
        /// </summary>
        /// <returns></returns>
        public static string GerarSaltPassword()
        {
            const int minSaltSize = 16;
            const int maxSaltSize = 64;

            // Generate a random number for the size of the salt.
            int saltSize = _random.Next(minSaltSize, maxSaltSize);

            // Allocate a byte array, which will hold the salt.
            var saltBytes = new byte[saltSize];

            // Initialize a random number generator.
            var rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(saltBytes);

            // Fill the salt with cryptographically strong byte values.
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Gerar uma senha automática de 8 dígitos.
        /// </summary>
        /// <returns>Retorna uma senha randomica de 8 dígitos númericos.</returns>
        public static string GerarPasswordRandom()
        {
            var strDigitos = new StringBuilder(8);
            for (var i = 0; i < 8; i++)
                strDigitos.Append(_random.Next(i == 0 ? 1 : 0, 9));
            return strDigitos.ToString();
        }

        /// <summary>
        /// Realiza a codificação de uma 'String' em 'Base64' para descaracterizar seu conteúdo.
        /// </summary>
        /// <param name="valor">Valor que será codificado em Base64.</param>
        /// <returns></returns>
        public static string Criptografar(string valor)
        {
            if (string.IsNullOrEmpty(valor)) 
                return string.Empty;
            return Convert.ToBase64String(ConvertStringToBytes(valor));
        }

        /// <summary>
        /// Realiza a decodificação de uma string codificada em 'Base64' para leitura do conteúdo.
        /// </summary>
        /// <param name="valor">String codificada em 'Base64' para decodificar.</param>
        /// <returns></returns>
        public static string Descriptografar(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return null;
            try
            {
                byte[] data = Convert.FromBase64String(valor);
                return Encoding.Unicode.GetString(data);
            }
            catch (Exception)
            {
                return valor;
            }
        }

        /// <summary>
        /// Converte uma variavel string em byte[] array;
        /// </summary>
        /// <param name="str">String a ser convertiva.</param>
        /// <returns></returns>
        private static byte[] ConvertStringToBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Criar instância de image do cabeçalho do e-mail com anexo 'embedder' no e-mail.
        /// </summary>
        /// <param name="cidAnexo">Identificador da imagem dentro do e-mail.</param>
        /// <returns></returns>
        public static EmailAnexo ObterAnexoImagemCabecalho(string cidAnexo, string mapPathAppServidor)
        {
            EmailAnexo imagemAnexo = null;
            var pathImageCabecalhoEmail = $"{mapPathAppServidor}\\Content\\site\\imgs\\logo-inFourSys-email-topo.png";
            FileInfo fileImagem = new FileInfo(pathImageCabecalhoEmail);
            if (fileImagem.Exists)
            {
                byte[] bytesImgCabecalhoEmail = File.ReadAllBytes(fileImagem.FullName);
                imagemAnexo = new EmailAnexo(bytesImgCabecalhoEmail, cidAnexo, fileImagem.Name);
            }
            return imagemAnexo;
        }
    }
}