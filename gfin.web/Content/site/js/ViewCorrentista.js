$(function () {
    listarCorrentista();
    aplicarListenerViewCorrentista();
    function aplicarListenerViewCorrentista() {
        $(".editableSelect").change(function () {
            $(".textBoxSelect").val($(".editableSelect option:selected").html());
            $(".textBoxSelect").focus();
        });
        $(".removerCorrentista").click(function () {
            $("#IdCorrentista").val($(this).data("id"));
            exibirConfirmacao("Remover Correntista", "Confirma remoção do Correntista da agenda?", removerCorrentista, null, "modelConfirmaRemocaoCorrentista");
        });
    }
    function removerCorrentista() {
        limparMensagem();
        var id = $("#IdCorrentista").val();
        $.ajax({
            type: "POST",
            url: "/Correntista/JsonRemoverCorrentista",
            cache: false,
            dataType: "json",
            data: { idCorrentista: id },
            success: function (result) {
                if (result.HasErro) {
                    exibirAlerta("Erro ao remover a correntista.", result.Erros, true);
                    return;
                }
                exibirMensagem(result.Mensagem, false);
                listarCorrentista();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    function listarCorrentista() {
        $.ajax({
            cache: false,
            type: "GET",
            url: "/Correntista/JsonListarCorrentista",
            dataType: "json",
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.Erros, true);
                    return;
                }
                $('#listarCorrentista').html(result.Model);
                aplicarListenerViewCorrentista();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
});