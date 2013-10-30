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
    alert: function (txtMessage) {
        var $div = window.top.$('<div id="dialog"></div>'),
            roots = this;
        $div.html('<div>' + txtMessage + '</div>');
        if (this.width === null || this.width === undefined) {
            this.width = 300;
        }

        if (this.height === null || this.height === undefined) {
            this.height = "auto";
        }

        $div.dialog({
            height: roots.height,
            width: roots.width,
            draggable: roots.draggable,
            title: "Message",
            modal: roots.modal,
            resizable: roots.resizable
        });
    }
};
dialog.prototype.constructor = dialog;
//#endregion