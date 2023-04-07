using GFin.Dados;
using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio.Erros;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GFin.Negocio
{
    public class EntidadeControleNegocio : GenericNegocio
    {
        public EntidadeControleNegocio(GFin.Negocio.Listeners.IClienteListener clienteListener) : base(clienteListener) { }
        /// <summary>
        /// Registra uma entidade de controle financeiro.
        /// </summary>
        /// <param name="entidade">Informações da entidade de controle financeiro a ser registrada.</param>
        /// <returns></returns>
        public EntidadeControle RegistrarEntidadeControle(EntidadeControle entidade)
        {
            ValidarEntidadeControle(entidade);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                //Informar data do registro da entidade.
                entidade.DataHoraRegistro = DateTime.Now;
                entidade = uofw.EntidadeControle.Incluir(entidade);
                uofw.SalvarAlteracoes();
                return entidade;
            }
        }

        /// <summary>
        /// Valida as informações da entidade.
        /// </summary>
        /// <param name="entidade">Informações da entidade a ser validada.</param>
        private void ValidarEntidadeControle(EntidadeControle entidade)
        {
            
            if (entidade == null)
                throw new NegocioException("Nenhuma informação da entidade de controle financeiro preenchida.");
            
            if (String.IsNullOrEmpty(entidade.NomeEntidade))
                throw new NegocioException("Nome da entidade de controle financeiro não informado.");
            entidade.NomeEntidade = entidade.NomeEntidade.Trim();

            if (entidade.CodigoTipoEntidade == (short)TipoEntidadeControleEnum.Juridica)
            {//Verificar preenchimento do CNPJ da empresa.
                if (String.IsNullOrEmpty(entidade.CpfCnpjEntidade))
                    throw new NegocioException("Número do CNPJ da empresa que terá o controle financeiro não informado.");
                if (!UtilNegocio.ValidarCNPJ(entidade.CpfCnpjEntidade))
                    throw new NegocioException("Número do CNPJ da empresa que terá o controle financeiro inválido.");
            }
            if (!String.IsNullOrEmpty(entidade.CpfCnpjEntidade))
            {
                if (entidade.CodigoTipoEntidade == (short)TipoEntidadeControleEnum.Fisica)
                {//Verificar preenchimento do CPF do responsável.
                    if (!UtilNegocio.ValidarCPF(entidade.CpfCnpjEntidade))
                        throw new NegocioException("Número do CPF do responsável que terá o controle financeiro inválido.");
                }
                using (IUnitOfWork uofw = new UnitOfWork())
                {
                    bool isEmpresa = (entidade.CodigoTipoEntidade == (short)TipoEntidadeControleEnum.Juridica);
                    int qtdRegs = uofw.EntidadeControle.QuantRegistros(ec => ec.CpfCnpjEntidade.Equals(entidade.CpfCnpjEntidade, StringComparison.CurrentCultureIgnoreCase));
                    if (qtdRegs > 0)
                        throw new NegocioException(string.Format("Já existe um registro para {0} com o número {1} [{2}] informado.", (isEmpresa ? "a empresa" : "o responsável"), (isEmpresa?"do CNPJ":"do CPF"), entidade.CpfCnpjEntidade));
                }
            }
        }

        /// <summary>
        /// Recupera a lista de entidades pelo tipo informado (fisico ou juridico);
        /// </summary>
        /// <param name="tipoEntidadeControle">Tipo da entidade controle financeiro.</param>
        /// <returns></returns>
        public List<EntidadeControle> ListarEntidadeControle(TipoEntidadeControleEnum tipoEntidadeControle)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                return uofw.EntidadeControle.Listar(e => e.CodigoTipoEntidade == (short)tipoEntidadeControle);
            }
        }

        /// <summary>
        /// Recupera a lista de entidades ATIVAS;
        /// </summary>
        /// <returns></returns>
        public List<EntidadeControle> ListarEntidadeControles()
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                return uofw.EntidadeControle.Listar(e => e.CodigoTipoSituacaoEntidade == (short)TipoSituacaoEnum.Ativo);
            }
        }

        /// <summary>
        /// Recupera uma entidade de conta pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador da entidade da conta para recuperar.</param>
        /// <returns></returns>
        public EntidadeControle ObterEntidadeControle(int id)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                return uofw.EntidadeControle.ObterPorId(id);
            }
        }

        /// <summary>
        /// Grava as alterações realizadas na entidade da conta em banco.
        /// </summary>
        /// <param name="entidade">Objeto com as informações da despesa fixa.</param>
        public void GravarEntidadeControle(EntidadeControle entidade)
        {
            ValidarEntidadeControle(entidade);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                uofw.EntidadeControle.Alterar(entidade);
                uofw.SalvarAlteracoes();
            } 
        }

        /// <summary>
        /// Remove uma entidade da conta, verificando se existe algum vínculo a ela, 
        /// caso exista a exclusão será apenas lógica, caso não será uma exclusão física, 
        /// removendo o registro do banco de dados.
        /// </summary>
        /// <param name="entidade">Objeto com as informações da entidade da conta.</param>
        public void RemoverEntidadeControle(EntidadeControle entidade)
        {
            if (entidade == null)
                throw new NegocioException("Nenhuma informação da entidade de controle preenchida.");

            if (entidade.Id == 0)
                throw new NegocioException("Identificador da entidade de controle não informado.");

            using (IUnitOfWork uofw = new UnitOfWork())
            {
                var _entidade = uofw.EntidadeControle.ObterPorId(entidade.Id);
                if (_entidade == null)
                    throw new NegocioException(string.Format("Não foi possível encontrar a entidade de controle com o Id [{0}].", entidade.Id));

                var qtd_usuarios = uofw.UsuarioAcessoEntidadeControle.QuantRegistros(uae => uae.IdEntidade == _entidade.Id);
                if (qtd_usuarios == 0)
                {//Nenhum usuário vinculado a entidade... exclusão física.
                    uofw.EntidadeControle.Excluir(_entidade);
                }
                else
                {//Exclusão lógica.
                    _entidade.CodigoTipoSituacaoEntidade = (short)TipoSituacaoEnum.Inativo;
                    uofw.EntidadeControle.Alterar(entidade);
                }
                uofw.SalvarAlteracoes();
            }

        }

        /// <summary>
        /// Recupera a lista de entidade de controle vinculada ao login (e-mail) do usuário.
        /// </summary>
        /// <param name="login">Login (e-mail) do usuário para recuperação das entidades.</param>
        /// <returns></returns>
        public List<EntidadeControle> ListarEntidadeControle(string login)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                return uofw.UsuarioAcessoEntidadeControle.ListarPorLoginUsuario(login);
            }
        }

        /// <summary>
        /// Recupera a entidade de controle, cuja o usuário está vinculado como responsável.
        /// Deve haver apenas uma, segundo as regras de negócio.
        /// </summary>
        /// <param name="idUsuario">Identificador do usuário para recuperação de sua entidade de controle.</param>
        /// <returns></returns>
        public EntidadeControle ObterEntidadeControlePadraoDoUsuario(int idUsuario)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                return uofw.UsuarioAcessoEntidadeControle.ObterEntidadeControleUsuario(idUsuario);
            }
        }

        /// <summary>
        /// Realiza alterações na entidade de controle que o usuário é responsável.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle que será alterada.</param>
        /// <param name="codigoTipoEntidadeControle">Código do tipo de entidade de controle.</param>
        /// <param name="nomeEntidadeControle">Nome da entidade de controle que será alterado.</param>
        /// <param name="cpfCnpjEntidadeControle">CNPJ da entidade de controle que será alterado.</param>
        public void AlterarNomeCnpjEntidadeControle(int idEntidade, short codigoTipoEntidadeControle, string nomeEntidadeControle, string cpfCnpjEntidadeControle)
        {

            if (idEntidade == 0)
                throw new NegocioException("Identificador da entidade de controle do usuário não informado.");
            if (codigoTipoEntidadeControle == 0)
                throw new NegocioException("Código do tipo da entidade de controle não informado.");
            if (string.IsNullOrEmpty(nomeEntidadeControle))
            {
                if (codigoTipoEntidadeControle == (short)TipoEntidadeControleEnum.Juridica)
                    throw new NegocioException("Nome da empresa não informado.");
                if (codigoTipoEntidadeControle == (short)TipoEntidadeControleEnum.Fisica)
                    throw new NegocioException("Legenda 'Minha Casa' não informada.");
            }
            if (codigoTipoEntidadeControle == (short)TipoEntidadeControleEnum.Juridica &&
                string.IsNullOrEmpty(cpfCnpjEntidadeControle))
                throw new NegocioException("Número do CPNJ da empresa não informado.");

            EntidadeControle _entidade = ObterEntidadeControle(idEntidade);
            if (_entidade == null)
                throw new NegocioException(string.Format("Não foi possível encontrar a entidade com o identificador [{0}] informado.", idEntidade));

            bool isHouveAlteracao = false;
            if (_entidade.CodigoTipoEntidade == (short)TipoEntidadeControleEnum.Fisica)
            {
                isHouveAlteracao = !nomeEntidadeControle.Equals(_entidade.NomeEntidade, StringComparison.CurrentCultureIgnoreCase);
            }
            else if (_entidade.CodigoTipoEntidade == (short)TipoEntidadeControleEnum.Juridica)
            {
                isHouveAlteracao = (!nomeEntidadeControle.Equals(_entidade.NomeEntidade, StringComparison.CurrentCultureIgnoreCase) || 
                    !cpfCnpjEntidadeControle.Equals(_entidade.CpfCnpjEntidade, StringComparison.CurrentCultureIgnoreCase));
            }

            if (isHouveAlteracao)
            {//Somente alterar os dados caso identificado mudanças.

                string _nomeAnterior = _entidade.NomeEntidade;
                string _cpfCnpjAnterior = _entidade.CpfCnpjEntidade;
                using (IUnitOfWork uofw = new UnitOfWork())
                {
                    _entidade.NomeEntidade = nomeEntidadeControle;
                    _entidade.CpfCnpjEntidade = cpfCnpjEntidadeControle;
                    uofw.EntidadeControle.Alterar(_entidade);
                    uofw.SalvarAlteracoes();
                }
                
                //Registrar histórico de alteração de dados da entidade de controle do usuário...
                GravarHistoricoUsuario(_entidade.Id,
                    TipoOperacaoHistoricoUsuarioEnum.AlterarDadosEntidadeControleUsuario,
                    string.Format("Dados anteriores - tipo: {0}, nome: {1}, cpfCnpj: {2}", _entidade.CodigoTipoEntidade, _nomeAnterior, _cpfCnpjAnterior));

            }

        }

    }
}
