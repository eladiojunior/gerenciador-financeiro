//--------------------------------------------------------------
// Aplicar estrutura de mascaras nos campos.
//--------------------------------------------------------------
aplicarMascarasFormatacao();
function aplicarMascarasFormatacao() {
    $("[alt=linha-digitavel]").mask('#####.##### #####.###### #####.###### # ##############', { reverse: true });
    $("[alt=numero-cheque]").mask('########', { reverse: true });
    $('[alt=numero]').mask('####', { reverse: true });
    $('[alt=data]').mask('##/##/####', { reverse: true });
    $('[alt=valor]').mask('#.##0,00', { reverse: true });
    $('[alt=cnpj]').mask('##.###.###/####-##', { reverse: true });
}
$('[alt=celular]').on("keyup", function (event) {
    var target, phone, element;
    target = (event.currentTarget) ? event.currentTarget : event.srcElement;
    phone = target.value.replace(/\D/g, '');
    element = $(target);
    element.unsetMask();
    if (phone.length > 10) {
        element.setMask("(99) 99999-9999");
    } else {
        element.setMask("(99) 9999-99999");
    }
});
//--------------------------------------------------------------

//--------------------------------------------------------------
// Controlador de mensagens exibidas em Modal.    
//--------------------------------------------------------------
function limparMensagem() {
    $(".mensagens").hide();
}
function exibirMensagem(msg, hasErro) {
    var alert = $(".mensagens .alert");
    if (alert != null) {
        var msgAlert = msg;
        if (hasArray(msg)) {
            msgAlert = '';
            for (var i = 0; i < msg.length; i++) {
                if (msgAlert != '') msgAlert += '<br/>';
                msgAlert += msg[i];
            }
        }
        alert.html(msgAlert);
        alert.removeClass("alert-danger").removeClass("alert-info")
        alert.addClass(hasErro ? "alert-danger" : "alert-info");
        $(".mensagens").show();
    }
}
//--------------------------------------------------------------

//--------------------------------------------------------------
//Verificar se o objeto informado (obj) é um Array.
//--------------------------------------------------------------
function hasArray(obj) {
    return (Object.prototype.toString.call(obj) === '[object Array]');
}
//--------------------------------------------------------------

//--------------------------------------------------------------
//Gerador de ID's utilizando o Random
//--------------------------------------------------------------
function obterIdRandom() {
    return Math.round(new Date().getTime() + (Math.random() * 100));
}
//--------------------------------------------------------------

//--------------------------------------------------------------
// Criar modal de mensagem de alerta da aplicação
//--------------------------------------------------------------
function exibirAlerta(titulo, mensagem, hasErro, callbackOk, idModal) {
    if (hasErro == null) hasErro = false;
    if (idModal == null || idModal.trim() == '') { idModal = "ModalAlerta"; }
    var objModelAlerta = $("#" + idModal).attr("id");
    if (objModelAlerta) {//Existe objeto Model de alerta... reutilizar.
        $("#titulo" + idModal).val(titulo);
        $("#mensagem" + idModal).val(mensagem);
    }
    else {//Criar um objeto novo, no BODY!
        var htmlModel = "";
        if (titulo == null || titulo.trim() == '') { titulo = "Alerta"; }
        if (mensagem == null) { mensagem = "Alerta para o usuário."; }
        if (hasArray(mensagem)) {
            var msg = '';
            for (var i = 0; i < mensagem.length; i++) {
                if (msg != '') msg += '<br/>';
                msg += mensagem[i];
            }
            mensagem = msg;
        }
        htmlModel += "<div id=\"" + idModal + "\" class=\"modal fade\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"" + idModal + "\">";
        htmlModel += "  <div class=\"modal-dialog\" role=\"document\">";
        htmlModel += "    <div class=\"modal-content\">";
        htmlModel += "      <div class=\"modal-header\">";
        htmlModel += "        <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Fechar\"><span aria-hidden=\"true\">&times;</span></button>";
        htmlModel += "        <h4 class=\"modal-title\" id=\"titulo" + idModal + "\">" + titulo + "</h4>";
        htmlModel += "      </div>";
        htmlModel += "      <div class=\"modal-body\">";
        htmlModel += "        <div class=\"row\">";
        htmlModel += "          <div class=\"col-xs-2 col-md-2 text-center\">";
        htmlModel += "            <img src=\"/Content/site/imgs/" + (hasErro ? "iconeErro.png" : "iconeAlerta.png") + "\" height=\"50px\" width=\"50px\" />";
        htmlModel += "          </div>";
        htmlModel += "          <div class=\"col-xs-10 col-md-10\">";
        htmlModel += "            <p id=\"mensagem" + idModal + "\">" + mensagem + "</p>";
        htmlModel += "          </div>";
        htmlModel += "        </div>";
        htmlModel += "      </div>";
        htmlModel += "      <div class=\"modal-footer\">";
        htmlModel += "        <button type=\"button\" class=\"btn btn-primary\" id=\"btnOk" + idModal + "\" data-dismiss=\"modal\">OK</button>";
        htmlModel += "      </div>";
        htmlModel += "    </div>";
        htmlModel += "  </div>";
        htmlModel += "</div>";
        $("body").append(htmlModel);
        if ($.isFunction(callbackOk))
            $("#btnOk" + idModal).click(callbackOk);
    }
    $("#" + idModal).modal();
}

