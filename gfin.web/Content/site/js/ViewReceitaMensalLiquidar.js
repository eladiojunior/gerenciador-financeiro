
$(function () {
    listenerLiquidarReceita();
    listenerDetalharReceitaLiquidada();
});

// Views\ReceitaMensal\_ListaReceitaSimplesPartial.cshtml
function listenerLiquidarReceita() {
    $(".linkLiquidarReceita").click(function () {
        var idReceita = $(this).data("id");
        $.ajax({
            cache: false,
            type: "GET",
            url: "/ReceitaMensal/JsonLiquidarReceitaMensal",
            contentType: "application/html",
            dataType: "json",
            data: {
                idReceita: idReceita
            },
            success: function (data) {
                if (data.HasErro) {
                    exibirAlerta("Erro na Liquidação", data.Erros, true);
                    return;
                }
                $('#liquidarReceitaMensal').html(data.Model);
                aplicarMascarasFormatacao();
                $("#dtpDataLiquidacaoReceita").datetimepicker({
                    locale: 'pt-br',
                    format: 'DD/MM/YYYY'
                });
                $('#btnLiquidarReceita').click(function () {
                    liquidarReceita();
                });
                showModalLiquidarReceita();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                exibirAlerta("Erro inesperado", errorThrown, true);
            }
        });
    });
}
function showModalLiquidarReceita() {
    limparMensagem();
    $('#btnLiquidarReceita').attr("disabled", false);
    $("#modalLiquidarReceitaMensal").modal();
}
function liquidarReceita() {
    var form = $("#liquidarForm");
    if (!$("form")[0].checkValidity()) {//Validação inválida;
        return;
    }
    $.ajax({
        cache: false,
        async: true,
        url: '/ReceitaMensal/JsonLiquidarReceitaMensal',
        type: "POST",
        data: form.serialize(),
        success: function (result) {
            if (result.HasErro) {
                exibirMensagem(result.Erros, true);
                return;
            }
            $('#btnLiquidarReceita').attr("disabled", true);
            exibirMensagem(result.Mensagem, false);
            listarReceitaMensal();
            setTimeout(function () { $("#modalLiquidarReceitaMensal").modal('hide'); }, 2000);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            exibirAlerta("Erro inesperado", errorThrown, true);
        }
    });
}
//-----------------------------------------------------------
function listenerDetalharReceitaLiquidada() {
    $(".linkDetalharReceitaLiquidada").click(function () {
        var idReceitaMensal = $(this).data("id");
        detalharReceitaLiquidada(idReceitaMensal);
    });
}
function detalharReceitaLiquidada(idReceita) {
    $.ajax({
        type: "GET",
        url: "ReceitaMensal/JsonDetalharReceitaLiquidada",
        cache: false,
        dataType: "json",
        data: { idReceita: idReceita },
        success: function (result) {
            if (result.HasErro) {
                exibirAlerta("Erro ao carregar receita mensal para confirmar remoção.", result.Erros, true);
                return;
            }
            $('#detalharReceitaLiquidada').html(result.Model);
            showModalDetalheReceitaLiquidada();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) { alert(errorThrown); }
    });
}
function showModalDetalheReceitaLiquidada() {
    limparMensagem();
    $("#modalDetalharReceitaLiquidada").modal();
    $("#modalDetalharReceitaLiquidada").on('hidden.bs.modal', function () {
        $('#detalharReceitaLiquidada').html("");
        limparMensagem();
    });
    $("#modalDetalharReceitaLiquidada").on('shown.bs.modal', function () {
        $('#btnEstornarReceitaLiquidada').attr("disabled", false);
        $("#btnEstornarReceitaLiquidada").click(function () {
            var idReceita = $("#IdReceitaMensalLiquidada").val();
            confirmarEstornoReceitaLiquidada(idReceita);
        });
    });
}
function confirmarEstornoReceitaLiquidada(idReceita) {
    exibirConfirmacao("Estorno da receita liquidada",
        "Deseja realmente estornar a receita liquidada?",
        function () {
            $('#btnEstornarReceitaLiquidada').attr("disabled", true);
            $.ajax({
                type: "POST",
                url: "ReceitaMensal/JsonEstornarReceitaLiquidada",
                cache: false,
                dataType: "json",
                data: { idReceita: idReceita },
                success: function (result) {
                    if (result.HasErro) {
                        exibirAlerta("Erro ao estornar receita mensal.", result.Erros, true);
                        return;
                    }
                    exibirMensagem(result.Mensagem, false);
                    listarReceitaMensal();
                    setTimeout(function () { $("#modalDetalharReceitaLiquidada").modal('hide'); }, 2000);
                }, error: function () {
                    exibirMensagem('Não foi possível estornar a receita liquidada.', true);
                }
            });
            $("#modalConfirmacaoEstornoReceitaLiquidada").modal('hide');
        }, null, "modalConfirmacaoEstornoReceitaLiquidada");
}
//-----------------------------------------------------------