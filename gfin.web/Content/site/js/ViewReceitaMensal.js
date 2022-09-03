$(function () {
    listarReceitaMensal();
    aplicarListenerViewReceitaMensal();
    $("#dtpDataRecebimentoReceita").datetimepicker({
        locale: 'pt-br',
        format: 'DD/MM/YYYY'
    });
});

function verificarReceitaRegistradaJaRecebida() {
    var dataSistema = new Date();
    var dataRecebimento = new Date($('#dtpDataRecebimentoReceita').data("DateTimePicker").date());
    if (dataSistema >= dataRecebimento) {
        var hasConfirma = window.confirm("Deseja registrar essa receita como recebida?");
        $("#IsReceitaRecebida").val(hasConfirma);
    }
}
function aplicarListenerViewReceitaMensal() {
    $(".removerReceitaMensal").click(function () {
        $("#IdReceitaMensal").val($(this).data("idreceitamensal"));
        exibirConfirmacao("Remover Receita Mensal", "Confirma a remoção da receita mensal selecionada?", removerReceitaMensal, null, "modelConfirmaRemocaoReceitaMensal");
    });
    $(".filtroReceitas").click(function () {
        var filtro = $(this).data("filtro");
        listarReceitaMensal(filtro);
    });
    $(".filtroReceitasSelect").change(function () {
        var filtro = $(this).val();
        listarReceitaMensal(filtro);
    });
    $(".linkEstornarReceitaLiquidada").click(function () {
        var idReceitaMensal = $(this).data("id");
        //Chamar método do JS: ViewReceitaMensalLiquidar.js
        confirmarEstornoReceitaLiquidada(idReceitaMensal);
    });
    //Chamar método do JS: ViewReceitaMensalLiquidar.js
    listenerLiquidarReceita();
    listenerDetalharReceitaLiquidada();
}
function removerReceitaMensal() {
    limparMensagem();
    var id = $("#IdReceitaMensal").val();
    $.ajax({
        type: "POST",
        url: "/ReceitaMensal/JsonRemoverReceitaMensal",
        cache: false,
        dataType: "json",
        data: { idReceitaMensal: id },
        success: function (result) {
            if (result.HasErro) {
                exibirAlerta("Erro ao remover a receita mensal.", result.Erros, true);
                return;
            }
            exibirMensagem(result.Mensagem, false);
            listarReceitaMensal();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });
}
function listarReceitaMensal(dataFiltro) {
    if (dataFiltro == null) dataFiltro = "";
    $.ajax({
        cache: false,
        type: "GET",
        url: "/ReceitaMensal/JsonListarReceitaMensal",
        dataType: "json",
        data: { dataFiltro: dataFiltro },
        success: function (result) {
            if (result.HasErro) {
                exibirMensagem(result.Erros, true);
                return;
            }
            $('#listaReceitaMensal').html(result.Model);
            aplicarListenerViewReceitaMensal();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });
}