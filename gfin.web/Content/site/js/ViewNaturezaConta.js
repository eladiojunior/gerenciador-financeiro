$(function () {
    listarNaturezaConta();
    aplicarListenerViewNaturezaConta();
    function aplicarListenerViewNaturezaConta() {
        $(".removerNaturezaConta").click(function () {
            $("#IdNaturezaConta").val($(this).data("id"));
            exibirConfirmacao("Remover Natureza da Conta", "Confirma remoção do natureza da conta?", removerNaturezaConta, null, "modelConfirmaRemocaoNaturezaConta");
        });
    }
    function removerNaturezaConta() {
        limparMensagem();
        var id = $("#IdNaturezaConta").val();
        $.ajax({
            type: "POST",
            url: "/NaturezaConta/JsonRemoverNaturezaConta",
            cache: false,
            dataType: "json",
            data: { idNaturezaConta: id },
            success: function (result) {
                if (result.HasErro) {
                    exibirAlerta("Erro ao remover a natureza da conta.", result.Erros, true);
                    return;
                }
                exibirMensagem(result.Mensagem, false);
                listarNaturezaConta();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    function listarNaturezaConta() {
        $.ajax({
            cache: false,
            type: "GET",
            url: "/NaturezaConta/JsonListarNaturezaConta",
            dataType: "json",
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.Erros, true);
                    return;
                }
                $('#listarNaturezaConta').html(result.Model);
                aplicarListenerViewNaturezaConta();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
});