//--------------------------------------------------------------
// Criar modal de confirmação (padrão) da aplicação
//--------------------------------------------------------------
function exibirConfirmacao(titulo, mensagem, callbackSim, callbackNao, idModal) {
    if (idModal == null || idModal.trim() == '') { idModal = "ModalConfirmacao"; }
    var objModelConfirmacao = $("#" + idModal).attr("id");
    if (objModelConfirmacao) {//Existe objeto Model de confirmação... reutilizar.
        $("#titulo" + idModal).val(titulo);
        $("#mensagem" + idModal).val(mensagem);
    }
    else {//Criar um objeto novo, no BODY!
        var htmlModel = "";
        if (titulo == null || titulo.trim() == '') { titulo = "Confirmação"; }
        if (mensagem == null || mensagem.trim() == '') { mensagem = "Por favor, confirme a operação?"; }
        htmlModel += "<div id=\"" + idModal + "\" class=\"modal fade\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"" + idModal + "\">";
        htmlModel += "  <div class=\"modal-dialog\" role=\"document\">";
        htmlModel += "    <div class=\"modal-content\">";
        htmlModel += "      <div class=\"modal-header\">";
        htmlModel += "        <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Fechar\"><span aria-hidden=\"true\">&times;</span></button>";
        htmlModel += "        <h4 class=\"modal-title\" id=\"titulo" + idModal + "\">" + titulo + "</h4>";
        htmlModel += "      </div>";
        htmlModel += "      <div class=\"modal-body\">";
        htmlModel += "        <p id=\"mensagem" + idModal + "\">" + mensagem + "</p>";
        htmlModel += "      </div>";
        htmlModel += "      <div class=\"modal-footer\">";
        htmlModel += "        <button type=\"button\" class=\"btn btn-primary\" id=\"btnSim" + idModal + "\" data-dismiss=\"modal\">Sim</button>";
        htmlModel += "        <button type=\"button\" class=\"btn btn-default\" id=\"btnNao" + idModal + "\" data-dismiss=\"modal\">Não</button>";
        htmlModel += "      </div>";
        htmlModel += "    </div>";
        htmlModel += "  </div>";
        htmlModel += "</div>";
        $("body").append(htmlModel);
        if ($.isFunction(callbackSim))
            $("#btnSim" + idModal).click(callbackSim);
        if ($.isFunction(callbackNao))
            $("#btnNao" + idModal).click(callbackNao);
    }
    $("#" + idModal).modal();
}
//--------------------------------------------------------------

//--------------------------------------------------------------
// Verifica qual é o menu que está ativo.
//--------------------------------------------------------------
var actionUrl = window.location.toString().toLowerCase();
$(".sidebar-menu li a").each(function (index) {
    var urlmenu = $(this).attr("href").toString().toLowerCase();
    if (urlmenu != null && urlmenu != "#" &&
        actionUrl.indexOf(urlmenu.toLowerCase()) >= 0) {
        var li = $(this).parent();
        var ul = li.parent();
        li.addClass("active");
        if (ul.hasClass('treeview-menu')) {
            ul.parent().addClass("active");
        }
    }
});

