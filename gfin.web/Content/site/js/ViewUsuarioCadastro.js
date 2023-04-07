$(document).ready(function () {

    // Views\Usuario\Cadastrar.cshtml
    var objCodigoTipoEntidadeControle = $('input[type=radio][name=CodigoTipoEntidadeControle]:checked');
    showHideDivEmpresa(objCodigoTipoEntidadeControle.val() == '2');
    $('input[type=radio][name=CodigoTipoEntidadeControle]').change(function () {
        showHideDivEmpresa((this.value == '2'));
    });
    function showHideDivEmpresa(isShow) {
        $("#divEmpresa").hide();
        $("#NomeEntidadeControle").removeAttr('required');
        $("#CnpjEntidadeControle").removeAttr('required');
        if (isShow) {
            $("#divEmpresa").show();
            $("#NomeEntidadeControle").prop('required', true);
            $("#CnpjEntidadeControle").prop('required', true);
            $("#NomeEntidadeControle").focus();
        }
    }
    //-----------------------------------------------------------

});
