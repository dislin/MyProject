function TestDropDownJS() {}
TestDropDownJS.prototype = {
    create: function () {
        var me = {};
        //#region init
        me.init = function () {
            this.getData();
            this.initEvent();
        };
        //#endregion

        //#region init
        me.initEvent = function () {
            var that = this;
            $("#intMemberID").on("change", function () {
                that.getData();
            });
        };
        //#endregion

        //#region getData
        me.getData = function () {
            var oData = { MemberID: $("#intMemberID").val() },
                txtContent = "";

            $.ajax({
                url: '../../AjaxService/GetTestDropDownJSData',
                type: "POST",
                cache: false,
                dataType: "json",
                data: oData,
                success: function (data) {
                    for (var num = 0; num < data.length; num += 1) {
                        txtContent += "<tr><td>ID:" + data[num].Id + "</td>";
                        txtContent += "<td>Name:" + data[num].Name + "</td>"
                        txtContent += "<td>Age:" + data[num].Age + "</td>"
                        txtContent += "<td>Sex:" + data[num].Sex + "</td></tr>"
                    }
                    $("#Content").html(txtContent);
                }
            });
        };
        //#endregion
        return me.init();
    }
};
TestDropDownJS.prototype.constructor = TestDropDownJS;

$(function () {
    var obj = new TestDropDownJS().create();
});