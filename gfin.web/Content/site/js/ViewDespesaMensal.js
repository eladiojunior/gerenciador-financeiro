var submit_sem_parar = false;
$(function () {
    listarDespesaMensal();
    $("#dtpDataVencimentoDespesa").datetimepicker({
        locale: 'pt-br',
        format: 'DD/MM/YYYY'
    });
    verificarParcelamento($("#chkParcelada").prop('checked'));
    $('#chkParcelada').on('click', function () {
        verificarParcelamento($(this).prop('checked'));
    });
    $("#CodigoFormaPagamentoDespesaMensal").change(function () {
        var idForma = $(this).val();
        var fieldChkParcelada = $("#chkParcelada");
        fieldChkParcelada.removeAttr("disabled");
        if (idForma == 3) {//Forma: Cheque à Vista
            fieldChkParcelada.attr("disabled", true);
            fieldChkParcelada.attr("checked", false);
            verificarParcelamento(fieldChkParcelada.attr("checked"));
        }
    });
    aplicarControlesSubmitRegistroDespesa();
});

//********************************************************************************************
//** Despesa Mensal - Geral
// -------------------------------------------------------------------------------------------
function aplicarControlesSubmitRegistroDespesa() {
    $("#formDespesaMensal").on("submit", function () {
        if (submit_sem_parar) //Submeter formulario diretamente...
            return true;
        var codigoFormaLiquidacao = parseInt($("#CodigoFormaPagamentoDespesaMensal").val());
        var hasDespesaParcelada = $("#chkParcelada").prop('checked');
        if (codigoFormaLiquidacao == 1) {//Liquidacao: Dinheiro
            return despesaDinheiro(hasDespesaParcelada);
        } else if (codigoFormaLiquidacao == 2) {//Liquidacao: Cartão de Crédito
            despesaCartaoCredito(hasDespesaParcelada, 0);
        } else if (codigoFormaLiquidacao == 3) {//Liquidação: Cheque à Vista
            despesaChequeAVista();
        } else if (codigoFormaLiquidacao == 4) {//Liquidação: Cheque Pré-Datado
            despesaChequePre(hasDespesaParcelada);
        } else if (codigoFormaLiquidacao == 5) {//Liquidação: Boleto
            if (hasDespesaParcelada) {
                despesaBoletoCobranca();
                return false;
            }
            return true;
        } else if (codigoFormaLiquidacao == 6) {//Liquidação: Débito em Conta Corrente
            despesaDebitoContaCorrente(hasDespesaParcelada);
        } else if (codigoFormaLiquidacao == 7) {//Liquidação: Fatura
            if (hasDespesaParcelada) {
                despesaFatura();
                return false;
            }
            return true;
        } else {
            return true;
        }
        return false;
    });
}
function listarDespesaMensal(dataFiltro) {
    if (dataFiltro == null) dataFiltro = "";
    $.ajax({
        cache: false,
        type: "GET",
        url: "/DespesaMensal/JsonListarDespesaMensal",
        dataType: "json",
        data: { dataFiltro: dataFiltro },
        success: function (result) {
            if (result.HasErro) {
                exibirMensagem(result.Erros, true);
                return;
            }
            $('#listaDespesaMensal').html(result.Model);
            aplicarListenerViewDespesaMensal();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });
}
function aplicarListenerViewDespesaMensal() {
    $(".linkRemoverDespesaMensal").click(function () {
        var idDespesaMensal = $(this).data("id");
        obterDespesaMensalParaRemocao(idDespesaMensal);
    });
    $(".filtroDespesas").click(function () {
        var filtro = $(this).data("filtro");
        listarDespesaMensal(filtro);
    });
    $(".filtroReceitasSelect").change(function () {
        var filtro = $(this).val();
        listarDespesaMensal(filtro);
    });
    $(".linkDetalharDespesaParcelada").click(function () {
        var idDespesaMensal = $(this).data("id");
        detalharDespesaParcelada(idDespesaMensal);
    });
    $(".linkEstornarDespesaLiquidada").click(function () {
        var idDespesaMensal = $(this).data("id");
        //Chamar método do JS: ViewDespesaMensalLiquidar.js
        confirmarEstornoDespesaLiquidada(idDespesaMensal);
    });
    //Chamar método do JS: ViewDespesaMensalLiquidar.js
    listenerLiquidarDespesa();
    listenerDetalharDespesaLiquidada();
}
function obterDespesaMensalParaRemocao(idDespesa) {
    $.ajax({
        type: "GET",
        url: "DespesaMensal/JsonConfirmarRemocao",
        cache: false,
        dataType: "json",
        data: { idDespesa: idDespesa },
        success: function (result) {
            if (result.HasErro) {
                exibirAlerta("Erro ao carregar despesa mensal para confirmar remoção.", result.Erros, true);
                return;
            }
            $('#removerDespesaMensal').html(result.Model);
            showModalConfirmacaoRemocaoDespesa();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
    });
}
function showModalConfirmacaoRemocaoDespesa() {
    limparMensagem();
    $("#modalConfirmarRemocaoDespesaMensal").modal();
    $("#modalConfirmarRemocaoDespesaMensal").on('hidden.bs.modal', function () {
        $('#removerDespesaMensal').html("");
        limparMensagem();
    });
    $('#btnConfirmarDespesa').attr("disabled", false);
    $('#btnConfirmarDespesa').click(function () {
        removerDespesaMensal();
    });
}
function removerDespesaMensal() {
    var form = $("#removerForm");
    if (!$("form")[0].checkValidity()) {//Validação inválida;
        return;
    }
    $.ajax({
        type: "POST",
        url: "/DespesaMensal/JsonRemoverDespesaMensal",
        cache: false,
        dataType: "json",
        data: form.serialize(),
        success: function (result) {
            if (result.HasErro) {
                exibirAlerta("Erro ao remover a despesa mensal.", result.Erros, true);
                return;
            }
            $('#btnConfirmarDespesa').attr("disabled", true);
            exibirMensagem(result.Mensagem, false);
            listarDespesaMensal();
            setTimeout(function () { $("#modalConfirmarRemocaoDespesaMensal").modal('hide'); }, 2000);
        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });
}
function detalharDespesaParcelada(idDespesa) {
    $.ajax({
        type: "GET",
        url: "DespesaMensal/JsonListarParcelamentoDespesa",
        cache: false,
        dataType: "json",
        data: { idDespesa: idDespesa },
        success: function (result) {
            if (result.HasErro) {
                exibirAlerta("Erro ao carregar detalhamento do parcelamento da despesa mensal.", result.Erros, true);
                return;
            }
            $('#listaDespesaMensalParceladas').html(result.Model);
            showModalDetalhamentoDespesaParcelada();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
    });
}
function showModalDetalhamentoDespesaParcelada() {
    limparMensagem();
    $("#modalListarParcelamentoDespesa").modal();
    $("#modalListarParcelamentoDespesa").on('hidden.bs.modal', function () {
        $('#listaDespesaMensalParceladas').html("");
        limparMensagem();
    });
}
function verificarParcelamento(checked) {
    var qtd = $('#QtdParcelasDespesa');
    var isReadOnly = !checked;
    qtd.prop('readonly', isReadOnly);
    isReadOnly ? $('#labelValorDespesa').html("Valor Despesa") : $('#labelValorDespesa').html("Valor Parcela");
    isReadOnly ? qtd.attr("required", "true") : qtd.attr("required", "false");
    isReadOnly ? $('#ValorDespesa').focus() : qtd.focus();
}
function validarBancoAgenciaContaCorrente() {
    var fieldBanco = $("#IdBancoAgenciaContaCorrente");
    var idBanco = parseInt(fieldBanco.val());
    return (idBanco && idBanco != 0);
}
//********************************************************************************************
//** Verifica se a despesa que esta sendo registrada esta com data de vencimento anterior
//** a data atual do sistema, identificando isso permite o registro da despesa jah liquidada.
// -------------------------------------------------------------------------------------------
function verificarDespesaRegistradaJaVencida() {
    var dataSistema = new Date();
    var dataVencimentoDespesa = new Date($('#dtpDataVencimentoDespesa').data("DateTimePicker").date());
    if (dataSistema >= dataVencimentoDespesa) {
        var resultConfirmacao = false;
        exibirConfirmacao("Registro de despesa",
            "Deseja registrar essa despesa mensal como liquidada?",
            function () {
                $("#IsDespesaLiquidada").val('true');
                $("#DataVenvimentoDespesaLiquidacao").val($("#DataVencimentoDespesa").val());
                $("#ValorDespesaLiquidacao").val($("#ValorDespesa").val());
                $("#ValorTotalLiquidacaoDespesa").val($("#ValorDespesa").val());
                //Campos da liquidação.
                $("#dtpDataLiquidacaoDespesa").datetimepicker({ locale: 'pt-br', format: 'DD/MM/YYYY' });
                $("#dtpDataLiquidacaoDespesa").val($("#DataVencimentoDespesa").val());
                $("#DataLiquidacaoDespesa").val($("#DataVencimentoDespesa").val());
                $("#modalRegistrarDespesaLiquidada").modal();
            },
            function () {
                //Registrar sem liquida-la!
                submit_sem_parar = true;
                $("#formDespesaMensal").submit();
            }, "modalConfirmacaoDespesaJaLiquidacao");
        return resultConfirmacao;
    }
    return true;
}

//********************************************************************************************
//** Despesa por Dinheiro
// -------------------------------------------------------------------------------------------
function despesaDinheiro(hasParcelada) {
    if (!hasParcelada)
        return verificarDespesaRegistradaJaVencida();
    $.ajax({
        type: "POST",
        url: "/DespesaMensal/JsonRegistrarDespesaParceladaDinheiro",
        cache: false,
        dataType: "json",
        data: $("#formDespesaMensal").serialize(),
        success: function (result) {
            if (result.HasErro) {
                exibirAlerta("Erro ao registrar despesa mensal parcelada.", result.Erros, true);
                return;
            }
            $('#registrarDespesaParceladaComDinheiro').html(result.Model);
            showModalParcelamentoDinheiro();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });
    return false;
}
function showModalParcelamentoDinheiro() {
    limparMensagem();
    aplicarMascarasFormatacao();
    aplicarVerificacaoValorParcela_Dinheiro();
    aplicarControlesRegistroDespesaParceladaComDinheiro();
    $("#modalRegistrarDespesaParceladaDinheiro").modal();
    $("#modalRegistrarDespesaParceladaDinheiro").on('hidden.bs.modal', function () {
        $('#registrarDespesaParceladaComDinheiro').html("");
        limparMensagem();
    });
}
function aplicarControlesRegistroDespesaParceladaComDinheiro() {
    $("#btnRegistrarDespesaParceladaDinheiro").click(function () {
        $("#modalRegistrarDespesaParceladaDinheiro").modal('hide');
        submit_sem_parar = true;
        $("#formDespesaMensal").attr("action", "/DespesaMensal/RegistrarDespesaParcelada");
        $("#formDespesaMensal").submit();
    });
}
function aplicarVerificacaoValorParcela_Dinheiro() {
    $(".valorParcelaDinheiro").change(function () {
        var valorTotal = 0;
        $(".valorParcelaDinheiro").each(function () {
            var money = $(this).val().replace('.', '').replace(',', '.');
            valorTotal += parseFloat(money) * 100;
        });
        var money = (valorTotal / 100).toFixed(2).replace(/./g, function (c, i, a) {
            return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
        });
        money = money.replace(',', '-').replace('.', ',').replace('-', '.');
        $("#ValorTotalParcelamento_Dinheiro").val(money);
    });
}

//********************************************************************************************
//** Despesa por Cartão de Credito
// -------------------------------------------------------------------------------------------
function despesaCartaoCredito(hasParcelada, idContaCorrente) {
    if (hasParcelada) {
        $.ajax({
            type: "POST", url: "DespesaMensal/DespesaParceladaCartaoCredito", cache: false,
            dataType: "html", data: $("#formDespesaMensal").serialize(),
            success: function (data) {
                $('#registrarDespesaParceladaComCartaoCredito').html(data);
                showModalRegistraDespesaParceladaComCartaoCredito();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
        });
    } else {
        $.ajax({
            type: "POST", url: "DespesaMensal/DespesaCartaoCredito", cache: false,
            dataType: "html", data: { idContaCorrente: idContaCorrente },
            success: function (data) {
                $('#registrarDespesaComCartaoCredito').html(data);
                showModalRegistraDespesaComCartaoCredito();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
        });
    }
}
function showModalRegistraDespesaParceladaComCartaoCredito() {
    limparMensagem();
    aplicarMascarasFormatacao();
    aplicarVerificacaoValorParcela_Cartao();
    aplicarControlesRegistroDespesaParceladaComCartaoCredito();
    $("#modalRegistrarDespesaParceladaCartaoCredito").modal();
    $("#modalRegistrarDespesaParceladaCartaoCredito").on('hidden.bs.modal', function () {
        $('#registrarDespesaParceladaComCartaoCredito').html("");
        limparMensagem();
    });
}
function aplicarVerificacaoValorParcela_Cartao() {
    $(".valorParcelaCartao").change(function () {
        var valorTotal = 0;
        $(".valorParcelaCartao").each(function () {
            var money = $(this).val().replace('.', '').replace(',', '.');
            valorTotal += parseFloat(money) * 100;
        });
        var money = (valorTotal / 100).toFixed(2).replace(/./g, function (c, i, a) {
            return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
        });
        money = money.replace(',', '-').replace('.', ',').replace('-', '.');
        $("#ValorTotalParcelamento_Cartao").val(money);
    });
}
function aplicarControlesRegistroDespesaParceladaComCartaoCredito() {
    $("#btnRegistrarDespesaParceladaCartaoCredito").click(function () {
        var rdbCartao = $("input[name=rdbCartaoCredito]:checked");
        if (rdbCartao.val() == null) {
            exibirMensagem("Nenhum cartão de crédito selecionado.", true);
        } else {
            $("#IdVinculoFormaLiquidacao").val(rdbCartao.val());
            $("#modalRegistrarDespesaParceladaCartaoCredito").modal('hide');
            submit_sem_parar = true;
            $("#formDespesaMensal").attr("action", "/DespesaMensal/RegistrarDespesaParcelada");
            $("#formDespesaMensal").submit();
        }
    });
    $("#IdBancoAgenciaContaCorrente").change(function () {
        $.ajax({
            type: "POST",
            url: "CartaoCredito/ListarCartaoContaCorrente",
            cache: false, dataType: "html",
            data: { idContaCorrente: $("#IdBancoAgenciaContaCorrente").val() },
            success: function (data) {
                $('#divListaCartoesCreditoContaCorrente').html(data);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
        });
    });
}
function showModalRegistraDespesaComCartaoCredito() {
    limparMensagem();
    aplicarMascarasFormatacao();
    aplicarControlesRegistroDespesaComCartaoCredito();
    $("#modalRegistrarDespesaCartaoCredito").modal();
    $("#modalRegistrarDespesaCartaoCredito").on('hidden.bs.modal', function () {
        $('#registrarDespesaComCartaoCredito').html("");
        limparMensagem();
    });
}
function aplicarControlesRegistroDespesaComCartaoCredito() {
    $("#btnRegistrarDespesaCartaoCredito").click(function () {
        var rdbCartao = $("input[name=rdbCartaoCredito]:checked");
        if (rdbCartao.val() == null) {
            exibirMensagem("Nenhum cartão de crédito selecionado.", true);
        } else {
            $("#IdVinculoFormaLiquidacao").val(rdbCartao.val());
            $("#modalRegistrarDespesaCartaoCredito").modal('hide');
            submit_sem_parar = true;
            $("#formDespesaMensal").submit();
        }
    });
    $("#IdBancoAgenciaContaCorrente").change(function () {
        var idContaCorrente = $(this).val();
        $.ajax({
            type: "POST",
            url: "CartaoCredito/ListarCartaoContaCorrente",
            cache: false, dataType: "html",
            data: { idContaCorrente: idContaCorrente },
            success: function (data) {
                $('#divListaCartoesCreditoContaCorrente').html(data);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
        });
    });
}

//********************************************************************************************
//** Despesa por Cheque à Vista
// -------------------------------------------------------------------------------------------
function despesaChequeAVista() {
    $.ajax({
        type: "POST", url: "DespesaMensal/DespesaChequeAVista", cache: false,
        dataType: "html", data: $("#formDespesaMensal").serialize(),
        success: function (data) {
            $('#registrarDespesaComChequeAVista').html(data);
            showModalRegistraDespesaComChequeAVista();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
    });
}
function showModalRegistraDespesaComChequeAVista() {
    limparMensagem();
    aplicarMascarasFormatacao();
    aplicarVerificacaoNumeroChequeAVista();
    aplicarControlesRegistroDespesaComChequeAVista();
    $("#modalRegistrarDespesaChequeAVista").modal();
    $("#modalRegistrarDespesaChequeAVista").on('hidden.bs.modal', function () {
        $('#registrarDespesaComChequeAVista').html("");
        limparMensagem();
    });
}
function aplicarVerificacaoNumeroChequeAVista() {
    $(".verificarSituacaoNumeroCheque").click(function () {
        var numeroCheque = parseInt($("#NumeroCheque").val());
        if (!numeroCheque || numeroCheque == 0) {
            exibirMensagem("Número do cheque não informado.");
            $("#NumeroCheque").focus();
            return false;
        }
        var hasValid = validarBancoAgenciaContaCorrente();
        if (!hasValid) {
            exibirMensagem("Nenhum Banco/Agência/Conta Corrente selecionado.");
            $("#IdBancoAgenciaContaCorrente").focus();
            return false;
        }
        limparMensagem();
        var numeroChequeAtual = parseInt($("#NumeroCheque").data("numero-cheque"));
        if (numeroChequeAtual == numeroCheque) { return; }
        var situacaoCheque = $('.situacaoChequeAVista');
        $.ajax({
            url: "DespesaMensal/JsonVerificarNumeroCheque",
            type: "GET",
            cache: false,
            data: {
                idContaCorrente: $("#IdBancoAgenciaContaCorrente").val(),
                numeroCheque: numeroCheque
            },
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.MensagemErro, true);
                    return;
                }
                //Atualizar situacao cheque...
                atualizarSituacaoCheque(situacaoCheque, result.Data);
            },
            error: function () {
                exibirMensagem('Erro, não possível verificar o número do cheque informado.', true);
            }
        });
        return true;
    });
}
function aplicarControlesRegistroDespesaComChequeAVista() {
    $("#btnRegistrarDespesaChequeAVista").click(function () {
        var numeroCheque = parseInt($("#NumeroCheque").val());
        if (!numeroCheque || numeroCheque == 0) {
            exibirMensagem("Número do cheque não informado.");
            $("#NumeroCheque").focus();
            return false;
        }
        var isChequeValido = $('.situacaoChequeAVista').data("numero-cheque-utilizavel");
        alert(isChequeValido);
        if (!isChequeValido) {
            exibirMensagem("Número do cheque é inválido ou já foi utilizado.");
            return false;
        }
        $("#IdVinculoFormaLiquidacao").val($("#NumeroCheque").val());
        $("#modalRegistrarDespesaChequeAVista").modal('hide');
        submit_sem_parar = true;
        $("#formDespesaMensal").submit();
    });
}
//********************************************************************************************
//** Despesa por Cheque Pré-Datado
// -------------------------------------------------------------------------------------------
function despesaChequePre(hasParcelada) {
    if (hasParcelada) {
        $.ajax({
            type: "POST",
            url: "DespesaMensal/DespesaParceladaChequePre",
            cache: false,
            dataType: "html",
            data: $("#formDespesaMensal").serialize(),
            success: function (data) {
                $('#registrarDespesaParceladaComChequePre').html(data);
                showModalRegistraDespesaParceladaComChequePre();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
        });
    } else {
        $.ajax({
            type: "POST",
            url: "DespesaMensal/DespesaChequePre",
            cache: false,
            dataType: "html",
            data: $("#formDespesaMensal").serialize(),
            success: function (data) {
                $('#registrarDespesaComChequePre').html(data);
                showModalRegistraDespesaComChequePre();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
        });
    }
}
function showModalRegistraDespesaParceladaComChequePre() {
    limparMensagem();
    aplicarMascarasFormatacao();
    aplicarVerificacaoNumeroChequePreInicial();
    aplicarVerificacaoValorParcelaChequePre();
    aplicarControlesRegistroDespesaParceladaComChequePre();
    $("#modalRegistrarDespesaParceladaChequePre").modal();
    $("#modalRegistrarDespesaParceladaChequePre").on('hidden.bs.modal', function () {
        $('#registrarDespesaParceladaComChequePre').html("");
        limparMensagem();
    });
}
function aplicarVerificacaoNumeroChequePreInicial() {
    $(".verificarSituacaoNumeroChequeInicial").click(function () {
        var numeroCheque = parseInt($("#NumeroChequeInicial").val());
        if (!numeroCheque || numeroCheque == 0) {
            exibirMensagem("Número inicial do cheque não informado.");
            $("#NumeroChequeInicial").focus();
            return false;
        }
        var hasValid = validarBancoAgenciaContaCorrente();
        if (!hasValid) {
            exibirMensagem("Nenhum Banco/Agência/Conta Corrente selecionado.");
            $("#IdBancoAgenciaContaCorrente").focus();
            return false;
        }
        limparMensagem();
        var numeroChequeAtual = parseInt($("#NumeroChequeInicial").data("numero-cheque"));
        if (numeroChequeAtual == numeroCheque) { return; }
        var situacaoCheque = $('.situacaoChequePreInicial');
        $.ajax({
            url: "DespesaMensal/JsonVerificarNumeroCheque",
            type: "GET",
            cache: false,
            data: {
                idContaCorrente: $("#IdBancoAgenciaContaCorrente").val(),
                numeroCheque: numeroCheque
            },
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.MensagemErro, true);
                    return;
                }
                if (result.Data.IsUtilizavel == false)
                {//Cheque inicial não utilizavel...
                    exibirMensagem("Número inicial do cheque inválido ou já utilizado, informe outro número.");
                    $("#NumeroChequeInicial").focus();
                    return;
                }
                //Recupera os demais números de cheques.
                recuperarNumerosChequesParcelamento(numeroCheque);
            },
            error: function () {
                exibirMensagem('Erro, não possível verificar o número do cheque informado.', true);
            }
        });
        return true;
    });
}
function recuperarNumerosChequesParcelamento(numeroInicial) {
    $.ajax({
        url: "DespesaMensal/JsonParcelamentoChequePre",
        type: "GET",
        cache: false,
        data: {
            idContaCorrente: $("#IdBancoAgenciaContaCorrente").val(),
            numeroChequeInicial: numeroInicial,
            qtdParcelas: $("#QtdParcelasDespesa").val()
        },
        success: function (result) {
            if (result.HasErro) {
                exibirMensagem(result.MensagemErro, true);
                return;
            }
            $.each(result.Data, function (index, value) {
                var obj = $("#ParcelasDespesa_" + index + "__NumeroParcela");
                atualizarSituacaoFieldCheque(obj, value);
            });
            aplicarVerificacaoNumeroChequePreParcelas();
        },
        error: function () {
            exibirMensagem('Erro, não possível verificar o número do cheque informado.', true);
        }
    });
}
function aplicarVerificacaoValorParcelaChequePre() {
    $(".valorParcelaChequePre").change(function () {
        var valorTotal = 0;
        $(".valorParcelaChequePre").each(function () {
            var money = $(this).val().replace('.', '').replace(',', '.');
            valorTotal += parseFloat(money) * 100;
        });
        var money = (valorTotal / 100).toFixed(2).replace(/./g, function (c, i, a) {
            return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
        });
        money = money.replace(',', '-').replace('.', ',').replace('-', '.');
        $("#ValorTotalParcelamento_ChequePre").val(money);
    });
}
function aplicarControlesRegistroDespesaParceladaComChequePre() {
    $("#btnRegistrarDespesaParceladaChequePre").click(function () {
        var numeroCheque = parseInt($("#NumeroChequeInicial").val());
        if (!numeroCheque || numeroCheque == 0) {
            exibirMensagem("Número inicial do cheque não informado.");
            $("#NumeroChequeInicial").focus();
            return false;
        }
        var isChequeValido = false;
        $(".numeroCheque").each(function (index) {
            isChequeValido = $(this).data("numero-cheque-utilizavel");
            numeroCheque = $(this).data("numero-cheque");
            if (!isChequeValido)
            {
                exibirMensagem("Número do cheque [" + numeroCheque + "] é inválido ou já foi utilizado.");
                $('.numeroCheque[data-numero-cheque="' + numeroCheque + '"]').focus();
                return false;
            }
        });
        if (isChequeValido) {
            $("#modalRegistrarDespesaParceladaChequePre").modal('hide');
            submit_sem_parar = true;
            $("#formDespesaMensal").attr("action", "/DespesaMensal/RegistrarDespesaParcelada");
            $("#formDespesaMensal").submit();
        }
    });
}
function aplicarVerificacaoNumeroChequePreParcelas() {
    $(".numeroCheque").change(function () {
        var numeroChequeNovo = parseInt($(this).val());
        if (numeroChequeNovo && numeroChequeNovo != 0) {//Verificar número do cheque.
            var numeroChequeAtual = parseInt($(this).data("numero-cheque"));
            if (numeroChequeAtual == numeroChequeNovo) { return; }
            var objNumeroCheque = $(this);
            $.ajax({
                url: "DespesaMensal/JsonVerificarNumeroCheque",
                type: "GET",
                cache: false,
                data: {
                    idContaCorrente: $("#IdBancoAgenciaContaCorrente").val(),
                    numeroCheque: numeroChequeNovo
                },
                success: function (result) {
                    if (result.HasErro) {
                        exibirMensagem(result.MensagemErro, true);
                        return;
                    }
                    //Atualizar situacao cheque...
                    atualizarSituacaoFieldCheque(objNumeroCheque, result.Data);
                },
                error: function () {
                    exibirMensagem('Erro, não possível verificar o número do cheque informado.', true);
                }
            });
        }
    });
}
function showModalRegistraDespesaComChequePre() {
    limparMensagem();
    aplicarMascarasFormatacao();
    aplicarVerificacaoNumeroChequePre();
    aplicarControlesRegistroDespesaComChequePre();
    $("#modalRegistrarDespesaChequePre").modal();
    $("#modalRegistrarDespesaChequePre").on('hidden.bs.modal', function () {
        $('#registrarDespesaComChequePre').html("");
        limparMensagem();
    });
}
function aplicarVerificacaoNumeroChequePre() {
    $(".verificarSituacaoNumeroCheque").click(function () {
        var numeroCheque = parseInt($("#NumeroCheque").val());
        if (!numeroCheque || numeroCheque == 0) {
            exibirMensagem("Número do cheque não informado.");
            $("#NumeroCheque").focus();
            return false;
        }
        var hasValid = validarBancoAgenciaContaCorrente();
        if (!hasValid) {
            exibirMensagem("Nenhum Banco/Agência/Conta Corrente selecionado.");
            $("#IdBancoAgenciaContaCorrente").focus();
            return false;
        }
        limparMensagem();
        var numeroChequeAtual = parseInt($("#NumeroCheque").data("numero-cheque"));
        if (numeroChequeAtual == numeroCheque) { return; }
        var situacaoCheque = $('.situacaoChequePre');
        $.ajax({
            url: "DespesaMensal/JsonVerificarNumeroCheque",
            type: "GET",
            cache: false,
            data: {
                idContaCorrente: $("#IdBancoAgenciaContaCorrente").val(),
                numeroCheque: numeroCheque
            },
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.MensagemErro, true);
                    return;
                }
                //Atualizar situacao cheque...
                atualizarSituacaoCheque(situacaoCheque, result.Data);
            },
            error: function () {
                exibirMensagem('Erro, não possível verificar o número do cheque informado.', true);
            }
        });
        return true;
    });
}
function aplicarControlesRegistroDespesaComChequePre() {
    $("#btnRegistrarDespesaChequePre").click(function () {
        var numeroCheque = parseInt($("#NumeroCheque").val());
        if (!numeroCheque || numeroCheque == 0) {
            exibirMensagem("Número do cheque não informado.");
            $("#NumeroCheque").focus();
            return false;
        }
        var isChequeValido = $('.situacaoChequePre').data("numero-cheque-utilizavel");
        if (!isChequeValido) {
            exibirMensagem("Número do cheque é inválido ou já foi utilizado.");
            return false;
        }
        $("#IdVinculoFormaLiquidacao").val($("#NumeroCheque").val());
        $("#modalRegistrarDespesaChequePre").modal('hide');
        submit_sem_parar = true;
        $("#formDespesaMensal").submit();
    });
}
function atualizarSituacaoCheque(fieldSituacao, objSituacaoCheque) {
    fieldSituacao.removeClass("bg-success");
    fieldSituacao.removeClass("bg-warning");
    fieldSituacao.removeClass("bg-danger");
    fieldSituacao.data("numero-cheque", objSituacaoCheque.NumeroCheque);
    fieldSituacao.data("id-cheque", objSituacaoCheque.IdCheque);
    fieldSituacao.data("numero-cheque-utilizavel", objSituacaoCheque.IsUtilizavel);
    var codigoSituacaoCheque = parseInt(objSituacaoCheque.CodigoSituacaoCheque);
    if (codigoSituacaoCheque != 0 && codigoSituacaoCheque != 1) {//Cheque movimentado, não pode ser utilizado;
        fieldSituacao.addClass("bg-danger");
    }
    else if (codigoSituacaoCheque == 1) {//Registrado e pronto para utilização.
        fieldSituacao.addClass("bg-success");
    }
    else if (codigoSituacaoCheque == 0) {//Não registrado... registrar ao criar despesa.
        fieldSituacao.addClass("bg-warning");
    }
    fieldSituacao.html(objSituacaoCheque.DescicaoSituacaoCheque);
}
function atualizarSituacaoFieldCheque(fieldSituacao, objSituacaoCheque) {
    fieldSituacao.removeClass("situacaoChequeEmitido").removeClass("situacaoChequeNaoRegistrado").removeClass("situacaoChequeRegistrado");
    fieldSituacao.data("numero-cheque", objSituacaoCheque.NumeroCheque);
    fieldSituacao.data("id-cheque", objSituacaoCheque.IdCheque);
    fieldSituacao.data("numero-cheque-utilizavel", objSituacaoCheque.IsUtilizavel);
    var codigoSituacaoCheque = parseInt(objSituacaoCheque.CodigoSituacaoCheque);
    if (codigoSituacaoCheque != 0 && codigoSituacaoCheque != 1) {//Cheque movimentado, não pode ser utilizado;
        fieldSituacao.addClass("situacaoChequeEmitido");
    }
    else if (codigoSituacaoCheque == 1) {//Registrado e pronto para utilização.
        fieldSituacao.addClass("situacaoChequeRegistrado");
    }
    else if (codigoSituacaoCheque == 0) {//Não registrado... registrar ao criar despesa.
        fieldSituacao.addClass("situacaoChequeNaoRegistrado");
    }
    fieldSituacao.val(objSituacaoCheque.NumeroCheque);
    fieldSituacao.prop('readonly', false);
    fieldSituacao.prop('title', objSituacaoCheque.DescicaoSituacaoCheque);
}



//********************************************************************************************
//** Despesa por Boleto de Cobrança
// -------------------------------------------------------------------------------------------
function despesaBoletoCobranca() {
    $.ajax({
        type: "POST",
        url: "DespesaMensal/DespesaParceladaBoleto",
        cache: false,
        dataType: "html",
        data: $("#formDespesaMensal").serialize(),
        success: function (data) {
            $('#registrarDespesaParceladaComBoleto').html(data);
            showModalRegistraDespesaParceladaComBoleto();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
    });
}
function showModalRegistraDespesaParceladaComBoleto() {
    limparMensagem();
    aplicarMascarasFormatacao();
    aplicarVerificacaoValorParcela_Boleto();
    aplicarControlesRegistroDespesaParceladaComBoleto();
    $("#modalRegistrarDespesaParceladaBoleto").modal();
    $("#modalRegistrarDespesaParceladaBoleto").on('hidden.bs.modal', function () {
        $('#registrarDespesaParceladaComBoleto').html("");
        limparMensagem();
    });
}
function aplicarVerificacaoValorParcela_Boleto() {
    $(".valorParcelaBoleto").change(function () {
        var valorTotal = 0;
        $(".valorParcelaBoleto").each(function () {
            var money = $(this).val().replace('.', '').replace(',', '.');
            valorTotal += parseFloat(money) * 100;
        });
        var money = (valorTotal / 100).toFixed(2).replace(/./g, function (c, i, a) {
            return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
        });
        money = money.replace(',', '-').replace('.', ',').replace('-', '.');
        $("#ValorTotalParcelamento_Boleto").val(money);
    });
}
function aplicarControlesRegistroDespesaParceladaComBoleto() {
    $("#btnRegistrarDespesaParceladaBoleto").click(function () {
        $("#modalRegistrarDespesaParceladaBoleto").modal('hide');
        submit_sem_parar = true;
        $("#formDespesaMensal").attr("action", "/DespesaMensal/RegistrarDespesaParcelada");
        $("#formDespesaMensal").submit();
    });
}

//********************************************************************************************
//** Despesa por Débito em Conta Corrente
// -------------------------------------------------------------------------------------------
function despesaDebitoContaCorrente(hasParcelada) {
    if (hasParcelada) {
        $.ajax({
            type: "POST",
            url: "DespesaMensal/DespesaParceladaDebitoConta",
            cache: false,
            dataType: "html",
            data: $("#formDespesaMensal").serialize(),
            success: function (data) {
                $('#registrarDespesaParceladaComDebitoConta').html(data);
                showModalRegistraDespesaParceladaComDebitoConta();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
        });
    } else {
        $.ajax({
            type: "POST",
            url: "DespesaMensal/DespesaDebitoConta",
            cache: false,
            dataType: "html",
            data: $("#formDespesaMensal").serialize(),
            success: function (data) {
                $('#registrarDespesaComDebitoConta').html(data);
                showModalRegistraDespesaComDebitoConta();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
        });
    }
}
function showModalRegistraDespesaParceladaComDebitoConta() {
    limparMensagem();
    aplicarMascarasFormatacao();
    aplicarVerificacaoValorParcela_DebitoConta();
    aplicarControlesRegistroDespesaParceladaComDebitoConta();
    $("#modalRegistrarDespesaParceladaDebitoConta").modal();
    $("#modalRegistrarDespesaParceladaDebitoConta").on('hidden.bs.modal', function () {
        $('#registrarDespesaParceladaComDebitoConta').html("");
        limparMensagem();
    });
}
function showModalRegistraDespesaComDebitoConta() {
    limparMensagem();
    aplicarMascarasFormatacao();
    aplicarControlesRegistroDespesaComDebitoConta();
    $("#modalRegistrarDespesaDebitoConta").modal();
    $("#modalRegistrarDespesaDebitoConta").on('hidden.bs.modal', function () {
        $('#registrarDespesaComDebitoConta').html("");
        limparMensagem();
    });
}
function aplicarVerificacaoValorParcela_DebitoConta() {
    $(".valorParcelaDebitoConta").change(function () {
        var valorTotal = 0;
        $(".valorParcelaDebitoConta").each(function () {
            var money = $(this).val().replace('.', '').replace(',', '.');
            valorTotal += parseFloat(money) * 100;
        });
        var money = (valorTotal / 100).toFixed(2).replace(/./g, function (c, i, a) {
            return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
        });
        money = money.replace(',', '-').replace('.', ',').replace('-', '.');
        $("#ValorTotalParcelamento_DebitoConta").val(money);
    });
}
function aplicarControlesRegistroDespesaParceladaComDebitoConta() {
    $("#btnRegistrarDespesaParceladaDebitoConta").click(function () {
        var hasValid = validarBancoAgenciaContaCorrente();
        if (!hasValid) {
            exibirMensagem("Nenhum Banco/Agência/Conta Corrente selecionado.");
            return false;
        }
        limparMensagem();
        $("#IdVinculoFormaLiquidacao").val($("#IdBancoAgenciaContaCorrente").val());
        $("#modalRegistrarDespesaParceladaDebitoConta").modal('hide');
        submit_sem_parar = true;
        $("#formDespesaMensal").attr("action", "/DespesaMensal/RegistrarDespesaParcelada");
        $("#formDespesaMensal").submit();
    });
}
function aplicarControlesRegistroDespesaComDebitoConta() {
    $("#btnRegistrarDespesaDebitoConta").click(function () {
        var hasValid = validarBancoAgenciaContaCorrente();
        if (!hasValid) {
            exibirMensagem("Nenhum Banco/Agência/Conta Corrente selecionado.");
            return false;
        }
        limparMensagem();
        $("#IdVinculoFormaLiquidacao").val($("#IdBancoAgenciaContaCorrente").val());
        $("#modalRegistrarDespesaDebitoConta").modal('hide');
        submit_sem_parar = true;
        $("#formDespesaMensal").submit();
    });
}

//********************************************************************************************
//** Despesa por Fatura
// -------------------------------------------------------------------------------------------
function despesaFatura() {
    $.ajax({
        type: "POST",
        url: "DespesaMensal/DespesaParceladaFatura",
        cache: false,
        dataType: "html",
        data: $("#formDespesaMensal").serialize(),
        success: function (data) {
            $('#registrarDespesaParceladaComFatura').html(data);
            showModalRegistraDespesaParceladaComFatura();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
    });
}
function showModalRegistraDespesaParceladaComFatura() {
    limparMensagem();
    aplicarMascarasFormatacao();
    aplicarVerificacaoValorParcela_Fatura();
    aplicarControlesRegistroDespesaParceladaComFatura();
    $("#modalRegistrarDespesaParceladaFatura").modal();
    $("#modalRegistrarDespesaParceladaFatura").on('hidden.bs.modal', function () {
        $('#registrarDespesaParceladaComFatura').html("");
        limparMensagem();
    });
}
function aplicarVerificacaoValorParcela_Fatura() {
    $(".valorParcelaFatura").change(function () {
        var valorTotal = 0;
        $(".valorParcelaFatura").each(function () {
            var money = $(this).val().replace('.', '').replace(',', '.');
            valorTotal += parseFloat(money) * 100;
        });
        var money = (valorTotal / 100).toFixed(2).replace(/./g, function (c, i, a) {
            return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
        });
        money = money.replace(',', '-').replace('.', ',').replace('-', '.');
        $("#ValorTotalParcelamento_Fatura").val(money);
    });
}
function aplicarControlesRegistroDespesaParceladaComFatura() {
    $("#btnRegistrarDespesaParceladaFatura").click(function () {
        $("#modalRegistrarDespesaParceladaFatura").modal('hide');
        submit_sem_parar = true;
        $("#formDespesaMensal").attr("action", "/DespesaMensal/RegistrarDespesaParcelada");
        $("#formDespesaMensal").submit();
    });
}

// Views\DespesaMensal\_RegistrarDespesaLiquidadaPartial.cshtml
$("#btnRegistrarDespesaLiquidada").click(function () {
    submit_sem_parar = true;
    $("#formDespesaMensal").submit();
});
$("#ValorDescontoLiquidacaoDespesa").change(function () {
    atualizarValorTotalLiquidacaoDespesa()
});
$("#ValorMultaJurosLiquidacaoDespesa").change(function () {
    atualizarValorTotalLiquidacaoDespesa()
});
function atualizarValorTotalLiquidacaoDespesa() {
    var valorTotalLiquidacao = 0;
    var valorDespesa = $("#ValorDespesaLiquidacao").val().replace('.', '').replace(',', '.').replace('', '0');
    var valorDesconto = $("#ValorDescontoLiquidacaoDespesa").val().replace('.', '').replace(',', '.').replace('', '0');
    var valorMultaJuros = $("#ValorMultaJurosLiquidacaoDespesa").val().replace('.', '').replace(',', '.').replace('', '0');
    valorTotalLiquidacao = valorTotalLiquidacao + parseFloat(valorDespesa);
    valorTotalLiquidacao = valorTotalLiquidacao - parseFloat(valorDesconto);
    valorTotalLiquidacao = valorTotalLiquidacao + parseFloat(valorMultaJuros);
    var strValorTotalLiquidacao = valorTotalLiquidacao.toFixed(2).replace(/./g, function (c, i, a) {
        return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
    });
    strValorTotalLiquidacao = strValorTotalLiquidacao.replace(',', '-').replace('.', ',').replace('-', '.');
    $("#ValorTotalLiquidacaoDespesa").val(strValorTotalLiquidacao);
}
//-----------------------------------------------------------

// Views\DespesaMensal\_ListaDespesaPartial.cshtml
registrarFiltragemDespesas();
function registrarFiltragemDespesas() {
    $(".filtroDespesas").click(function () {
        var filtro = $(this).data("filtro");
        listarDespesas(filtro);
    });
    $(".filtroDespesasSelect").change(function () {
        var filtro = $(this).val();
        listarDespesas(filtro);
    });
}
function listarDespesas(dataFiltro) {
    $.ajax({
        type: "GET",
        url: "DespesaMensal/FiltrarDespesas",
        contentType: "application/html; charset=utf-8",
        dataType: "html",
        data: { dataFiltro: dataFiltro },
        success: function (data) {
            $('#listaDespesas').html(data);
            registrarFiltragemDespesas();
        }
    });
}
//-----------------------------------------------------------