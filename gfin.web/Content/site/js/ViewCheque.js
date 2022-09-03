$(function () {
    $('[alt=numero-cheque]').mask('00000000', { reverse: true });
    $('FORM').on("submit", function () {
        confirmarRegistroCheques();
        return false;
    });
    $("#IdBancoAgenciaContaCorrente").change(function () {
        listarCheque($(this).val(), 0);
    });
    listarCheque($("#IdBancoAgenciaContaCorrente").val(), 0);
    aplicarListenerViewCheque();
    function aplicarListenerViewCheque() {
        $(".cancelarCheque").click(function () {
            $("#IdCheque").val($(this).data("id"));
            exibirConfirmacao("Cancelar Cheque", "Confirma cancelamento do cheque?", cancelarCheque, null, "modelConfirmaCancelarCheque");
        });
        $("#CodigoSituacaoCheque").change(function () {
            var filtroCodigoSituacao = $(this).val();
            var filtroIdBancoAgenciaContaCorrente = $("#IdBancoAgenciaContaCorrente").val();
            if (!filtroIdBancoAgenciaContaCorrente) filtroIdBancoAgenciaContaCorrente = 0;
            listarCheque(filtroIdBancoAgenciaContaCorrente, filtroCodigoSituacao);
        });
        $(".historicoCheque").click(function () {
            obterHistoricoCheque($(this).data("id"));
        });
    }
    function aplicarListenerModalRegistrarCheques() {
        $("#modalChequeRegistro").on('show.bs.modal', function () {
            limparMensagem();
        });
        $(".removerChequeRegistro").click(function () {
            var qtdCheques = parseInt($("#QtdChequeRegistro").val());
            var numeroChequeLabel = parseInt($(this).data("numero-cheque"));
            var inputNumeroCheque = $('.numeroCheque[data-numero-cheque="' + numeroChequeLabel + '"]');
            inputNumeroCheque.val('0');
            qtdCheques = qtdCheques - 1;
            $("#QtdChequeRegistro").val(qtdCheques)
            $("#btnRegistrarCheques").prop('disabled', (qtdCheques == 0));
            $(this).parent().parent().hide();
        });
    }
    function aplicarListenerModalHistoricoCheque() {
        $("#modalHistoricoCheque").on('show.bs.modal', function () {
            limparMensagem();
        });
    }
    function confirmarRegistroCheques() {
        $.ajax({
            type: "POST",
            url: "/Cheque/JsonRegistrarListarCheques",
            cache: false,
            dataType: "json",
            data: $("#formChequeMensal").serialize(),
            success: function (result) {
                if (result.HasErro) {
                    exibirAlerta("Erro ao registrar lista de cheques.", result.Erros, true);
                    return;
                }
                $('#chequeRegistros').html(result.Model);
                aplicarListenerModalRegistrarCheques();
                $("#modalChequeRegistro").modal();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    function cancelarCheque() {
        limparMensagem();
        var id = $("#IdCheque").val();
        $.ajax({
            type: "POST",
            url: "/Cheque/JsonCancelarCheque",
            cache: false,
            dataType: "json",
            data: { idCheque: id },
            success: function (result) {
                if (result.HasErro) {
                    exibirAlerta("Erro ao cancelar cheque.", result.Erros, true);
                    return;
                }
                exibirMensagem(result.Mensagem, false);
                listarCheque($("#IdBancoAgenciaContaCorrente").val(), 0);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    function listarCheque(idBancoAgenciaContaCorrente, codigoSituacaoCheque) {
        if (idBancoAgenciaContaCorrente == "")
            idBancoAgenciaContaCorrente = "0";
        $.ajax({
            cache: false,
            type: "GET",
            url: "/Cheque/JsonListarCheque",
            dataType: "json",
            data: {
                idBancoAgenciaContaCorrente: idBancoAgenciaContaCorrente,
                codigoSituacaoCheque: codigoSituacaoCheque
            },
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.Erros, true);
                    return;
                }
                $('#listarCheque').html(result.Model);
                aplicarListenerViewCheque();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    function obterHistoricoCheque(id) {
        $.ajax({
            cache: false,
            type: "GET",
            url: "/Cheque/JsonHistoricoCheque",
            dataType: "json",
            data: { idCheque: id },
            success: function (result) {
                if (result.HasErro) {
                    exibirMensagem(result.Erros, true);
                    return;
                }
                $('#historicoCheque').html(result.Model);
                aplicarListenerModalHistoricoCheque();
                $("#modalHistoricoCheque").modal();
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
});