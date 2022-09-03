using GFin.Dados;
using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio.Erros;
using GFin.Negocio.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using GFin.Core.Negocio.DTOs;

namespace GFin.Negocio
{
    public class DespesaNegocio : GenericNegocio
    {
        public DespesaNegocio(GFin.Negocio.Listeners.IClienteListener clienteListener) : base(clienteListener) { }
        /// <summary>
        /// Registra uma despesa mensal no sistema;
        /// </summary>
        /// <param name="despesa">Informações da despesa a ser registrada.</param>
        public DespesaMensal RegistrarDespesa(DespesaMensal despesa)
        {

            ValidarDespesa(despesa);
            
            if (!despesa.IsDespesaLiquidada)
            {//Registrar a despesa como NÃO liquidada.
                despesa.DataHoraLiquidacaoDespesa = null;
                despesa.TextoObservacaoDespesa = string.Empty;
                despesa.ValorDescontoLiquidacaoDespesa = 0;
                despesa.ValorMultaJurosLiquidacaoDespesa = 0;
                despesa.ValorTotalLiquidacaoDespesa = 0;
            }

            despesa.DataHoraRegistroDespesa = DateTime.Now;

            //Inclusão de apenas uma despesa. 
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                despesa = uofw.DespesaMensal.Incluir(despesa);
                uofw.SalvarAlteracoes();
                //Verificar se é uma despesa liquidada com cheque... sendo gerar emissão do mesmo.
                if (despesa.CodigoTipoFormaLiquidacao == (int)TipoFormaLiquidacaoEnum.ChequeAVista ||
                    despesa.CodigoTipoFormaLiquidacao == (int)TipoFormaLiquidacaoEnum.ChequePreDatado)
                {//Gerar emissão do cheque.
                    ChequeEmitido chequeEmitido = new ChequeEmitido();
                    chequeEmitido.IdCheque = despesa.CodigoVinculoFormaLiquidacao.Value;
                    chequeEmitido.ValorChequeEmitido = despesa.ValorDespesa;
                    chequeEmitido.DataEmissaoCheque = despesa.DataHoraRegistroDespesa.Date;
                    chequeEmitido.DataVencimentoCheque = despesa.DataVencimentoDespesa;
                    chequeEmitido.HistoricoEmissaoCheque = string.Format("{0} emitido para despesa: {1}", UtilEnum.GetTextoFormaLiquidacao(despesa.CodigoTipoFormaLiquidacao), despesa.DescricaoDespesa);
                    ChequeNegocio negocioCheque = new ChequeNegocio(base.ClienteListener);
                    negocioCheque.EmitirCheque(chequeEmitido);
                }
                return despesa;
            }

        }

        /// <summary>
        /// Verifica há existência de despesas fixa do mês corrente que ainda não foram geradas como depesas do mês.
        /// </summary>
        /// <returns>Retorna true caso existam despesas fixas e false caso contrário.</returns>
        public bool IsDespesasFixasParaRegistroNoMesCorrente(int idEntidade)
        {
            bool isExistem = false;

            using (IUnitOfWork uofw = new UnitOfWork())
            {
                int qtdDespesasFixas = uofw.DespesaFixa.QuantRegistros(df => df.IdEntidade == idEntidade && df.CodigoTipoSituacaoDespesaFixa == (short)TipoSituacaoEnum.Ativo);
                if (qtdDespesasFixas == 0)
                {//Não existem despesas fixas para geração...
                    return isExistem;
                }

                var mesCorrente = DateTime.Now.Month;
                var anoCorrente = DateTime.Now.Year;
                var ultimoDiaMesCorrente = UtilNegocio.ObterDiasMes(mesCorrente, anoCorrente);

                var listDespesasFixas = uofw.DespesaFixa.Listar(df => df.IdEntidade == idEntidade && df.CodigoTipoSituacaoDespesaFixa == (short)TipoSituacaoEnum.Ativo);
                foreach (var despesaFixa in listDespesasFixas)
                {

                    int diaVencimento = despesaFixa.DiaVencimentoDespesaFixa;
                    if (ultimoDiaMesCorrente < diaVencimento)
                        diaVencimento = ultimoDiaMesCorrente;
                    var dataVenciamento = new DateTime(anoCorrente, mesCorrente, diaVencimento);
                    if (IsDespesaFixaRegistradaComoDespesaMensal(idEntidade, despesaFixa.Id, dataVenciamento))
                    {//Despesa mensal já registrada com base na fixa.
                        continue; //Recupera a próxima despesa fixa.
                    }
                    isExistem = true;
                    break;
                }
            }

            return isExistem;
        }

        /// <summary>
        /// Recupera a lista de despesas mensais totalizada por mês e valor de um ano informado.
        /// </summary>
        /// <param name="anoCorrente">Ano para totalizar as despesas.</param>
        /// <returns></returns>
        public List<TotalReceitaMensalDTO> ListarTotaisReceitasAnual(int anoCorrente)
        {

            decimal totalReceitasFixas = ObterTotalReceitasFixas();

            List<TotalReceitaMensalDTO> listResult = new List<TotalReceitaMensalDTO>();
            for (int mes = 1; mes <= 12; mes++)
            {
                int dia = (DateTime.Now.Month == mes ? DateTime.Now.Day : 1);
                DateTime mesCompetencia = new DateTime(anoCorrente, mes, dia);
                decimal valorTotalMes = 0;
                if (mes > DateTime.Now.Month)
                    valorTotalMes = totalReceitasFixas;
                listResult.Add(new TotalReceitaMensalDTO { MesCompetencia = mesCompetencia, ValorTotalMes = valorTotalMes });
            }

            using (IUnitOfWork uofw = new UnitOfWork())
            {
                var result = uofw.ReceitaMensal.ListarTotaisReceitaAnual(ClienteListener.UsuarioLogado.IdEntidade, anoCorrente);
                foreach (var item in listResult)
                {
                    if (result.ContainsKey(item.MesCompetencia.Month))
                    {//Atualizar lista... com valores.
                        item.ValorTotalMes += result[item.MesCompetencia.Month];
                    }
                }
            }

            return listResult;
        }

