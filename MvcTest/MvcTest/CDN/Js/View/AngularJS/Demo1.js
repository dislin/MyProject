
//#region demo1
function demo1() { }
demo1.prototype = {
    init: function () {
        var me = {};
        me.name = "Leo";
        me.age = 30;
        me.isShowAge = true;

        me.saySomething = function () {
            return "hello " + me.name + ", you are " + me.age + " years old.";
        };

        me.addOneYearOld = function () {
            me.age = (Number(me.age) + 1);
        };

        me.refreshSaySomething = function () {
            $("#txtSaySomething").text(me.saySomething());
        };

        me.isShowAgeFn = function () {
            me.isShowAge = (me.age > 18);
        };

        me.ageIsValid = function () {
            if (isNaN(me.age) && Number(me.age) !== 0) {
                alert("pls enter correct number format");
                me.age = 30;
            }
        };

        me.ageChangeEvent = function () {
            me.refreshSaySomething();
            me.isShowAgeFn();
            me.ageIsValid();
        }
        return me;
    }
};
demo1.prototype.constructor = demo1;
//#endregion


//#region demo1Controller
function demo1Controller($scope) {
    $scope.model = this.demo1;
    $scope.$watch("model.age", $scope.model.ageChangeEvent);
}
demo1Controller.prototype = {
    demo1: new demo1().init()
};
demo1Controller.prototype.constructor = demo1Controller;
//#endregion

$(function () {
    $("#btn1").on("click", function () {

    });
});