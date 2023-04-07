// Views\DespesaMensal\_RegistrarDespesaSimplesPartial.cshtml
$("#linkRegistrarNovaDespesa").click(function () {
    $.ajax({
        cache: false,
        type: "GET",
        url: "/DespesaMensal/JsonRegistrarNovaDespesa",
        contentType: "application/html",
        dataType: "json",
        success: function (data) {
            if (data.HasErro) {
                exibirAlerta("Erro ao registrar nova despesa simples.", data.Erros, true);
                return;
            }
            $('#novaDespesaMensal').html(data.Model);
            aplicarMascarasFormatacao();
            $("#dtpDataVencimentoDespesa").datetimepicker({
                locale: 'pt-br',
                format: 'DD/MM/YYYY'
            });
            $('#btnRegistrarNovaDespesa').click(function () {
                registrarNovaDespesaSimples();
            });
            showModalNovaDespesa();
        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            exibirAlerta("Erro inesperado", errorThrown, true);
        }
    });
});
function showModalNovaDespesa() {
    limparMensagem();
    $("#modalRegistrarNovaDespesaMensal").modal();
}
function registrarNovaDespesaSimples() {
    var form = $("#registrarNovaDespesaForm");
    if (!form[0].checkValidity()) {//Validação não realizada.
        return;
    }
    $.ajax({
        cache: false,
        url: '/DespesaMensal/JsonRegistrarNovaDespesa',
        type: "POST",
        data: form.serialize(),
        success: function (result) {
            if (result.HasErro) {
                exibirMensagem(result.Erros, true);
                return;
            }
            $('#btnRegistrarNovaDespesa').attr("disabled", true);
            exibirMensagem(result.Mensagem, false);
            listarDespesaMensal();
            setTimeout(function () { $("#modalRegistrarNovaDespesaMensal").modal('hide'); }, 2000);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            exibirAlerta("Erro inesperado", errorThrown, true);
        }
    });
}
//-----------------------------------------------------------