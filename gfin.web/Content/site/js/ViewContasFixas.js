$(function () {
    $(".linkHistoricoProcesso").click(function () {
        obterHistoricoProcesso($(this).data("id"));
    });
    function obterHistoricoProcesso(id) {
        $.ajax({
            cache: false,
            type: "GET",
            url: "/ContasFixas/JsonHistoricoProcesso",
            dataType: "json",
            data: { idProcesso: id },
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.Erros, true);
                    return;
                }
                $('#historicoProcesso').html(result.Model);
                limparMensagem();
                $("#modalHistoricoProcesso").modal();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
});