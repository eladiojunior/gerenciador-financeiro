$(function () {
    $(".novaNaturezaDespesa").click(function () {
        var retornoDropdown = $(this).data("retornodropdown");
        $.ajax({
            cache: false,
            type: "GET",
            url: "/NaturezaConta/JsonNovaNaturezaDespesa",
            dataType: "json",
            data: { retornoDropdown: retornoDropdown },
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.Erros, true);
                    return;
                }
                $('#novaNaturezaDespesa').html(result.Model);
                aplicarListenerModalNaturezaConta();
                $("#modalNaturezaConta").modal();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    });
    $(".novaNaturezaReceita").click(function () {
        var retornoDropdown = $(this).data("retornodropdown");
        $.ajax({
            cache: false,
            type: "GET",
            url: "/NaturezaConta/JsonNovaNaturezaReceita",
            dataType: "json",
            data: { retornoDropdown: retornoDropdown },
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.Erros, true);
                    return;
                }
                $('#novaNaturezaReceita').html(result.Model);
                aplicarListenerModalNaturezaConta();
                $("#modalNaturezaConta").modal();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    });
    function aplicarListenerModalNaturezaConta() {
        $("#modalNaturezaConta").on('show.bs.modal', function () {
            limparMensagem();
        });
        $("#modalNaturezaConta").on('shown.bs.modal', function () {
            $("#DescricaoNaturezaConta").focus();
        });
        $('FORM').on("submit", function () {
            $.ajax({
                type: 'POST',
                url: "/NaturezaConta/JsonRegistrar",
                dataType: "json",
                data: $("#formNaturezaConta").serialize(),
                success: function (result) {
                    if (result.HasErro) {
                        exibirMensagem(result.Erros, true);
                        return;
                    }
                    exibirMensagem(result.Mensagem, false);
                    var idDropdownRetorno = $("#IdDropdownRetorno").val();
                    $("#" + idDropdownRetorno).append('<option selected value="' + result.Model.Id + '">' + result.Model.DescricaoNaturezaConta + '</option>');
                    $("#DescricaoNaturezaConta").val('').focus();
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                }
            });
        });
    }
});
