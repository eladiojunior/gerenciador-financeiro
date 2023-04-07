$(function () {
    $('[alt=valor]').mask('#.##0,00', { reverse: true });
    listarReceitaFixa();
    aplicarListenerViewReceitaFixa();
    function aplicarListenerViewReceitaFixa() {
        $(".removerReceitaFixa").click(function () {
            $("#IdReceitaFixa").val($(this).data("idreceitafixa"));
            exibirConfirmacao("Remover Receita Fixa", "Confirma a remoção da receita fixa selecionada?", removerReceitaFixa, null, "modelConfirmaRemocaoReceitaFixa");
        });
    }
    function removerReceitaFixa() {
        limparMensagem();
        var id = $("#IdReceitaFixa").val();
        $.ajax({
            type: "POST",
            url: "/ReceitaFixa/JsonRemoverReceitaFixa",
            cache: false,
            dataType: "json",
            data: { idReceitaFixa: id },
            success: function (result) {
                if (result.HasErro) {
                    exibirAlerta("Erro ao remover a receita fixa.", result.Erros, true);
                    return;
                }
                exibirMensagem(result.Mensagem, false);
                listarReceitaFixa();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    function listarReceitaFixa() {
        $.ajax({
            cache: false,
            type: "GET",
            url: "/ReceitaFixa/JsonListarReceitaFixa",
            dataType: "json",
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.Erros, true);
                    return;
                }
                $('#listaReceitaFixa').html(result.Model);
                aplicarListenerViewReceitaFixa();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
});
