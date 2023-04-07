// Views\ReceitaMensal\_RegistrarReceitaSimplesPartial.cshtml
$("#linkRegistrarNovaReceita").click(function () {
    $.ajax({
        cache: false,
        type: "GET",
        url: "/ReceitaMensal/JsonRegistrarNovaReceita",
        contentType: "application/html",
        dataType: "json",
        success: function (data) {
            if (data.HasErro) {
                exibirAlerta("Erro ao registrar nova receita simples.", data.Erros, true);
                return;
            }
            $('#novaReceitaMensal').html(data.Model);
            aplicarMascarasFormatacao();
            $("#dtpDataRecebimentoReceita").datetimepicker({
                locale: 'pt-br',
                format: 'DD/MM/YYYY'
            });
            $('#btnRegistrarNovaReceita').click(function () {
                registrarNovaReceitaSimples();
            });
            showModalNovaReceita();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            exibirAlerta("Erro inesperado", errorThrown, true);
        }
    });
});
function showModalNovaReceita() {
    limparMensagem();
    $("#modalRegistrarNovaReceitaMensal").modal();
}
function registrarNovaReceitaSimples() {
    var form = $("#registrarNovaReceitaForm");
    if (!form[0].checkValidity()) {//Validação não realizada.
        return;
    }
    $.ajax({
        cache: false,
        url: '/ReceitaMensal/JsonRegistrarNovaReceita',
        type: "POST",
        data: form.serialize(),
        success: function (result) {
            if (result.HasErro) {
                exibirMensagem(result.Erros, true);
                return;
            }
            $('#btnRegistrarNovaReceita').attr("disabled", true);
            exibirMensagem(result.Mensagem, false);
            listarReceitaMensal();
            setTimeout(function () { $("#modalRegistrarNovaReceitaMensal").modal('hide'); }, 2000);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            exibirAlerta("Erro inesperado", errorThrown, true);
        }
    });
}
//-----------------------------------------------------------