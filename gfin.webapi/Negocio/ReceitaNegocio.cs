using gfin.webapi.Dados;
using gfin.webapi.Dados.Enums;
using gfin.webapi.Dados.Models;
using gfin.webapi.Negocio.Erros;
using gfin.webapi.Negocio.Filtros;
using System;
using System.Collections.Generic;
using gfin.webapi.Negocio.Listeners;

namespace gfin.webapi.Negocio
{
    public class ReceitaNegocio : GenericNegocio
    {
        public ReceitaNegocio(IClienteListener cliente, GFinContext context) : base(cliente, context) { }
        /// <summary>
        /// Registra uma receita mensal no sistema;
        /// </summary>
        /// <param name="receita">Informações da receita a ser registrada.</param>
        public ReceitaMensal RegistrarReceita(ReceitaMensal receita)
        {
            ValidarReceita(receita);
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                receita.DataHoraRegistroReceita = DateTime.Now;
                receita = uofw.ReceitaMensal.Incluir(receita);
                uofw.SalvarAlteracoes();
            }
            return receita;
        }

        /// <summary>
        /// Realiza a validação das informações da receita antes de seu registro ou alteração.
        /// </summary>
        /// <param name="receita">Informações da receita a ser validada.</param>
        /// <returns></returns>
        private void ValidarReceita(ReceitaMensal receita)
        {
            if (receita == null)
            {
                throw new NegocioException("Nenhuma informação da receita mensal preenchida.");
            }
            if (receita.IdEntidade == 0)
            {
                receita.IdEntidade = ClienteListener.UsuarioLogado.IdEntidade;
            }
            if (String.IsNullOrEmpty(receita.TextoDescricaoReceita))
            {
                throw new NegocioException("Descrição da receita mensal não informado.");
            }

            if (receita.DataRecebimentoReceita == null)
            {
                throw new NegocioException("Data de recebimento da receita não informada.");
            }
            
            if (receita.IdNaturezaContaReceita == 0)
            {
                throw new NegocioException("Natureza da receita não informada.");
            }

            var negocioNatureza = new NaturezaNegocio(ClienteListener, GFinContext);
            if (!negocioNatureza.IsNaturezaConta(receita.IdEntidade, receita.IdNaturezaContaReceita))
            {//Verificar se a natureza informada existe...
                throw new NegocioException(
                    $"Natureza da Receita (Id: {receita.NaturezaContaReceita.Id}) não existe ou está inativa.");
            }

            if (receita.ValorReceita == 0)
            {
                throw new NegocioException("Valor da receita não informado.");
            }

            if (receita.IsReceitaLiquidada)
            {
                if (receita.DataHoraLiquidacaoReceita == null)
                {
                    throw new NegocioException("Data da liquidação da receita não informada.");
                }
                if (receita.ValorTotalLiquidacaoReceita == 0)
                {
                    throw new NegocioException("Valor total de liquidação da receita não informado.");
                }
            }

        }

