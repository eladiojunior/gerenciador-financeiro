$(function () {
    listarDespesaMensal();
    listarReceitaMensal();
    carregarGraficoContasAnual();
});

// Views\Home\Index.cshtml
function carregarGraficoContasAnual() {
    $('#divGrafico').html(obterDivAguarde("Carregando o gráfico de contas..."));
    $.ajax({
        cache: false,
        type: "GET",
        url: "/Home/JsonGraficoContasAnual",
        dataType: "json",
        success: function (result) {
            if (result.HasErro) {
                exibirMensagem(result.Erros, true);
                return;
            }
            var ctx = document.createElement('canvas');
            //var ctx = document.getElementById("chartContasAnuais");
            if (ctx == null) return;
            ctx.getContext('2d');
            $('#divGrafico').html("").append(ctx);
            var myChart = new Chart(ctx, result.Model);
            myChart.resize(300, 250);
        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });
}
function listarReceitaMensal() {
    $('#listaReceitas').html(obterDivAguarde("Carregando as receitas..."));
    $.ajax({
        cache: false,
        type: "GET",
        url: "/ReceitaMensal/JsonListarReceitaMensalSimples",
        contentType: "application/html",
        dataType: "json",
        success: function (result) {
            if (result.HasErro) {
                $('#listaReceitas').html("<div class=\"alert alert-danger\" role=\"alert\">" + result.Erros + "</div>");
                return;
            }
            $('#listaReceitas').html(result.Model);
            listenerLiquidarReceita();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            exibirAlerta("Erro inesperado", errorThrown, true);
        }
    });
}
function listarDespesaMensal() {
    $('#listaDespesas').html(obterDivAguarde("Carregando as despesas..."));
    $.ajax({
        cache: false,
        type: "GET",
        url: "/DespesaMensal/JsonListarDespesaMensalSimples",
        contentType: "application/html",
        dataType: "json",
        success: function (result) {
            if (result.HasErro) {
                $('#listaDespesas').html("<div class=\"alert alert-danger\" role=\"alert\">" + result.Erros + "</div>");
                return;
            }
            $('#listaDespesas').html(result.Model);
            listenerLiquidarDespesa();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            exibirAlerta("Erro inesperado", errorThrown, true);
        }
    });
}
//-----------------------------------------------------------