/// <reference path="/CDN/Js/Lib/jquery-1.10.2" />
/// <reference path="/CDN/Js/Lib/jquery.extend.js" />
/// <reference path="/CDN/Js/Lib/angular-1.2.4/angular.js" />

/*global alert,Enumerable,gv,console,dialog,l,document,location,Utility,setTimeout,jQuery*/

//#region Payment
function Payment(name, charge, duedate, ispay) {
    this.name = name;
    this.charge = charge;
    this.dueDate = duedate;

    if (ispay === true || ispay === false) {
        this.isPay = ispay;
    }
}

Payment.prototype = {
    name: "",
    charge: 0,
    isPay: false,
    dueDate: null
};
Payment.prototype.constructor = Payment;
//#endregion


//#region Demo3Model
function Demo3Model() { }
Demo3Model.prototype = {
    init: function () {
        var myModule = angular.module('myModule', []);

        myModule.factory('oItems', function () {
            var me = {};
            me.getDateFormat = function (txtDate) {
                return MyFunction.formatHelper.dateTime(txtDate);
            };

            me.getData = function () {
                var oDs = [new Payment("rent fee", 1000, new Date("2014/03/15 00:00:00"), true),
                    new Payment("mobile charge", 150, new Date("2014/03/08 00:00:00")),
                    new Payment("Network charge", 40, new Date("2014/03/25 00:00:00"))];

                return oDs
            };

            me.getStatus = function (isPay) {
                if (isPay) {
                    return "Paid";
                } else {
                    return "Unpaid";
                }
            };

            me.showPaid = function (oDataList) {
                var txtPaidList = "";
                if (oDataList !== undefined && oDataList !== null && typeof (oDataList) === "object") {
                    for (var intIndex = 0; intIndex < oDataList.length; intIndex += 1) {
                        if (oDataList[intIndex].isPay) {
                            txtPaidList += " " + oDataList[intIndex].name + ",";
                        }
                    }

                }
                return txtPaidList.substring(0, (txtPaidList.length > 0 ? (txtPaidList.length - 1) : 0));
            };

            return me;
        });

        return myModule;
    }
};
Demo3Model.prototype.constructor = Demo3Model;
//#endregion
new Demo3Model().init();

//#region Demo3Controller
function Demo3Controller($scope,oItems) {
    $scope.DataList = oItems.getData();
    console.log($scope);
}

Demo3Controller.prototype = { };

Demo3Controller.prototype.constructor = Demo3Controller;
//#endregion

$(function () {});