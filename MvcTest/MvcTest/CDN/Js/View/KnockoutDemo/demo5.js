/// <reference path="../../Lib/jquery-1.10.2.js" />
/// <reference path="../../Lib/jquery.extend.js" />
/// <reference path="../../Lib/jStorage.js" />
/// <reference path="../../Lib/json2.js" />
/// <reference path="../../Lib/knockout-2.3.0.js" />
/*global alert,$,Enumerable,console,dialog,l,document,
location,Utility,setTimeout,jQuery,require,window,arguments,navigator*/

function demo5Model() { }
demo5Model.prototype = {
    instance: function () {
        var me = {};
        me.init = function () {
            this.initEvent();
        };

        me.initEvent = function () {
            $("a.del-btn").on("click", function () {
                console.log(new productMode($(this).attr("pid")));
            });
        };

        me.getData = function () {
            var dataSource = null;
            $.ajax({
                url: '../../AjaxService/GetProduct',
                type: "POST",
                cache: false,
                dataType: "json",
                data: null,
                async: false,
                success: function (data) {
                    dataSource = data;
                }
            });
            return dataSource;
        };
        return me;
    }
};
demo5Model.prototype.constructor = demo5Model;

function productModel() {}
productModel.prototype = {
    instance: function () {
        var me = {}, that = this;
        me.init = function () {
            this.initEvent();
        };

        me.initEvent = function () {
            $("a.del-btn").on("click", function () {
                that.deleteProduct($(this).attr("pid"));
            });
        };
        return me.init();
    },
    dataSource: ko.observableArray(),
    deleteProduct: function (id) {
        var self = this, oProduct = null;
        for (var key in self.dataSource) {
            if (self.dataSource[key].ProductId == id) {
                oProduct = self.dataSource[key];
            }
        }
        ko.utils.arrayRemoveItem(self.dataSource, oProduct);
        console.log(self.dataSource);
    }
};
productModel.prototype.constructor = productModel;


$(function () {
    var fn = new demo5Model().instance(),
        ds = new productModel();
    ds.dataSource = fn.getData();

    ko.applyBindings(ds);
    ds.instance();
});