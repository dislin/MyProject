/// <reference path="../../Lib/jquery-1.10.2.js" />
/// <reference path="../../Lib/jquery.extend.js" />
/// <reference path="../../Lib/jStorage.js" />
/// <reference path="../../Lib/json2.js" />
/// <reference path="../../Lib/knockout-2.3.0.js" />
/*global alert,$,Enumerable,console,dialog,l,document,
location,Utility,setTimeout,jQuery,require,window,arguments,navigator*/

function demo5Function() { }
demo5Function.prototype = {
    create: function () {
        var me = {};
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

demo5Function.prototype.constructor = demo5Function;

function productModel() { }

productModel.prototype = {
    create: function () {
        var me = {}, roots = this;
        me.init = function () {
            this.initEvent();
            this.refresh();
        };

        me.initEvent = function () {
            $("a.del-btn").on("click", function () {
                var oList = roots.dataSource();
                for (var key in oList) {
                    if (oList[key].ProductId == $(this).attr("pid")) {
                        oList.splice(key, 1);
                    }
                }
                roots.dataSource(oList)
            });
        };

        me.refresh = function () {
            var that = this;
            $("#btnRefresh").on("click", function () {
                roots.dataSource(new demo5Function().create().getData());
                that.initEvent();
            });
        }
        return me.init();
    },
    dataSource: ko.observable(new demo5Function().create().getData()),
    deleteProduct: null
};

productModel.prototype.constructor = productModel;


$(function () {
    var ds = new productModel();
    ko.applyBindings(ds);
    ds.create();
});