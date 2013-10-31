/// <reference path="Lib/jquery-1.10.2.js" />
/// <reference path="Lib/jquery-ui-1.10.3.custom.js" />
/// <reference path="Lib/jquery.extend.js" />
/// <reference path="Lib/jStorage.js" />
/// <reference path="Lib/json2.js" />
/// <reference path="Lib/knockout-2.3.0.js" />
/*global alert,$,Enumerable,console,dialog,l,document,
location,Utility,setTimeout,jQuery,require,window,arguments,navigator*/


//#region CommonEnum
var commonEnum = {
    dialog: {
        status: {
            close: 0,
            open: 1
        }
    }
}
//#endregion

//#region dialog
function dialog() { }
dialog.prototype = {
    status: commonEnum.dialog.status.close,
    height: "auto",
    width: 300,
    draggable: true,
    modal: true,
    resizable: true,
    show: function (txtTitle, txtMessage, oButtons) {
        if (arguments.length < 2) {
            throw new Error("please input title and content in dialog.");
        }

        var $div = window.top.$('<div id="mydialog"></div>'),
            roots = this,
            buttons = [],
            obtn = {};

        $div.html('<div>' + txtMessage + '</div>');
        if (this.width === null || this.width === undefined) {
            this.width = 300;
        }

        if (this.height === null || this.height === undefined) {
            this.height = "auto";
        }

        if (oButtons === null || oButtons === undefined) {
            buttons.push(new dialogButton("OK", function () {
                roots.close();
            }));
        } else {
            if (oButtons instanceof Array) {
                buttons = buttons.concat(oButtons);
            } else if (MyFunction.getConstructorName(oButtons) === "dialogButton") {
                buttons.push(oButtons);
            }
        }
        for (var intNum in buttons) {
            obtn[buttons[intNum].name] = buttons[intNum].callback;
        }
        $div.dialog({
            height: roots.height,
            width: roots.width,
            draggable: roots.draggable,
            title: txtTitle,
            modal: roots.modal,
            resizable: roots.resizable,
            buttons: obtn
        });
    },
    alert: function (txtTitle, txtMessage) {
        if (arguments.length < 2) {
            throw new Error("please input title and content in dialog.");
        }
        this.show(txtTitle, txtMessage);
    },
    confirm: function (txtTitle, txtMessage, arrButtons) {
        if (arguments.length < 2) {
            throw new Error("please input title and content in dialog.");
        }
        if (arrButtons === null || arrButtons === undefined) {
            this.show(txtTitle, txtMessage);
        } else {
            this.show(txtTitle, txtMessage, arrButtons);
        }
        
    },
    close: function () {
        window.top.$("#mydialog").dialog("close").remove();

    }
};
dialog.prototype.constructor = dialog;
//#endregion


//#region dialogButton
function dialogButton(txtName, fnCallBack) {
    if (arguments.length <= 0) {
        throw new Error("Must be assign the parameters.");
    }

    if (fnCallBack !== null && fnCallBack !== undefined) {
        if (typeof fnCallBack !== "function") {
            throw new Error("fnCallBack must be a function.");
        }
    }

    this.name = txtName;
    this.callback = fnCallBack;
}

dialogButton.prototype = {
    name: "",
    callback: null
};
dialogButton.prototype.constructor = dialogButton;
//#endregion