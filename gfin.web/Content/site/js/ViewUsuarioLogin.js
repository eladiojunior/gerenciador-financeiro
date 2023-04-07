$(function () {

    // Views\Usuario\Login.cshtml
    $("#Email").focusout(function () {
        if ($(this).val() != "") {
            listarEntidades($(this).val());
        }
    });
    if ($("#Email").val() != "")
    {
        listarEntidades($("#Email").val());
    }
    $(".help-manter-logado").click(function () {
        $(this).popover('show');
    });
    function listarEntidades(login) {
        $("#IdEntidade option").remove();
        $("#IdEntidade").append('<option value="">Recuperando informações...</option>')
        $.ajax({
            type: "GET",
            url: "JsonListarEntidades",
            contentType: "application/html; charset=utf-8",
            dataType: "html",
            data: {login : login},
            success: function (data) {
                var _options = JSON.parse(data);
                if (_options.length == 0) {//Nenhuma entidade encontrata.
                    $("#IdEntidade option").remove();
                    $("#IdEntidade").append('<option value="">Nenhuma informação encontrada.</option>')
                } else {
                    $("#IdEntidade option").remove();
                    var count = 0;
                    $.each(_options, function () {
                        $("#IdEntidade").append('<option value="' + this.Value + '" ' + ((count++) == 0 ? ' selected="true"' : '') + '>' + this.Text + '</option>')
                    });
                }
            }
        });
    }
    //-----------------------------------------------------------

});