// Retorna DIV de Aguardando...
function obterDivAguarde(msg) {
    if (msg == null || msg == '') msg = 'Agurade...';
    var htmlDiv = "";
    htmlDiv += "<div id=\"fountainG\">";
    htmlDiv += "<div id=\"fountainG_1\" class=\"fountainG\"></div>";
    htmlDiv += "<div id=\"fountainG_2\" class=\"fountainG\"></div>";
    htmlDiv += "<div id=\"fountainG_3\" class=\"fountainG\"></div>";
    htmlDiv += "<div id=\"fountainG_4\" class=\"fountainG\"></div>";
    htmlDiv += "<div id=\"fountainG_5\" class=\"fountainG\"></div>";
    htmlDiv += "<div id=\"fountainG_6\" class=\"fountainG\"></div>";
    htmlDiv += "<div id=\"fountainG_7\" class=\"fountainG\"></div>";
    htmlDiv += "<div id=\"fountainG_8\" class=\"fountainG\"></div>";
    htmlDiv += "<p>" + msg + "</p>";
    htmlDiv += "</div>";
    return htmlDiv;
}

//****************************************************************************************
//** Responsável pela alteração da foto do usuário.
//----------------------------------------------------------------------------------------
function CropAvatar() {
    this.$avatar = $('#fotoUsuario');
    this.$avatarModal = $('#modalAlterarFotoUsuario');
    this.$avatarForm = this.$avatarModal.find('.avatar-form');
    this.$avatarUpload = this.$avatarForm.find('.avatar-upload');
    this.$avatarSrc = this.$avatarForm.find('.avatar-src');
    this.$avatarId = this.$avatarForm.find('.avatar-id');
    this.$avatarData = this.$avatarForm.find('.avatar-data');
    this.$avatarInput = this.$avatarForm.find('.avatar-input');
    this.$avatarSave = this.$avatarForm.find('.avatar-save');
    this.$avatarWrapper = this.$avatarModal.find('.avatar-wrapper');
    this.$avatarWrapperEdit = this.$avatarModal.find('.avatar-wrapper-edit');
    this.$avatarMensagens = this.$avatarModal.find('.avatar-msg');
    this.$avatarPreview = this.$avatarModal.find('.avatar-preview');
    this.init();
}
CropAvatar.prototype = {
    constructor: CropAvatar,
    support: {
        fileList: !!$('<input type="file">').prop('files'),
        blobURLs: !!window.URL && URL.createObjectURL,
        formData: !!window.FormData
    },
    init: function () {
        this.support.datauri = this.support.fileList && this.support.blobURLs;
        if (!this.support.formData) {
            this.initIframe();
        }
        this.addListener();
    },
    addListener: function () {
        this.$avatarInput.on('change', $.proxy(this.change, this));
        this.$avatarForm.on('submit', $.proxy(this.submit, this));
        this.$avatarModal.on('show.bs.modal', $.proxy(this.showModal, this));
    },
    showModal: function () {
        var url = this.$avatar.attr('src');
        this.$avatarPreview.html('<img src="' + url + '">');
        this.$avatarMensagens.empty();
    },
    initIframe: function () {
        var target = 'upload-iframe-' + (new Date()).getTime();
        var $iframe = $('<iframe>').attr({
            name: target,
            src: ''
        });
        var _this = this;
        // Ready ifrmae
        $iframe.one('load', function () {
            // respond response
            $iframe.on('load', function () {
                var data;
                try {
                    data = $(this).contents().find('body').text();
                } catch (e) {
                    console.log(e.message);
                }
                if (data) {
                    try {
                        data = $.parseJSON(data);
                    } catch (e) {
                        console.log(e.message);
                    }
                    _this.submitDone(data);
                } else {
                    _this.submitFail('Image upload failed!');
                }
                _this.submitEnd();
            });
        });
        this.$iframe = $iframe;
        this.$avatarForm.attr('target', target).after($iframe.hide());
    },
    change: function () {
        var file = this.getFileUpload(this.$avatarInput);
        if (this.isValidImageFile(file)) {
            if (this.support.datauri) {
                if (this.url) {
                    URL.revokeObjectURL(this.url); // Revoke the old one
                }
                this.url = URL.createObjectURL(file);
                this.startCropper();
            }
            else {
                this.syncUpload();
            }
        }
    },
    getFileUpload: function (inputFile) {
        var file;
        if (this.support.datauri) {
            var files = inputFile.prop('files');
            if (files.length > 0) {
                file = files[0];
            }
        } else {
            file = inputFile.val();
        }
        return file;
    },
    submit: function () {
        if (!this.$avatarSrc.val() && !this.$avatarInput.val()) {
            exibirAlerta("Alerta", "Favor carregar uma imagem no formato (jpg, png ou gif) com no máximo de 2MB (MegaBytes).", false);
            return false;
        }
        var file = this.getFileUpload(this.$avatarInput);
        if (!this.isValidImageFile(file)) {
            exibirAlerta("Alerta", "Favor carregar uma imagem no formato (jpg, png ou gif) com no máximo de 2MB (MegaBytes).", false);
            return false;
        }
        if (this.support.formData) {
            this.ajaxUpload();
            return false;
        }
    },
    isValidImageFile: function (file) {
        var isValid = false;
        //Valida o tipo da imagem.
        if (file.type) {
            isValid = /^image\/\w+$/.test(file.type);
        } else {
            isValid = /\.(jpg|jpeg|png|gif)$/.test(file);
        }
        if (isValid) {
            //Recuperar o tamanho máximo da imagem.
            var maxSizeFile = this.$avatarInput.attr('data-max-size');
            //Validar o tamanho da imagem.
            var maxSize = parseInt(maxSizeFile, 10);
            //alert(file.length + ' - ' + file.size + ' - ' + file.fileSize)
            var size = file.size;
            isValid = (maxSize > size);
        }
        if (!isValid) {
            exibirAlerta("Alerta", "Favor carregar uma imagem no formato (jpg, png ou gif) com no máximo de 2MB (MegaBytes).", false);
        }
        return isValid;
    },
    startCropper: function () {
        var _this = this;
        if (this.active) {
            this.$img.cropper('replace', this.url);
        } else {
            this.$img = $('<img src="' + this.url + '">');
            this.$avatarWrapper.hide();
            this.$avatarWrapperEdit.empty().html(this.$img);
            this.$avatarWrapperEdit.show();
            this.$img.cropper({
                aspectRatio: 1,
                preview: this.$avatarPreview.selector,
                crop: function (e) {
                    var json = [
                          '{"x":' + e.x,
                          '"y":' + e.y,
                          '"height":' + e.height,
                          '"width":' + e.width,
                          '"rotate":' + e.rotate + '}'
                    ].join();

                    _this.$avatarData.val(json);
                }
            });
            this.active = true;
        }
        this.$avatarModal.one('hidden.bs.modal', function () {
            _this.$avatarPreview.empty();
            _this.stopCropper();
        });
    },
    stopCropper: function () {
        if (this.active) {
            this.$img.cropper('destroy');
            this.$img.remove();
            this.active = false;
            this.$avatarWrapperEdit.hide();
            this.$avatarWrapper.show();
        }
    },
    ajaxUpload: function () {
        var url = this.$avatarForm.attr('action');
        var data = new FormData(this.$avatarForm[0]);
        var _this = this;
        $.ajax(url, {
            type: 'post',
            data: data,
            dataType: 'json',
            processData: false,
            contentType: false,
            success: function (data) {
                _this.submitDone(data);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                _this.submitFail(textStatus || errorThrown);
            }
        });
    },
    syncUpload: function () {
        this.$avatarSave.click();
    },
    submitDone: function (data) {
        if (data.HasErro) {
            exibirAlerta("Erro", data.Erros, true);
            return;
        }
        this.url = data.Model;
        if (this.support.datauri || this.uploaded) {
            this.uploaded = false;
            this.cropDone();
        } else {
            this.uploaded = true;
            this.$avatarSrc.val(this.url);
            this.startCropper();
        }
        this.$avatarInput.val('');
    },
    submitFail: function (msg) {
        exibirAlerta("Erro inesperado", msg, true);
    },
    cropDone: function () {
        this.$avatarForm.get(0).reset();
        this.$avatar.attr('src', this.url);
        this.stopCropper();
        this.$avatarModal.modal('hide');
    },
    alert: function (msg) {
        var $alert = [
              '<div class="alert alert-danger avatar-alert alert-dismissable">',
                '<button type="button" class="close" data-dismiss="alert">&times;</button>',
                msg,
              '</div>'
        ].join('');
        this.$avatarMensagens.empty().html($alert);
    }
};
$(function () {
    return new CropAvatar();
});
//----------------------------------------------------------------------------------------