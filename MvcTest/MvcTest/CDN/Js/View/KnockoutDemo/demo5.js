/// <reference path="../../Lib/jquery-1.10.2.js" />
/// <reference path="../../Lib/jquery.extend.js" />
/// <reference path="../../Lib/jStorage.js" />
/// <reference path="../../Lib/json2.js" />
/*global alert,$,Enumerable,console,dialog,l,document,
location,Utility,setTimeout,jQuery,require,window,arguments,navigator*/

function demo5Model() { }
demo5Model.prototype = {
    instance: function () {
        var me = {};
        me.init = function () {
            this.getData();
        };

        me.getData = function () {
            $.ajax({
                url: '../../AjaxService/GetProduct',
                type: "POST",
                cache: false,
                dataType: "json",
                data: null,
                success: function (data) {
                    console.log(data);
                }
            });
        };
        return me;
    }
};
demo5Model.prototype.constructor = demo5Model;

$(function () {
    var fn = new demo5Model().instance();
    fn.init();
});