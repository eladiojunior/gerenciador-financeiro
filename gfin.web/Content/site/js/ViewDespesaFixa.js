$(function () {
    $('[alt=valor]').mask('#.##0,00', { reverse: true });
    listarDespesaFixa();
    aplicarListenerViewDespesaFixa();
    function aplicarListenerViewDespesaFixa() {
        $(".removerDepesaFixa").click(function () {
            $("#IdDespesaFixa").val($(this).data("iddespesafixa"));
            exibirConfirmacao("Remover Despesa Fixa", "Confirma a remoção da despesa fixa selecionada?", removerDespesaFixa, null, "modelConfirmaRemocaoDespesaFixa");
        });
        $(".historicoDespesaFixa").click(function () {
            listarHistoricoDespesaFixa($(this).data("iddespesafixa"));
        });
    }
    function removerDespesaFixa() {
        limparMensagem();
        var id = $("#IdDespesaFixa").val();
        $.ajax({
            type: "POST",
            url: "/DespesaFixa/JsonRemoverDespesaFixa",
            cache: false,
            dataType: "json",
            data: { idDespesaFixa: id },
            success: function (result) {
                if (result.HasErro) {
                    exibirAlerta("Erro ao remover a despesa fixa", result.Erros, true);
                    return;
                }
                exibirMensagem(result.Mensagem, false);
                listarDespesaFixa();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    function listarHistoricoDespesaFixa(id) {
        $.ajax({
            cache: false,
            type: "POST",
            url: "/DespesaFixa/JsonListarHistoricoDespesaFixa",
            dataType: "json",
            data: { idDespesaFixa: id },
            success: function (result) {
                $('#historicoDespesaFixa').html(result.Model);
                montarGraficoHistoricoDespesaFixa(id);
                limparMensagem();
                $("#modalHistoricoDespesaFixa").modal();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    function listarDespesaFixa() {
        $.ajax({
            cache: false,
            type: "GET",
            url: "/DespesaFixa/JsonListarDespesaFixa",
            dataType: "json",
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.Erros, true);
                    return;
                }
                $('#listaDespesaFixa').html(result.Model);
                aplicarListenerViewDespesaFixa();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    function montarGraficoHistoricoDespesaFixa(idDespesaFixa) {
        var ctx = document.getElementById("chartHistoricoDespesas");
        if (ctx == null) return;
        ctx.getContext('2d');
        $.ajax({
            cache: false,
            type: "GET",
            url: "/DespesaFixa/JsonGraficoHistoricoDespesaFixa",
            dataType: "json",
            data: { idDespesaFixa: idDespesaFixa },
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.Erros, true);
                    return;
                }
                var myChart = new Chart(ctx, result.Model);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
});