        /// <summary>
        /// Recupera a lista de despesas mensais totalizada por mês e valor de um ano informado.
        /// </summary>
        /// <param name="anoCorrente">Ano para totalizar as despesas.</param>
        /// <returns></returns>
        public List<TotalDespesaMensalDTO> ListarTotaisDespesasAnual(int anoCorrente)
        {

            decimal totalDespesasFixas = ObterTotalDespesasFixas();

            List<TotalDespesaMensalDTO> listResult = new List<TotalDespesaMensalDTO>();
            for (int mes = 1; mes <= 12; mes++)
            {
                int dia = (DateTime.Now.Month == mes ? DateTime.Now.Day : 1);
                DateTime mesCompetencia = new DateTime(anoCorrente, mes, dia);
                decimal valorTotalMes = 0;
                if (mes > DateTime.Now.Month)
                    valorTotalMes = totalDespesasFixas;
                listResult.Add(new TotalDespesaMensalDTO { MesCompetencia = mesCompetencia, ValorTotalMes = valorTotalMes });
            }

            using (IUnitOfWork uofw = new UnitOfWork())
            {
                var result = uofw.DespesaMensal.ListarTotaisDespesasAnual(ClienteListener.UsuarioLogado.IdEntidade, anoCorrente);
                foreach (var item in listResult)
                {
                    if (result.ContainsKey(item.MesCompetencia.Month))
                    {//Atualizar lista... com os valores.
                        item.ValorTotalMes += result[item.MesCompetencia.Month];
                    }
                }
            }

            return listResult;
        }

