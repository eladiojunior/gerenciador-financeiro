var filtroData = "";
var isExibirReceitas = true;
var isExibirDespesas = true;
var isExibirLiquidadas = false;
var isExibirMesesAnteriores = false;
$(function () {
    listarContasMensal(null, isExibirReceitas, isExibirDespesas, isExibirLiquidadas, isExibirMesesAnteriores);
    aplicarListenerViewContasMensal();
});
function aplicarListenerViewContasMensal() {
    $('input[name=IsExibirReceitas]').click(function () {
        isExibirReceitas = $(this).is(':checked');
        listarContasMensal();
    });
    $('input[name=IsExibirDespesas]').click(function () {
        isExibirDespesas = $(this).is(':checked');
        listarContasMensal();
    });
    $('input[name=IsExibirLiquidadas]').click(function () {
        isExibirLiquidadas = $(this).is(':checked');
        listarContasMensal();
    });
    $('input[name=IsExibirContasAbertasMesesAnteriores]').click(function () {
        isExibirMesesAnteriores = $(this).is(':checked');
        listarContasMensal();
    });
    $(".filtroContas").click(function () {
        filtroData = $(this).data("filtro");
        listarContasMensal();
    });
    $(".filtroContasSelect").change(function () {
        filtroData = $(this).val();
        listarContasMensal();
    });
    $(".linkEstornarReceitaLiquidada").click(function () {
        var idReceitaMensal = $(this).data("id");
        //Chamar método do JS: ViewReceitaMensalLiquidar.js
        confirmarEstornoReceitaLiquidada(idReceitaMensal);
    });
    //Chamar método do JS: ViewReceitaMensalLiquidar.js
    listenerLiquidarReceita();
    listenerDetalharReceitaLiquidada();
    $(".linkEstornarDespesaLiquidada").click(function () {
        var idDespesaMensal = $(this).data("id");
        //Chamar método do JS: ViewDespesaMensalLiquidar.js
        confirmarEstornoDespesaLiquidada(idDespesaMensal);
    });
    //Chamar método do JS: ViewDespesaMensalLiquidar.js
    listenerLiquidarDespesa();
    listenerDetalharDespesaLiquidada();
}
function listarContasMensal() {
    $.ajax({
        cache: false,
        type: "GET",
        url: "/ContasMensal/JsonListarContasMensal",
        dataType: "json",
        data: {
            dataFiltro: filtroData,
            hasExibirReceitas: isExibirReceitas,
            hasExibirDespesas: isExibirDespesas,
            hasExibirLiquidadas: isExibirLiquidadas,
            hasExibirAbertasMesesAnteriores: isExibirMesesAnteriores
        },
        success: function (result) {
            if (result.HasErro) {
                exibirMensagem(result.Erros, true);
                return;
            }
            $('#listaContasMensal').html(result.Model);
            aplicarListenerViewContasMensal();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });
}
// -----------------------------------------------------------
// Implementação de metodos para acionar a atualização da 
// lista de contas registradas...
// -----------------------------------------------------------
function listarReceitaMensal() {
    listarContasMensal();
}
function listarDespesaMensal() {
    listarContasMensal();
}