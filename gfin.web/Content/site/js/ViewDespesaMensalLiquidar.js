$(function () {
    listenerLiquidarDespesa();
    listenerDetalharDespesaLiquidada();
});

// Views\DespesaMensal\_ListaDespesaSimplesPartial.cshtml
// Views\DespesaMensal\_ListaDespesaPartial.cshtml
// Views\Conta\_ListaContasPartial.cshtml
function listenerLiquidarDespesa() {
    $(".linkLiquidarDespesa").click(function () {
        var idDespesa = $(this).data("id");
        $.ajax({
            cache: false,
            type: "GET",
            url: "/DespesaMensal/JsonLiquidarDespesaMensal",
            contentType: "application/html",
            dataType: "json",
            data: {
                idDespesa: idDespesa
            },
            success: function (data) {
                if (data.HasErro) {
                    exibirAlerta("Erro na Liquidação", data.Erros, true);
                    return;
                }
                $('#liquidarDespesaMensal').html(data.Model);
                aplicarMascarasFormatacao();
                $("#dtpDataLiquidacaoDespesa").datetimepicker({
                    locale: 'pt-br',
                    format: 'DD/MM/YYYY'
                });
                $('#btnLiquidarDespesa').click(function () {
                    liquidarDespesa();
                });
                showModalLiquidarDespesa();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                exibirAlerta("Erro inesperado", errorThrown, true);
            }
        });
    });
}
function showModalLiquidarDespesa() {
    limparMensagem();
    listenerCalcularTotalLiquidacaoDespesa();
    $('#btnLiquidarDespesa').attr("disabled", false);
    $("#modalLiquidarDespesaMensal").modal();
}
function liquidarDespesa() {
    var form = $("#liquidarForm");
    if (!$("form")[0].checkValidity()) {//Validação inválida;
        return;
    }
    $.ajax({
        cache: false,
        async: true,
        url: '/DespesaMensal/JsonLiquidarDespesaMensal',
        type: "POST",
        data: form.serialize(),
        success: function (result) {
            if (result.HasErro) {
                exibirMensagem(result.Erros, true);
                return;
            }
            $('#btnLiquidarDespesa').attr("disabled", true);
            exibirMensagem(result.Mensagem, false);
            listarDespesaMensal();
            setTimeout(function () { $("#modalLiquidarDespesaMensal").modal('hide'); }, 2000);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            exibirAlerta("Erro inesperado", errorThrown, true);
        }
    });
}
function listenerCalcularTotalLiquidacaoDespesa() {
    $(".valorDespesa,.valorDescontoLiquidacaoDespesa,.valorMultaJurosLiquidacaoDespesa").focusout(function () {
        var vlrDespesa = 0;
        if ($(".valorDespesa").val() != "")
            vlrDespesa = parseFloat($(".valorDespesa").val().replace('R$', '').replace(' ', '').replace('.', '').replace(',', '.')) * 100;
        var vlrDesconto = 0;
        if ($(".valorDescontoLiquidacaoDespesa").val() != '')
            vlrDesconto = parseFloat($(".valorDescontoLiquidacaoDespesa").val().replace('.', '').replace(',', '.')) * 100;
        var vlrMultaJuros = 0;
        if ($(".valorMultaJurosLiquidacaoDespesa").val() != '')
            vlrMultaJuros = parseFloat($(".valorMultaJurosLiquidacaoDespesa").val().replace('.', '').replace(',', '.')) * 100;
        var valorTotal = ((vlrDespesa + vlrMultaJuros) - vlrDesconto);
        var money = (valorTotal / 100).toFixed(2).replace(/./g, function (c, i, a) {
            return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
        });
        money = money.replace(',', '-').replace('.', ',').replace('-', '.');
        $(".valorTotalLiquidacaoDespesa").val('R$ ' + money);
    });
}
//-----------------------------------------------------------

// Views\DespesaMensal\_ListaDespesaPartial.cshtml
function listenerDetalharDespesaLiquidada() {
    $(".linkDetalharDespesaLiquidada").click(function () {
        var idDespesaMensal = $(this).data("id");
        detalharDespesaLiquidada(idDespesaMensal);
    });
}
function detalharDespesaLiquidada(idDespesa) {
    $.ajax({
        type: "GET",
        url: "DespesaMensal/JsonDetalharDespesaLiquidada",
        cache: false,
        dataType: "json",
        data: { idDespesa: idDespesa },
        success: function (result) {
            if (result.HasErro) {
                exibirAlerta("Erro ao carregar despesa mensal para confirmar remoção.", result.Erros, true);
                return;
            }
            $('#detalharDespesaLiquidada').html(result.Model);
            showModalDetalheDespesaLiquidada();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
    });
}
function showModalDetalheDespesaLiquidada() {
    limparMensagem();
    $("#modalDetalharDespesaLiquidada").modal();
    $("#modalDetalharDespesaLiquidada").on('hidden.bs.modal', function () {
        $('#detalharDespesaLiquidada').html("");
        limparMensagem();
    });
    $("#modalDetalharDespesaLiquidada").on('shown.bs.modal', function () {
        $('#btnEstornarDespesaLiquidada').attr("disabled", false);
        $("#btnEstornarDespesaLiquidada").click(function () {
            var idDespesa = $("#IdDespesaMensalLiquidada").val();
            confirmarEstornoDespesaLiquidada(idDespesa);
        });
    });
}
function confirmarEstornoDespesaLiquidada(idDespesa) {
    exibirConfirmacao("Estorno da despesa liquidada",
        "Deseja realmente estornar a despesa liquidada?",
        function () {
            $('#btnEstornarDespesaLiquidada').attr("disabled", true);
            $.ajax({
                type: "POST",
                url: "DespesaMensal/JsonEstornarDespesaLiquidada",
                cache: false,
                dataType: "json",
                data: { idDespesa: idDespesa },
                success: function (result) {
                    if (result.HasErro) {
                        exibirAlerta("Erro ao estornar despesa mensal.", result.Erros, true);
                        return;
                    }
                    exibirMensagem(result.Mensagem, false);
                    listarDespesaMensal();
                    setTimeout(function () { $("#modalDetalharDespesaLiquidada").modal('hide'); }, 2000);
                }, error: function () {
                    exibirMensagem('Não foi possível estornar a despesa liquidada.', true);
                }
            });
            $("#modalConfirmacaoEstornoDespesaLiquidada").modal('hide');
        }, null, "modalConfirmacaoEstornoDespesaLiquidada");
}
//-----------------------------------------------------------