        /// <summary>
        /// Recupera o total de despesas fixas, ativas, de uma determinada entidade.
        /// </summary>
        /// <returns></returns>
        public decimal ObterTotalDespesasFixas()
        {
            decimal totalResult = 0;
            int idEntidade = ClienteListener.UsuarioLogado.IdEntidade;
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                totalResult = uofw.DespesaFixa.ObterTotalDespesasFixas(idEntidade);
            }
            return totalResult;
        }

        /// <summary>
        /// Recupera o total de receitas fixas, ativas, de uma determinada entidade.
        /// </summary>
        /// <returns></returns>
        public decimal ObterTotalReceitasFixas()
        {
            decimal totalResult = 0;
            int idEntidade = ClienteListener.UsuarioLogado.IdEntidade;
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                totalResult = uofw.ReceitaFixa.ObterTotalReceitasFixas(idEntidade);
            }
            return totalResult;
        }

        /// <summary>
        /// Responsável por registrar as despesas parceladas no sistema.
        /// </summary>
        /// <param name="despesaParcelada">Objeto com o parcelamento da despesa para registro.</param>
        public void RegistrarDespesaParcelada(DespesaMensalParcelada despesaParcelada)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {

                //Identificador de vinculo entre as despesas...
                int idVinculoDespesaParcelada = UtilNegocio.GerarIdRandom();

                DespesaMensal despesaMensal;
                int qtdParcelas = despesaParcelada.ParcelamentoDespesa.Count;
                foreach (var item in despesaParcelada.ParcelamentoDespesa)
	            {

                    despesaMensal = new DespesaMensal();
                    despesaMensal.IdEntidade = ClienteListener.UsuarioLogado.IdEntidade;
                    despesaMensal.DescricaoDespesa = string.Format("{0}/{1}-{2}", item.NumeroParcela.ToString("00"), qtdParcelas.ToString("00"), despesaParcelada.TextoDescricaoDespesa);
                    despesaMensal.DataVencimentoDespesa = item.DataVencimentoParcela;
                    despesaMensal.CodigoTipoFormaLiquidacao = despesaParcelada.CodigoTipoFormaLiquidacao;
                    despesaMensal.IdNaturezaContaDespesa = despesaParcelada.IdNaturezaContaDespesa;
                    despesaMensal.CodigoVinculoFormaLiquidacao = item.CodigoVinculoFormaLiquidacao;
                    despesaMensal.ValorDespesa = item.ValorParcela;
                    despesaMensal.IsDespesaLiquidada = item.IsLiquidada;
                    despesaMensal.IsDespesaParcelada = true;
                    despesaMensal.CodigoDespesaParcelada = idVinculoDespesaParcelada;
                    despesaMensal.TextoObservacaoDespesa = string.Empty;
                    despesaMensal.ValorDescontoLiquidacaoDespesa = 0;
                    despesaMensal.ValorMultaJurosLiquidacaoDespesa = 0;
                    despesaMensal.ValorTotalLiquidacaoDespesa = 0;
                    despesaMensal.DataHoraRegistroDespesa = DateTime.Now;

                    uofw.DespesaMensal.Incluir(despesaMensal);

                    //Verificar se é uma despesa liquidada com cheque... sendo gerar emissão do mesmo.
                    if (despesaMensal.CodigoTipoFormaLiquidacao == (int)TipoFormaLiquidacaoEnum.ChequePreDatado)
                    {//Gerar emissão do cheque.
                        ChequeEmitido chequeEmitido = new ChequeEmitido();
                        chequeEmitido.IdCheque = despesaMensal.CodigoVinculoFormaLiquidacao.Value;
                        chequeEmitido.ValorChequeEmitido = despesaMensal.ValorDespesa;
                        chequeEmitido.DataEmissaoCheque = despesaMensal.DataHoraRegistroDespesa.Date;
                        chequeEmitido.DataVencimentoCheque = despesaMensal.DataVencimentoDespesa;
                        chequeEmitido.HistoricoEmissaoCheque = string.Format("{0} emitido para despesa: {1}", UtilEnum.GetTextoFormaLiquidacao(despesaMensal.CodigoTipoFormaLiquidacao), despesaMensal.DescricaoDespesa);
                        ChequeNegocio negocioCheque = new ChequeNegocio(base.ClienteListener);
                        negocioCheque.EmitirCheque(chequeEmitido);
                    }

                }

                uofw.SalvarAlteracoes();
            }

        }

        /// <summary>
        /// Realiza a validação das informações da despesa antes de seu registro ou alteração.
        /// </summary>
        /// <param name="despesa">Informações da despesa a ser validada.</param>
        /// <returns></returns>
        private void ValidarDespesa(DespesaMensal despesa)
        {
            if (despesa == null)
            {
                throw new NegocioException("Despesa informada nula.");
            }
            if (despesa.IdEntidade == 0)
            {
                despesa.IdEntidade = ClienteListener.UsuarioLogado.IdEntidade;
            }
            if (String.IsNullOrEmpty(despesa.DescricaoDespesa))
            {
                throw new NegocioException("Descrição da despesa mensal não informado.");
            }

            if (despesa.DataVencimentoDespesa == null)
            {
                throw new NegocioException("Data de vencimento da despesa não informada.");
            }
            
            if (despesa.CodigoTipoFormaLiquidacao == (int)TipoFormaLiquidacaoEnum.NaoInformado)
            {
                throw new NegocioException("Forma de pagamento da despesa não informada.");
            }

            if (despesa.IdNaturezaContaDespesa == 0)
            {
                throw new NegocioException("Natureza da despesa não informada.");
            }

            var negocioNatureza = new NaturezaNegocio(base.ClienteListener);
            if (!negocioNatureza.IsNaturezaConta(despesa.IdEntidade, despesa.IdNaturezaContaDespesa))
            {//Verificar se a natureza informada existe...
                throw new NegocioException(string.Format("Natureza da Despesa (Id: {0}) não existe ou está inativa.", despesa.IdNaturezaContaDespesa));
            }

            if (despesa.ValorDespesa == 0)
            {
                throw new NegocioException("Valor da despesa não informado.");
            }

            if (despesa.IsDespesaLiquidada)
            {
                if (despesa.DataHoraLiquidacaoDespesa == null)
                {
                    throw new NegocioException("Data da liquidação da despesa não informada.");
                }
                if (despesa.ValorTotalLiquidacaoDespesa == 0 && despesa.ValorDescontoLiquidacaoDespesa == 0)
                {
                    throw new NegocioException("Valor total de liquidação da despesa não informado.");
                }
            }

        }

        /// <summary>
        /// Registra uma despesa fixa mensal no sistema.
        /// </summary>
        /// <param name="despesa">Informações da despesa fixa a ser registrada.</param>
        /// <returns></returns>
        public DespesaFixa RegistrarDespesaFixa(DespesaFixa despesa)
        {
            
            ValidarDespesaFixa(despesa);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                //Registrar histórico de evolução da despesa fixa.
                HistoricoDespesaFixa historicoDespesaFixa = new HistoricoDespesaFixa();
                historicoDespesaFixa.IdDespesaFixa = despesa.Id;
                historicoDespesaFixa.ValorHistoricoDespesaFixa = despesa.ValorDespesaFixa;
                historicoDespesaFixa.DataHoraRegistroHistoricoDespesaFixa = DateTime.Now;
                uofw.HistoricoDespesaFixa.Incluir(historicoDespesaFixa);

                despesa.DataHoraRegistroDespesaFixa = historicoDespesaFixa.DataHoraRegistroHistoricoDespesaFixa;
                despesa = uofw.DespesaFixa.Incluir(despesa);

                uofw.SalvarAlteracoes();

                return despesa;
            }

        }

        /// <summary>
        /// Realiza a validação das informações da despesa fixa antes de seu registro ou alteração.
        /// </summary>
        /// <param name="despesa">Informações da despesa fixa a ser validada.</param>
        /// <returns></returns>
        private void ValidarDespesaFixa(DespesaFixa despesa)
        {
            if (despesa == null)
            {
                throw new NegocioException("Despesa Fixa informada nula.");
            }
            if (despesa.IdEntidade == 0)
            {
                despesa.IdEntidade = ClienteListener.UsuarioLogado.IdEntidade;
            }
            if (String.IsNullOrEmpty(despesa.DescricaoDespesaFixa))
            {
                throw new NegocioException("Descrição da despesa fixa mensal não informado.");
            }

            if (despesa.DiaVencimentoDespesaFixa == 0)
            {
                throw new NegocioException("Dia de vencimento da despesa fixa não informado.");
            }

            if (despesa.DiaVencimentoDespesaFixa < 1 && despesa.DiaVencimentoDespesaFixa > 31)
            {
                throw new NegocioException("Dia de vencimento da despesa fixa inválido.");
            }

            if (despesa.IdNaturezaContaDespesaFixa == 0)
            {
                throw new NegocioException("Natureza da despesa fixa não informada.");
            }
            var negocioNatureza = new NaturezaNegocio(base.ClienteListener);
            if (!negocioNatureza.IsNaturezaConta(despesa.IdNaturezaContaDespesaFixa))
            {//Verificar se a natureza informada existe...
                throw new NegocioException(string.Format("Natureza da Despesa Fixa (Id: {0}) não existe ou está inativa.", despesa.NaturezaContaDespesaFixa.Id));
            }
            
            if (despesa.ValorDespesaFixa <= 0)
            {
                throw new NegocioException("Valor da despesa fixa não informado.");
            }

        }

        /// <summary>
        /// Verifica as despesas fixa do mês corrente e gera as depesas do mês caso não tenha sido gerada ainda.
        /// </summary>
        /// <returns>Retorna a quantidade de despesas fixa geradas no mês corrente.</returns>
        public int VerificarDespesasFixasDoMes(int idEntidade)
        {
            int qtdDespesasRegistradas = 0;
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                int qtdDespesasFixas = uofw.DespesaFixa.QuantRegistros(df => df.IdEntidade == idEntidade && df.CodigoTipoSituacaoDespesaFixa == (short)TipoSituacaoEnum.Ativo);
                if (qtdDespesasFixas == 0)
                {//Não existem despesas fixas para geração...
                    return 0;
                }

                var mesCorrente = DateTime.Now.Month;
                var anoCorrente = DateTime.Now.Year;
                var ultimoDiaMesCorrente = UtilNegocio.ObterDiasMes(mesCorrente, anoCorrente);

                DespesaMensal despesaMensal = null;

                var listDespesasFixas = uofw.DespesaFixa.Listar(df => df.IdEntidade == idEntidade && df.CodigoTipoSituacaoDespesaFixa == (short)TipoSituacaoEnum.Ativo);
                foreach (var despesaFixa in listDespesasFixas)
                {

                    int diaVencimento = despesaFixa.DiaVencimentoDespesaFixa;
                    if (ultimoDiaMesCorrente < diaVencimento)
                        diaVencimento = ultimoDiaMesCorrente;
                    var dataVenciamento = new DateTime(anoCorrente, mesCorrente, diaVencimento);
                    if (IsDespesaFixaRegistradaComoDespesaMensal(idEntidade, despesaFixa.Id, dataVenciamento))
                    {//Despesa mensal já registrada com base na fixa.
                        continue; //Recupera a próxima despesa fixa.
                    }

                    //Atualizar valor da despesa fixa...
                    AtualizarValorDespesaFixa(despesaFixa);

                    //Criar despesa mensal com base na fixa.
                    despesaMensal = new DespesaMensal();
                    despesaMensal.IdEntidade = idEntidade;
                    despesaMensal.IdDespesaFixa = despesaFixa.Id;
                    despesaMensal.DescricaoDespesa = despesaFixa.DescricaoDespesaFixa;
                    despesaMensal.DataVencimentoDespesa = dataVenciamento;
                    despesaMensal.CodigoTipoFormaLiquidacao = despesaFixa.CodigoTipoFormaLiquidacaoDespesaFixa;
                    despesaMensal.IdNaturezaContaDespesa = despesaFixa.IdNaturezaContaDespesaFixa;
                    despesaMensal.ValorDespesa = despesaFixa.ValorDespesaFixa;
                    despesaMensal.IsDespesaLiquidada = false;
                    despesaMensal.IsDespesaParcelada = false;
                    despesaMensal.TextoObservacaoDespesa = string.Empty;
                    despesaMensal.ValorDescontoLiquidacaoDespesa = 0;
                    despesaMensal.ValorMultaJurosLiquidacaoDespesa = 0;
                    despesaMensal.ValorTotalLiquidacaoDespesa = 0;
                    despesaMensal.DataHoraRegistroDespesa = DateTime.Now;

                    despesaMensal = uofw.DespesaMensal.Incluir(despesaMensal);

                    qtdDespesasRegistradas++; //Mais uma despesa mensal registrada...

                }

                uofw.SalvarAlteracoes();

            }

            return qtdDespesasRegistradas;

        }

        /// <summary>
        /// Atualizar o valor da despesa fixa conforme o valor pago nos últimos três meses.
        /// Essa funcionalidade garante que qualquer ajuste na despesa fixa seja considerada nos demais meses seguintes.
        /// </summary>
        /// <param name="despesaFixa">Instância da despesa fixa a ser atualizada.</param>
        private decimal AtualizarValorDespesaFixa(DespesaFixa despesaFixa)
        {

            decimal valorDespesaFixaAtualizada = despesaFixa.ValorDespesaFixa;
            
            //Recuperar as três ultimas despesas mensais pagas...
            var listUltimasDespesasMensaisPagas = ListarUltimasDespesasMensaisPagas(despesaFixa.IdEntidade, despesaFixa.Id, 3);
            decimal totalDespesasMensaisPagas = 0;
            int qtdDespesasMensaisPagas = 0;
            foreach (var itemDespesaMensal in listUltimasDespesasMensaisPagas)
            {
                totalDespesasMensaisPagas += (itemDespesaMensal.ValorTotalLiquidacaoDespesa.HasValue ? itemDespesaMensal.ValorTotalLiquidacaoDespesa.Value : 0);
                qtdDespesasMensaisPagas += (itemDespesaMensal.ValorTotalLiquidacaoDespesa.HasValue ? 1 : 0);
            }

            if (qtdDespesasMensaisPagas <= 1)
            {//Identificada que existem poucas despesa paga... NÃO será recalculado valor da despesa fixa.
                return valorDespesaFixaAtualizada;
            }

            //Atualizar valor da despesa fixa...
            valorDespesaFixaAtualizada = (totalDespesasMensaisPagas / qtdDespesasMensaisPagas);

            //Verificar se existe diferença do valor da despesa fixa (atual) e a calculada.
            if (valorDespesaFixaAtualizada == despesaFixa.ValorDespesaFixa)
            {//Não houve diferença;
                return valorDespesaFixaAtualizada;
            }

            //Registrar histório de atualização da despesa fixa.
            using (IUnitOfWork uofw = new UnitOfWork())
            {

                HistoricoDespesaFixa historicoDespesaFixa = new HistoricoDespesaFixa();
                historicoDespesaFixa.IdDespesaFixa = despesaFixa.Id;
                historicoDespesaFixa.ValorHistoricoDespesaFixa = despesaFixa.ValorDespesaFixa;
                historicoDespesaFixa.DataHoraRegistroHistoricoDespesaFixa = DateTime.Now;

                uofw.HistoricoDespesaFixa.Incluir(historicoDespesaFixa);

                despesaFixa.ValorDespesaFixa = valorDespesaFixaAtualizada;
                uofw.DespesaFixa.Alterar(despesaFixa);

                uofw.SalvarAlteracoes();

            }

            return valorDespesaFixaAtualizada;

        }

        /// <summary>
        /// Retorna as últimas despesas mensais pagas vinculadas a uma despesa fixa, conforme id da despesa fixa e a quantidade que será retornada.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param>
        /// <param name="idDespesaFixa">Identificador da despesa fixa de origem.</param>
        /// <param name="qtdDespesasRetornadas">Quant. de despesas pagas a ser retornada.</param>
        /// <returns></returns>
        private List<DespesaMensal> ListarUltimasDespesasMensaisPagas(int idEntidade, int idDespesaFixa, int qtdDespesasRetornadas)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                return uofw.DespesaMensal.Listar(dm => dm.IdEntidade == idEntidade && dm.IdDespesaFixa == idDespesaFixa && dm.IsDespesaLiquidada == true);
            }
        }

        /// <summary>
        /// Verifica se a despesa fixa foi registrada como mensal e com a mesma data de vencimento, 
        /// caso tenha sido registrada, retorna TRUE, se não FALSE.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param>
        /// <param name="idDespesaFixa">Identificador da despesa fixa.</param>
        /// <param name="dataVenciamento">Data de vencimento da despesa mensal.</param>
        /// <returns></returns>
        private bool IsDespesaFixaRegistradaComoDespesaMensal(int idEntidade, int idDespesaFixa, DateTime dataVenciamento)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                int qtdDespesaMensal = uofw.DespesaMensal.QuantRegistros(dm => dm.IdEntidade == idEntidade && dm.IdDespesaFixa == idDespesaFixa && dm.DataVencimentoDespesa == dataVenciamento);
                return (qtdDespesaMensal > 0);
            } 
        }

        /// <summary>
        /// Recupera a lista de despesas fixas.
        /// </summary>
        /// <param name="hasSomenteAtivas">Indicador de recuperação das despesas fixas, somente ativas?</param>
        /// <returns></returns>
        public List<DespesaFixa> ListarDespesaFixa(bool hasSomenteAtivas)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                if (hasSomenteAtivas)
                    return uofw.DespesaFixa.Listar(df => df.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && df.CodigoTipoSituacaoDespesaFixa == (short)TipoSituacaoEnum.Ativo,
                        df => df.DiaVencimentoDespesaFixa, true, 
                        df => df.NaturezaContaDespesaFixa);
                else
                    return uofw.DespesaFixa.Listar(df => df.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade, 
                        df => df.DiaVencimentoDespesaFixa, true, 
                        df => df.NaturezaContaDespesaFixa);
            } 
        }

        /// <summary>
        /// Recupera a lista de despesas mensais.
        /// </summary>
        /// <returns></returns>
        public List<DespesaMensal> ListarDespesaMensal(FiltroDespesaMensal filtro)
        {
            List<DespesaMensal> listaResult = null;
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                listaResult = uofw.DespesaMensal.ListarDespesaPorFiltro(ClienteListener.UsuarioLogado.IdEntidade, filtro);
            } 
            return listaResult;
        }

        /// <summary>
        /// Recupera uma despesa fixa da base pelo Id informado.
        /// </summary>
        /// <param name="id">Identificador da despesa fixa.</param>
        /// <param name="hasCarregarHistorico">Indicador de carregamento do historico de atualização da despesa fixa.</param>
        /// <returns></returns>
        public DespesaFixa ObterDespesaFixa(int id, bool hasCarregarHistorico = false)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                if (hasCarregarHistorico)
                    return uofw.DespesaFixa.Obter(q => q.Id == id, n => n.ListaHistoricoDespesaFixa);
                return uofw.DespesaFixa.ObterPorId(id);
            }
        }

        /// <summary>
        /// Grava as alterações realizadas na despesa fixa em banco.
        /// </summary>
        /// <param name="despesaFixa">Objeto com as informações da despesa fixa.</param>
        /// <param name="hasGravarHistorico">Flag (indicador) que diz se precisa gravar histórico de alteração da despesa fixa.</param>
        public void GravarDespesaFixa(DespesaFixa despesaFixa, bool hasGravarHistorico = false)
        {
            ValidarDespesaFixa(despesaFixa);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                if (hasGravarHistorico)
                {
                    HistoricoDespesaFixa historicoDespesaFixa = new HistoricoDespesaFixa();
                    historicoDespesaFixa.IdDespesaFixa = despesaFixa.Id;
                    historicoDespesaFixa.ValorHistoricoDespesaFixa = despesaFixa.ValorDespesaFixa;
                    historicoDespesaFixa.DataHoraRegistroHistoricoDespesaFixa = DateTime.Now;
                    uofw.HistoricoDespesaFixa.Incluir(historicoDespesaFixa);
                }
                uofw.DespesaFixa.Alterar(despesaFixa);
                uofw.SalvarAlteracoes();
            }
        }

        /// <summary>
        /// Remove uma despesa fixa, verificando se existe alguma despesa mensal vinculada a ela, 
        /// caso exista a exclusão será apenas lógica, caso não será uma exclusão física, removendo 
        /// o registro do banco de dados.
        /// </summary>
        /// <param name="idDespesaFixa">Identificador da despesa que será removida.</param>
        public void RemoverDespesaFixa(int idDespesaFixa)
        {
            if (idDespesaFixa == 0)
                throw new NegocioException("Identificador da despesa fixa mensal não informado.");

            DespesaFixa _despesaFixa = null;
            bool hasVinculos = false;
            using (IUnitOfWork uofw = new UnitOfWork())
            {

                _despesaFixa = uofw.DespesaFixa.ObterPorId(idDespesaFixa);
                if (_despesaFixa == null)
                    throw new NegocioException(string.Format("Não foi possível encontrar a despesa fixa com o Id [{0}].", _despesaFixa.Id));
                
                //Verificar vinculo em despesas mensais.
                var qtd = uofw.DespesaMensal.QuantRegistros(dm => dm.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && dm.DespesaFixa.Id == _despesaFixa.Id, dm => dm.DespesaFixa);
                hasVinculos = (qtd != 0);
                if (!hasVinculos)
                {//Verificar vinculo no histórico de despesas.
                    qtd = uofw.HistoricoDespesaFixa.QuantRegistros(q => q.IdDespesaFixa == _despesaFixa.Id);
                    hasVinculos = (qtd != 0);
                }

            }

            using (IUnitOfWork uofw = new UnitOfWork())
            {
                if (hasVinculos)
                {//Exclusão lógica.
                    _despesaFixa.CodigoTipoSituacaoDespesaFixa = (short)TipoSituacaoEnum.Inativo;
                    uofw.DespesaFixa.Alterar(_despesaFixa);
                }
                else
                {//Nenhuma despesa mensal vinculada... exclusão física.
                    uofw.DespesaFixa.Excluir(_despesaFixa);
                }

                uofw.SalvarAlteracoes();

            } 
        }

        /// <summary>
        /// Recupera o total de despesa de um intervalo de datas (inicial e final), conforme o indicador informado:
        /// - 0 = Total Geral das Despesas;
        /// - 1 = Total de Despesas Pagas;
        /// - 2 = Total de Despesas Abertas;
        /// - 3 = Total de Despesas Vencidas;
        /// </summary>
        /// <param name="dataInicialFiltro">Data da Inicial do filtro da recuperação do total.</param>
        /// <param name="dataFinalFiltro">Data da Final do filtro da recuperação do total.</param>
        /// <param name="indTipoTotalDespesa">Indicador de retorno do valor total de despesa.</param>
        /// <returns></returns>
        public decimal ObterTotalDespesa(DateTime dataInicialFiltro, DateTime dataFinalFiltro, int indTipoTotalDespesa)
        {
            if (dataInicialFiltro == null)
                throw new NegocioException("Data inicial para recuperação do total de despesas nula.");

            if (dataFinalFiltro == null)
                throw new NegocioException("Data final para recuperação do total de despesas nula.");

            int idEntidade = ClienteListener.UsuarioLogado.IdEntidade;

            using (IUnitOfWork uofw = new UnitOfWork())
            {
                decimal totalGeral = 0;
                if (indTipoTotalDespesa == 0)
                {
                    totalGeral += uofw.DespesaMensal.ObterTotalDespesa(idEntidade, dataInicialFiltro, dataFinalFiltro, 1); //Pagas (Liquidadas);
                    totalGeral += uofw.DespesaMensal.ObterTotalDespesa(idEntidade, dataInicialFiltro, dataFinalFiltro, 2); //Abertas;
                    totalGeral += uofw.DespesaMensal.ObterTotalDespesa(idEntidade, dataInicialFiltro, dataFinalFiltro, 3); //Vencidas;
                    return totalGeral;
                }
                //Recupera o total pelo Indicador de Total informado.
                totalGeral = uofw.DespesaMensal.ObterTotalDespesa(idEntidade, dataInicialFiltro, dataFinalFiltro, indTipoTotalDespesa);
                return totalGeral;
            }
        }

        /// <summary>
        /// Calcula do valor total da despesa liquidada, aplicando o desconto, juros e multa.
        /// </summary>
        /// <param name="valorDespesa">Valor da despesa registrada (obrigatorio).</param>
        /// <param name="valorDescontoLiquidacao">Valor aplicado no desconto da despesa na liquidação (opcional).</param>
        /// <param name="valorJurosMultaLiquidacao">Valor de juros e multa aplicado a despesa na liquidação (opcional).</param>
        /// <returns></returns>
        public decimal CalcularTotalDespesaLiquidada(decimal valorDespesa, decimal? valorDescontoLiquidacao, decimal? valorJurosMultaLiquidacao)
        {
            decimal valorTotal = valorDespesa;
            valorTotal += valorJurosMultaLiquidacao.HasValue ? valorJurosMultaLiquidacao.Value : 0;
            valorTotal -= valorDescontoLiquidacao.HasValue ? valorDescontoLiquidacao.Value : 0;
            return valorTotal;
        }

        /// <summary>
        /// Recupera uma despesa mensal pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador da despesa mensal.</param>
        /// <returns></returns>
        public DespesaMensal ObterDespesaMensal(int id)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                return uofw.DespesaMensal.Obter(dm => dm.Id == id, dm => dm.NaturezaContaDespesa);
            }
        }

        /// <summary>
        /// Realiza o estorno de um despesa mensal liquidada.
        /// </summary>
        /// <param name="idDespesa">Identificador da despesa a ser estornada.</param>
        public void EstornarDespesaMensal(int idDespesa)
        {
            if (idDespesa == 0)
            {
                throw new NegocioException("Identificador da despesa mensal não informado, sem essa informação não é possível estornar.");
            }

            DespesaMensal despesa = ObterDespesaMensal(idDespesa);
            if (despesa.IsDespesaLiquidada == false)
            {
                throw new NegocioException("Despesa mensal sinalizada como Não Liquidada, estorno não necessário.");
            }

            string observacaoDespesa = "Despesa foi liquidada em [{0}] com valor total de liquidação de [{1}], desconto de [{2}] e juros/multas de [{3}], estorno realizado a pedido em [{4}].";
            observacaoDespesa = string.Format(observacaoDespesa,
                despesa.DataHoraLiquidacaoDespesa.HasValue ? despesa.DataHoraLiquidacaoDespesa.Value.ToString("dd/MM/yyyy") : "Não informado",
                despesa.ValorTotalLiquidacaoDespesa.HasValue ? despesa.ValorTotalLiquidacaoDespesa.Value.ToString("c") : ((decimal)0).ToString("c"),
                despesa.ValorDescontoLiquidacaoDespesa.HasValue ? despesa.ValorDescontoLiquidacaoDespesa.Value.ToString("c") : ((decimal)0).ToString("c"),
                despesa.ValorMultaJurosLiquidacaoDespesa.HasValue ? despesa.ValorMultaJurosLiquidacaoDespesa.Value.ToString("c") : ((decimal)0).ToString("c"),
                DateTime.Now.ToString("dd/MM/yyyy"));

            despesa.IsDespesaLiquidada = false;
            despesa.DataHoraLiquidacaoDespesa = null;
            despesa.ValorDescontoLiquidacaoDespesa = null;
            despesa.ValorMultaJurosLiquidacaoDespesa = null;
            despesa.ValorTotalLiquidacaoDespesa = null;
            despesa.TextoObservacaoDespesa = observacaoDespesa;

            GravarDespesaMensal(despesa);

        }

        /// <summary>
        /// Realizar a alteração das informações da despesa mensal, conforme as informações enviadas.
        /// </summary>
        /// <param name="despesa">Informações da despesa a ser alterada.</param>
        public void GravarDespesaMensal(DespesaMensal despesa)
        {
            ValidarDespesa(despesa);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                uofw.DespesaMensal.Alterar(despesa);
                uofw.SalvarAlteracoes();
            }
        }

        /// <summary>
        /// Remover a despesa pelo seu ID e seus parcelamentos caso informado.
        /// </summary>
        /// <param name="despesa">Objeto com as informações da despesa.</param>
        public void RemoverDespesaMensal(int idDespesa, bool hasRemoverParcelamento)
        {
            
            if (idDespesa == 0)
                throw new NegocioException("Identificador da despesa mensal não informado.");

            List<DespesaMensal> listDespesaParaRemover = new List<DespesaMensal>();

            DespesaMensal _despesa = ObterDespesaMensal(idDespesa);
            if (_despesa == null)
                throw new NegocioException(string.Format("Não foi encontrado a despesa mensal com o Id [{0}] para remoção.", idDespesa));
            if (_despesa.IsDespesaLiquidada)
                throw new NegocioException("Despesa mensal já liquidada, para remover a despesa, por favor realize seu estorno.");

            if (hasRemoverParcelamento)
            {//Verificar se no parcelamento existem despesas 

                int qtdParcelasLiquidadas = ObterQuantParcelasLiquidadasPorCodigoDespesaParcelada(_despesa.CodigoDespesaParcelada.Value);
                if (qtdParcelasLiquidadas > 0)
                    throw new NegocioException("Existem parcelas da despesa mensal já liquidada, para remover a despesa, por favor realize seu estorno das parcelas.");
                //Recupera a lista parcelas vinculadas a despesas.
                listDespesaParaRemover = ListarDespesasParcelamento(_despesa.CodigoDespesaParcelada.Value);
            }
            else
            {
                listDespesaParaRemover.Add(_despesa);
            }

            ChequeNegocio negocioCheque = new ChequeNegocio(ClienteListener);

            using (IUnitOfWork uofw = new UnitOfWork())
            {

                foreach (var item in listDespesaParaRemover)
                {
                    uofw.DespesaMensal.Excluir(item);
                    //Verificar o tipo de despesa para remover as referências de Cheques, se houver.
                    if (item.CodigoTipoFormaLiquidacao == (short)TipoFormaLiquidacaoEnum.ChequeAVista || item.CodigoTipoFormaLiquidacao == (short)TipoFormaLiquidacaoEnum.ChequePreDatado)
                    {
                        negocioCheque.CancelarChequePorRemocaoDespesaMensal(item.CodigoVinculoFormaLiquidacao.Value, item.DescricaoDespesa);
                    }
                }
                uofw.SalvarAlteracoes();
            }

        }

        /// <summary>
        /// Recupera a quantidade de parcelas liquidadas pelo código de despesa parcelada.
        /// </summary>
        /// <param name="codigoDespesaParcelada">Código identificador do parcelameneto.</param>
        /// <returns></returns>
        public int ObterQuantParcelasLiquidadasPorCodigoDespesaParcelada(int codigoDespesaParcelada)
        {
            int qtdResult = 0;
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                qtdResult = uofw.DespesaMensal.QuantRegistros(w => w.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade &&
                                                              w.CodigoDespesaParcelada == codigoDespesaParcelada && w.IsDespesaLiquidada == true);
            }
            return qtdResult;
        }

        /// <summary>
        /// Realiza a liquidação de uma despesa informada pelo seu identificador.
        /// </summary>
        /// <param name="idDespesa">Identificador da despesa a ser liquidada.</param>
        /// <param name="dataLiquidacao">Data de liquidação da despesa.</param>
        /// <param name="valorDesconto">Valor de desconto aplicado na liquidação da despesa.</param>
        /// <param name="valorMultaJuros">Valor de juros/multas aplicados na liquidação da despesa.</param>
        /// <param name="observacao">Texto de observação da liquidação da despesa.</param>
        public void LiquidarDespesa(int idDespesa, DateTime dataLiquidacao, decimal? valorDesconto, decimal? valorJurosMulta, string observacao)
        {
            if (idDespesa == 0)
                throw new NegocioException("Identificador da despesa mensal não informado.");
            if (dataLiquidacao.CompareTo(DateTime.Now.Date) > 0)
                throw new NegocioException("Data de liquidação da despesa deve ser maior ou igual a data atual.");

            DespesaMensal _despesa = ObterDespesaMensal(idDespesa);
            if (_despesa == null)
                throw new NegocioException(string.Format("Despesa mensal com o Id [{0}] não encontrada para liquidação.", idDespesa));
            if (_despesa.IsDespesaLiquidada)
                throw new NegocioException("Despesa mensal já liquidada, alterações não permitidas.");

            //Calcular o valor total da liquidação da despesa.
            decimal valorTotalLiquidacao = CalcularTotalDespesaLiquidada(_despesa.ValorDespesa, valorDesconto, valorJurosMulta);

            using (IUnitOfWork uofw = new UnitOfWork())
            {

                _despesa.DataHoraLiquidacaoDespesa = dataLiquidacao;
                _despesa.ValorDescontoLiquidacaoDespesa = valorDesconto;
                _despesa.ValorMultaJurosLiquidacaoDespesa = valorJurosMulta;
                _despesa.ValorTotalLiquidacaoDespesa = valorTotalLiquidacao;
                _despesa.IsDespesaLiquidada = true;
                _despesa.TextoObservacaoDespesa = observacao;

                uofw.DespesaMensal.Alterar(_despesa);
                uofw.SalvarAlteracoes();

            }
        }

        /// <summary>
        /// Recupera a lista de totais de despesas vencidas nos meses anteriores ao corrente.
        /// </summary>
        /// <param name="mesCorrente">Mês corrente para verificação das despesas vencidas a partir dele, ele não será incluído na busca.</param>
        /// <param name="anoCorrente">Ano corrente para verificação das despesas vencidas a partir dele, ele não será incluído na busca.</param>
        /// <returns></returns>
        public List<DespesaMensal> ListarTotaisDespesasVencidasMesesAnteriores(int mesCorrente, int anoCorrente)
        {
            List<DespesaMensal> listaResult = null;
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                var dataBuscaAnterior = new DateTime(anoCorrente, mesCorrente, 1);
                listaResult = uofw.DespesaMensal.ListarTotaisDespesasVencidas(ClienteListener.UsuarioLogado.IdEntidade, dataBuscaAnterior);
            }
            return listaResult;
        }

        /// <summary>
        /// Recupera a quantidade de parcelas de uma despesa, a partir de uma código de identificação das parcelas.
        /// </summary>
        /// <param name="codigoDespesaParcelada">Código de identificação de parcelamento da despesa.</param>
        /// <returns></returns>
        public int ObterQtdParcelasDespesa(int codigoDespesaParcelada)
        {
            int qtd = 0;
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                qtd = uofw.DespesaMensal.QuantRegistros(w => w.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && w.CodigoDespesaParcelada == codigoDespesaParcelada);
            }
            return qtd;
        }

        /// <summary>
        /// Recupera a lista de despesa de um parcelamento, a partir de uma código de identificação das parcelas.
        /// </summary>
        /// <param name="codigoDespesaParcelada">Código de identificação de parcelamento da despesa.</param>
        /// <returns></returns>
        public List<DespesaMensal> ListarDespesasParcelamento(int codigoDespesaParcelada)
        {
            List<DespesaMensal> listaResult = null;
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                listaResult = uofw.DespesaMensal.Listar(w => w.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && w.CodigoDespesaParcelada == codigoDespesaParcelada, null, true, i => i.NaturezaContaDespesa);
            }
            return listaResult;
        }

    }

}
