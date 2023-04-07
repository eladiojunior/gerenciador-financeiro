$(function () {
    listarCartaoCredito();
    aplicarListenerViewCartaoCredito();
    function aplicarListenerViewCartaoCredito() {
        $('[alt=mesAno]').mask('00/0000', { reverse: true });
        $('[alt=numero]').mask('00', { reverse: true });
        $('[alt=numeroCartao]').mask('0000 0000 0000 0000', { reverse: true });
        $('[alt=valor]').mask('#.##0,00', { reverse: true });
        $('input[name=HasCartaoPrePago]').click(function () {
            if ($(this).is(':checked')) {
                $('input[name=HasCartaoCredito]').attr("checked", false);
                $('input[name=HasCartaoDebito]').attr("checked", false);
            }
        });
        $('input[name=HasCartaoCredito],input[name=HasCartaoDebito]').click(function () {
            if ($(this).is(':checked')) {
                $('input[name=HasCartaoPrePago]').attr("checked", false);
            }
        });
        $(".removerCartaoCredito").click(function () {
            $("#IdCartaoCredito").val($(this).data("id"));
            exibirConfirmacao("Remover Cartão de Crédito", "Confirma remoção do Cartão de Crédito?", removerCartaoCredito, null, "modelConfirmaRemocaoCartaoCredito");
        });
    }
    function removerCartaoCredito() {
        limparMensagem();
        var id = $("#IdCartaoCredito").val();
        $.ajax({
            type: "POST",
            url: "/CartaoCredito/JsonRemoverCartaoCredito",
            cache: false,
            dataType: "json",
            data: { idCartaoCredito: id },
            success: function (result) {
                if (result.HasErro) {
                    exibirAlerta("Erro ao remover cartão de crédito.", result.Erros, true);
                    return;
                }
                exibirMensagem(result.Mensagem, false);
                listarCartaoCredito();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    function listarCartaoCredito() {
        $.ajax({
            cache: false,
            type: "GET",
            url: "/CartaoCredito/JsonListarCartaoCredito",
            dataType: "json",
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.Erros, true);
                    return;
                }
                $('#listarCartaoCredito').html(result.Model);
                aplicarListenerViewCartaoCredito();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
});