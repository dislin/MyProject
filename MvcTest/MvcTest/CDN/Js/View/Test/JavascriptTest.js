// <reference path="../../Lib/jquery-1.10.2.js" />
/// <reference path="../../Lib/jquery.extend.js" />
/// <reference path="../../Common.js" />
/// <reference path="../../Lib/jStorage.js" />
/// <reference path="../../Lib/json2.js" />
/// <reference path="../../Lib/knockout-2.3.0.js" />
/*global alert,$,Enumerable,console,dialog,l,document,
location,Utility,setTimeout,jQuery,require,window,arguments,navigator*/

$(function () {
    var d = new dialog();
    d.alert("welcome", "pls try it.");

    $("#btn1").on("click", function () {
        var btnOk = new dialogButton("Ok", function () {
                alert("ok");
                d.close();
            }),
            btnCancel = new dialogButton("Cancel", function () {
                d.close();
            }),
            arrButtons = [btnOk, btnCancel];

        d.confirm("oh!oh! click me", "I click confirm btn", arrButtons);
    });
});