        /// <summary>
        /// Registra uma receita fixa mensal no sistema.
        /// </summary>
        /// <param name="receita">Informações da receita fixa a ser registrada.</param>
        /// <returns></returns>
        public ReceitaFixa RegistrarReceitaFixa(ReceitaFixa receita)
        {
            
            ValidarReceitaFixa(receita);

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                receita.DataHoraRegistroReceitaFixa = DateTime.Now;
                receita = uofw.ReceitaFixa.Incluir(receita);
                uofw.SalvarAlteracoes();
            }
            return receita;

        }

        /// <summary>
        /// Realiza a validação das informações da receita fixa antes de seu registro ou alteração.
        /// </summary>
        /// <param name="receita">Informações da receita fixa a ser validada.</param>
        /// <returns></returns>
        private void ValidarReceitaFixa(ReceitaFixa receita)
        {
            if (receita == null)
            {
                throw new NegocioException("Nenhum informação de receita fixa preenchida.");
            }
            if (receita.IdEntidade == 0)
            {
                receita.IdEntidade = ClienteListener.UsuarioLogado.IdEntidade;
            }
            if (String.IsNullOrEmpty(receita.DescricaoReceitaFixa))
            {
                throw new NegocioException("Descrição da receita fixa mensal não informado.");
            }

            if (receita.DiaRecebimentoReceitaFixa == 0)
            {
                throw new NegocioException("Dia de recebimento da receita fixa não informado.");
            }

            if (receita.DiaRecebimentoReceitaFixa < 1 && receita.DiaRecebimentoReceitaFixa > 31)
            {
                throw new NegocioException("Dia de recebimento da receita fixa inválido.");
            }

            if (receita.IdNaturezaContaReceitaFixa == 0)
            {
                throw new NegocioException("Natureza da receita fixa não informada.");
            }
            var negocioNatureza = new NaturezaNegocio(ClienteListener, GFinContext);
            if (!negocioNatureza.IsNaturezaConta(receita.IdNaturezaContaReceitaFixa))
            {//Verificar se a natureza informada existe...
                throw new NegocioException(
                    $"Natureza da Receita Fixa (Id: {receita.NaturezaContaReceitaFixa.Id}) não existe ou está inativa.");
            }
            
            if (receita.ValorReceitaFixa <= 0)
            {
                throw new NegocioException("Valor da receita fixa não informado.");
            }

        }

        /// <summary>
        /// Verifica se existem receitas fixa do mês corrente que ainda não foram geradas como depesas do mês.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle financeiro.</param>
        /// <returns>Retorna true, se existir e false e não existirem.</returns>
        public bool IsReceitasFixasParaRegistroNoMesCorrente(int idEntidade)
        {
            bool isExistem = false;

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                int qtdReceitasFixas = uofw.ReceitaFixa.QuantRegistros(rf => rf.IdEntidade == idEntidade && rf.CodigoTipoSituacaoReceitaFixa == (short)TipoSituacaoEnum.Ativo);
                if (qtdReceitasFixas == 0)
                {//Não existem receitas fixas para geração...
                    return false;
                }

                var mesCorrente = DateTime.Now.Month;
                var anoCorrente = DateTime.Now.Year;
                var ultimoDiaMesCorrente = UtilNegocio.ObterDiasMes(mesCorrente, anoCorrente);

                var listReceitasFixas = uofw.ReceitaFixa.Listar(rf => rf.IdEntidade == idEntidade && rf.CodigoTipoSituacaoReceitaFixa == (short)TipoSituacaoEnum.Ativo);
                foreach (var receitaFixa in listReceitasFixas)
                {

                    int diaRecebimento = receitaFixa.DiaRecebimentoReceitaFixa;
                    if (ultimoDiaMesCorrente < diaRecebimento)
                        diaRecebimento = ultimoDiaMesCorrente;
                    var dataRecebimento = new DateTime(anoCorrente, mesCorrente, diaRecebimento);
                    if (IsReceitaFixaRegistradaComoReceitaMensal(idEntidade, receitaFixa.Id, dataRecebimento))
                    {//Receita mensal já registrada com base na fixa.
                        continue; //Recupera a próxima receita fixa.
                    }
                    isExistem = true;
                    break;
                }

                return isExistem;
            }

        }


        /// <summary>
        /// Verifica as receitas fixa do mês corrente e gera as depesas do mês caso não tenha sido gerada ainda.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle financeiro.</param>
        /// <returns>Retorna a quantidade de receitas fixa geradas no mês corrente.</returns>
        public int VerificarReceitasFixasDoMes(int idEntidade)
        {
            
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                int qtdReceitasFixas = uofw.ReceitaFixa.QuantRegistros(rf => rf.IdEntidade == idEntidade && rf.CodigoTipoSituacaoReceitaFixa == (short)TipoSituacaoEnum.Ativo);
                if (qtdReceitasFixas == 0)
                {//Não existem receitas fixas para geração...
                    return 0;
                }

                var mesCorrente = DateTime.Now.Month;
                var anoCorrente = DateTime.Now.Year;
                var ultimoDiaMesCorrente = UtilNegocio.ObterDiasMes(mesCorrente, anoCorrente);

                int qtdReceitasRegistradas = 0;
                ReceitaMensal receitaMensal;

                var listReceitasFixas = uofw.ReceitaFixa.Listar(rf => rf.IdEntidade == idEntidade && rf.CodigoTipoSituacaoReceitaFixa == (short)TipoSituacaoEnum.Ativo);
                foreach (var receitaFixa in listReceitasFixas)
                {

                    int diaRecebimento = receitaFixa.DiaRecebimentoReceitaFixa;
                    if (ultimoDiaMesCorrente < diaRecebimento)
                        diaRecebimento = ultimoDiaMesCorrente;
                    var dataRecebimento = new DateTime(anoCorrente, mesCorrente, diaRecebimento);
                    if (IsReceitaFixaRegistradaComoReceitaMensal(idEntidade, receitaFixa.Id, dataRecebimento))
                    {//Receita mensal já registrada com base na fixa.
                        continue; //Recupera a próxima receita fixa.
                    }

                    //Criar receita mensal com base na fixa.
                    receitaMensal = new ReceitaMensal();
                    receitaMensal.IdEntidade = idEntidade;
                    receitaMensal.IdReceitaFixa = receitaFixa.Id;
                    receitaMensal.CodigoTipoFormaRecebimento = (short)TipoFormaRecebimentoEnum.Dinheiro;
                    receitaMensal.TextoDescricaoReceita = receitaFixa.DescricaoReceitaFixa;
                    receitaMensal.DataRecebimentoReceita = dataRecebimento;
                    receitaMensal.IdNaturezaContaReceita = receitaFixa.IdNaturezaContaReceitaFixa;
                    receitaMensal.ValorReceita = receitaFixa.ValorReceitaFixa;
                    receitaMensal.IsReceitaLiquidada = false;
                    receitaMensal.ValorTotalLiquidacaoReceita = 0;

                    receitaMensal = RegistrarReceita(receitaMensal);
                    qtdReceitasRegistradas++; //Mais uma receita mensal registrada...

                }

                return qtdReceitasRegistradas;
            }

        }

        /// <summary>
        /// Verifica se a receita fixa foi registrada como mensal e com a mesma data de recebimento, 
        /// caso tenha sido registrada, retorna TRUE, se não FALSE.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param>
        /// <param name="idReceitaFixa">Identificador da receita fixa.</param>
        /// <param name="dataRecebimento">Data de recebimento da receita mensal.</param>
        /// <returns></returns>
        private bool IsReceitaFixaRegistradaComoReceitaMensal(int idEntidade, int idReceitaFixa, DateTime dataRecebimento)
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                int qtdReceitaMensal = uofw.ReceitaMensal.QuantRegistros(rm => rm.IdEntidade == idEntidade && rm.IdReceitaFixa == idReceitaFixa && rm.DataRecebimentoReceita == dataRecebimento);
                return (qtdReceitaMensal > 0);
            } 
        }

        /// <summary>
        /// Recupera a lista de receitas fixas.
        /// </summary>
        /// <param name="hasSomenteAtivas">Indicador de recuperação das receitas fixas, somente ativas?</param>
        /// <returns></returns>
        public List<ReceitaFixa> ListarReceitaFixa(bool hasSomenteAtivas)
        {
            List<ReceitaFixa> listaReceitas = null;
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                if (hasSomenteAtivas)
                    listaReceitas = uofw.ReceitaFixa.Listar(rf => rf.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && rf.CodigoTipoSituacaoReceitaFixa == (short)TipoSituacaoEnum.Ativo, 
                        rf => rf.DiaRecebimentoReceitaFixa, true, 
                        rf => rf.NaturezaContaReceitaFixa);
                else
                    listaReceitas = uofw.ReceitaFixa.Listar(rf => rf.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade, 
                        rf => rf.DiaRecebimentoReceitaFixa, true,
                        rf => rf.NaturezaContaReceitaFixa);
            } 
            return listaReceitas;
        }

        /// <summary>
        /// Recupera uma receita fixa da base pelo Id informado.
        /// </summary>
        /// <param name="id">Identificador da receita fixa.</param>
        /// <returns></returns>
        public ReceitaFixa ObterReceitaFixa(int id)
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                return uofw.ReceitaFixa.ObterPorId(id);
            } 
        }

        /// <summary>
        /// Grava as alterações realizadas na receita fixa em banco.
        /// </summary>
        /// <param name="receitaFixa">Objeto com as informações da receita fixa.</param>
        public void GravarReceitaFixa(ReceitaFixa receitaFixa)
        {
            ValidarReceitaFixa(receitaFixa);
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                uofw.ReceitaFixa.Alterar(receitaFixa);
                uofw.SalvarAlteracoes();
            } 
        }

        /// <summary>
        /// Remove uma receita fixa, verificando se existe alguma receita mensal vinculada a ela, 
        /// caso exista a exclusão será apenas lógica, caso não será uma exclusão física, removendo 
        /// o registro do banco de dados.
        /// </summary>
        /// <param name="receitaFixa">Objeto com as informações da receita fixa.</param>
        public void RemoverReceitaFixa(ReceitaFixa receitaFixa)
        {
            if (receitaFixa == null)
            {
                throw new NegocioException("Nenhum informação de receita fixa preenchida.");
            }

            if (receitaFixa.Id == 0)
            {
                throw new NegocioException("Identificador da receita fixa mensal não informado.");
            }

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {

                var _receitaFixa = uofw.ReceitaFixa.ObterPorId(receitaFixa.Id);
                if (_receitaFixa == null)
                {
                    throw new NegocioException(
                        $"Não foi possível encontrar a receita fixa com o Id [{receitaFixa.Id}].");
                }

                var qtd = uofw.ReceitaMensal.QuantRegistros(rm => rm.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && rm.IdReceitaFixa == _receitaFixa.Id, rm => rm.ReceitaFixa);
                if (qtd == 0)
                {//Nenhuma receita mensal vinculada... exclusão física.
                    uofw.ReceitaFixa.Excluir(_receitaFixa);
                }
                else
                {//Exclusão lógica.
                    _receitaFixa.CodigoTipoSituacaoReceitaFixa = (short)TipoSituacaoEnum.Inativo;
                    uofw.ReceitaFixa.Alterar(_receitaFixa);
                }
                uofw.SalvarAlteracoes();

            }

        }

        /// <summary>
        /// Remove uma receita fixa, verificando se existe alguma receita mensal vinculada a ela, 
        /// caso exista a exclusão será apenas lógica, caso não será uma exclusão física, removendo 
        /// o registro do banco de dados.
        /// </summary>
        /// <param name="idReceitaFixa">Identificador da receita que será removida.</param>
        public void RemoverReceitaFixa(int idReceitaFixa)
        {
            if (idReceitaFixa == 0)
                throw new NegocioException("Identificador da receita fixa mensal não informado.");

            ReceitaFixa _receitaFixa = null;
            bool hasVinculos = false;
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {

                _receitaFixa = uofw.ReceitaFixa.ObterPorId(idReceitaFixa);
                if (_receitaFixa == null)
                    throw new NegocioException(
                        $"Não foi possível encontrar a receita fixa com o Id [{_receitaFixa.Id}].");

                //Verificar vinculo em receitas mensais.
                var qtd = uofw.ReceitaMensal.QuantRegistros(dm => dm.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && dm.ReceitaFixa.Id == _receitaFixa.Id, dm => dm.ReceitaFixa);
                hasVinculos = (qtd != 0);

            }

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                if (hasVinculos)
                {//Exclusão lógica.
                    _receitaFixa.CodigoTipoSituacaoReceitaFixa = (short)TipoSituacaoEnum.Inativo;
                    uofw.ReceitaFixa.Alterar(_receitaFixa);
                }
                else
                {//Nenhuma receita mensal vinculada... exclusão física.
                    uofw.ReceitaFixa.Excluir(_receitaFixa);
                }

                uofw.SalvarAlteracoes();

            }
        }

        /// <summary>
        /// Recupera a lista de receitas mensais.
        /// </summary>
        /// <param name="filtro">Filtro para recuperação das receitas.</param>
        /// <returns></returns>
        public List<ReceitaMensal> ListarReceitaMensal(FiltroReceitaMensal filtro)
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                return uofw.ReceitaMensal.ListarReceitaPorFiltro(ClienteListener.UsuarioLogado.IdEntidade, filtro);
            }
        }

        /// <summary>
        /// Recupera o total de receita de um intervalo de datas (inicial e final), conforme o indicador informado:
        /// - 0 = Total Geral das Receitas;
        /// - 1 = Total de Receitas Recebidas;
        /// - 2 = Total de Receitas Abertas;
        /// - 3 = Total de Receitas Vencidas;
        /// </summary>
        /// <param name="dataInicialFiltro">Data da Inicial do filtro da recuperação do total.</param>
        /// <param name="dataFinalFiltro">Data da Final do filtro da recuperação do total.</param>
        /// <param name="indTipoTotalReceita">Indicador de retorno do valor total de receita.</param>
        /// <returns></returns>
        public decimal ObterTotalReceita(DateTime dataInicialFiltro, DateTime dataFinalFiltro, int indTipoTotalReceita)
        {
            if (dataInicialFiltro == null)
            {
                throw new NegocioException("Data inicial para recuperação do total de receitas nula.");
            }

            if (dataFinalFiltro == null)
            {
                throw new NegocioException("Data final para recuperação do total de receitas nula.");
            }

            int idEntidade = ClienteListener.UsuarioLogado.IdEntidade;

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                return uofw.ReceitaMensal.ObterTotalReceita(idEntidade, dataInicialFiltro, dataFinalFiltro, indTipoTotalReceita);
            } 
        }

        /// <summary>
        /// Recupera uma receita mensal pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador da receita mensal.</param>
        /// <returns></returns>
        public ReceitaMensal ObterReceitaMensal(int id)
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                return uofw.ReceitaMensal.Obter(rm => rm.Id == id, rm => rm.NaturezaContaReceita);
            }
        }

        /// <summary>
        /// Remove uma receita mensal, conforme id informado.
        /// </summary>
        /// <param name="idReceita">Identificador da receita a ser removida.</param>
        public void RemoverReceitaMensal(int idReceita)
        {
            if (idReceita == 0)
                throw new NegocioException("Identificador da receita mensal não informado.");

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                var _receita = uofw.ReceitaMensal.ObterPorId(idReceita);
                if (_receita == null)
                    throw new NegocioException(
                        $"Não foi encontrado a receita mensal com o Id [{idReceita}] para remoção.");
                if (_receita.IsReceitaLiquidada)
                    throw new NegocioException("Receita mensal já liquidada, para remover a receita, por favor realize seu estorno.");

                uofw.ReceitaMensal.Excluir(_receita);
                uofw.SalvarAlteracoes();
            }
        }

        /// <summary>
        /// Grava as alteraçãos realizadas na receita mensal informada.
        /// </summary>
        /// <param name="receita">Objeto de receita mensal com as informações alteradas.</param>
        public void GravarReceitaMensal(ReceitaMensal receita)
        {
            
            if (receita == null)
                throw new NegocioException("Receita mensal informada nula.");
            if (receita.Id == 0)
                throw new NegocioException("Identificador da receita mensal não informado.");
            if (receita.IsReceitaLiquidada)
                throw new NegocioException("Receita mensal já liquidada, alterações não permitidas.");

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                uofw.ReceitaMensal.Alterar(receita);
                uofw.SalvarAlteracoes();
            }
            
        }

        /// <summary>
        /// Realiza o estorno de um receita mensal recebida.
        /// </summary>
        /// <param name="idReceita">Identificador da despesa a ser estornada.</param>
        public void EstornarReceitaMensal(int idReceita)
        {
            if (idReceita == 0)
            {
                throw new NegocioException("Identificador da despesa mensal não informado, sem essa informação não é possível estornar.");
            }

            ReceitaMensal receita = ObterReceitaMensal(idReceita);
            if (receita.IsReceitaLiquidada == false)
            {
                throw new NegocioException("Receita mensal sinalizada como Não Recebida, estorno não necessário.");
            }

            receita.IsReceitaLiquidada = false;
            receita.DataHoraLiquidacaoReceita = null;
            receita.ValorTotalLiquidacaoReceita = null;

            GravarReceitaMensal(receita);

        }

        /// <summary>
        /// Realiza a liquidação de um receita, específica pelo seu ID, na data e valor total informado.
        /// </summary>
        /// <param name="idReceita">Identificador da receita que será liquidada.</param>
        /// <param name="dataLiquidacao">Data da liquidação, caso seja direfente de hoje.</param>
        /// <param name="valorTotalLiquidacao">Valor total da liquidação da receitam.</param>
        public void LiquidarReceita(int idReceita, DateTime dataLiquidacao, decimal valorTotalLiquidacao)
        {

            if (idReceita == 0)
                throw new NegocioException("Identificador da receita mensal não informado.");
            if (dataLiquidacao.CompareTo(DateTime.Now.Date) > 0)
                throw new NegocioException("Data de liquidação da receita deve ser maior ou igual a data atual.");
            if (valorTotalLiquidacao < 0)
                throw new NegocioException("Valor total de liquidação da receita deve precisa ser maior que zero.");
            
            ReceitaMensal _receita = ObterReceitaMensal(idReceita);
            if (_receita == null)
                throw new NegocioException($"Receita mensal com o Id [{idReceita}] não encontrada para liquidação.");
            if (_receita.IsReceitaLiquidada)
                throw new NegocioException("Receita mensal já liquidada, alterações não permitidas.");

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                
                _receita.DataHoraLiquidacaoReceita = dataLiquidacao;
                _receita.ValorTotalLiquidacaoReceita = valorTotalLiquidacao;
                _receita.IsReceitaLiquidada = true;

                uofw.ReceitaMensal.Alterar(_receita);
                uofw.SalvarAlteracoes();

            }

        }

        /// <summary>
        /// Recupera a lista de totais de receitas vencidas nos meses anteriores ao corrente.
        /// </summary>
        /// <param name="mesCorrente">Mês corrente para verificação das receitas vencidas a partir dele, ele não será incluído na busca.</param>
        /// <param name="anoCorrente">Ano corrente para verificação das receitas vencidas a partir dele, ele não será incluído na busca.</param>
        /// <returns></returns>
        public List<ReceitaMensal> ListarTotaisReceitasVencidasMesesAnteriores(int mesCorrente, int anoCorrente)
        {
            List<ReceitaMensal> listaResult = null;
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                var dataBuscaAnterior = new DateTime(anoCorrente, mesCorrente, 1);
                listaResult = uofw.ReceitaMensal.ListarTotaisReceitasVencidas(ClienteListener.UsuarioLogado.IdEntidade, dataBuscaAnterior);
            }
            return listaResult;
        }
    }

}