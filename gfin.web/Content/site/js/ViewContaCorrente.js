$(function () {
    listarContaCorrente();
    aplicarListenerViewContaCorrente();
    function aplicarListenerViewContaCorrente() {
        $('[alt=numero]').mask('0000', { reverse: true });
        $('[alt=valor]').mask('#.##0,00', { reverse: true });
        $(".removerContaCorrente").click(function () {
            $("#IdContaCorrente").val($(this).data("id"));
            exibirConfirmacao("Remover Conta Corrente", "Confirma remoção da Conta Corrente?", removerContaCorrente, null, "modelConfirmaRemocaoContaCorrente");
        });
    }
    function removerContaCorrente() {
        limparMensagem();
        var id = $("#IdContaCorrente").val();
        $.ajax({
            type: "POST",
            url: "/ContaCorrente/JsonRemoverContaCorrente",
            cache: false,
            dataType: "json",
            data: { idContaCorrente: id },
            success: function (result) {
                if (result.HasErro) {
                    exibirAlerta("Erro ao remover conta corrente.", result.Erros, true);
                    return;
                }
                exibirMensagem(result.Mensagem, false);
                listarContaCorrente();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    function listarContaCorrente() {
        $.ajax({
            cache: false,
            type: "GET",
            url: "/ContaCorrente/JsonListarContaCorrente",
            dataType: "json",
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.Erros, true);
                    return;
                }
                $('#listarContaCorrente').html(result.Model);
                aplicarListenerViewContaCorrente();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
});