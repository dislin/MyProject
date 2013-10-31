/// <reference path="jquery-1.10.2.js" />
/// <reference path="jStorage.js" />
/// <reference path="json2.js" />
/*global alert,$,Enumerable,console,dialog,l,document,
location,Utility,setTimeout,jQuery,require,window,arguments,navigator*/

//#region String function

//#region String.Format
String.Format = function () {
    if (arguments.length === 0) {
        return null;
    }
    var str = arguments[0], i = 1, reg = null;
    for (i = 1; i < arguments.length; i += 1) {
        reg = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(reg, arguments[i]);
    }
    return str;
};
//#endregion

//#region String.RightPad
String.RightPad = function (obj, str, num) {
    var RtStr = "", i = 0, j = 0,
        tmpStr = "";
    tmpStr += obj;
    for (i = 0; i < num; i += 1) {
        tmpStr += str;
    }
    for (j = 0; j < num; j += 1) {
        RtStr += tmpStr.charAt(j);
    }
    return RtStr;
};
//#endregion

//#region String.LeftPad
String.LeftPad = function (obj, str, num) {
    var objLength = 0, i = 0, j = 0,
        RtStr = "",
        tmpStr = "";
    for (i = 0; i < num; i += 1) {
        tmpStr += str;
    }
    tmpStr += obj;
    objLength = tmpStr.length;
    for (j = 0; j < num; j += 1) {
        RtStr = tmpStr.charAt(objLength - j - 1) + RtStr;
    }
    return RtStr;
};
//#endregion

//#region String.SerializeProperty
String.SerializeProperty = function (obj) {
    var txtPropList = "";
    txtPropList += "{";
    for (prop in obj) {
        txtPropList += String.Format('"{0}":"{1}",', prop, typeof(obj[prop]));
    }
    txtPropList = txtPropList.substr(0, (txtPropList.length - 1)) + "}";
    return txtPropList;
};
//#endregion

//#endregion

//#region Myfunction
//this one is a function base, can not inherit this one
var MyFunction = {

    //#region inherit
    inherit: function (_child, _parent) {
        var _tempObj = function () { };
        _tempObj.prototype = _parent.prototype;
        _child.prototype = new _tempObj();
        _child.superclassical = _parent.superclassical || _parent.prototype;
        _child.prototype.constructor = _child;
    },
    //#endregion

    //#region isExist
    isExist: function (_val) {
        if (_val === null || _val === undefined) {
            return false;
        } else {
            return true;
        }
    },
    //#endregion

    //#region extend
    extend: function (_parent, _child) {
        var p;
        _child = _child || {};
        for (p in _parent) {
            if (_parent.hasOwnProperty(p)) {
                _child[p] = _parent[p];
            }
        }
        return _child;
    },
    //#endregion

    //#region extendAll
    extendAll: function (_parent, _child) {
        var p,
            toStr = Object.prototype.toString,
            astr = "[object Array]";
        _child = _child || {};
        for (p in _parent) {
            if (_parent.hasOwnProperty(p)) {
                if (typeof _parent[p] === "object") {
                    _child[p] = (toStr.call(_parent[p]) === astr) ? [] : {};
                    this.extendAll(_parent[p], _child[p]);
                } else {
                    _child[p] = _parent[p];
                }
            }
        }
        return _child;
    },
    //#endregion

    //#region validator
    validator: {
        types: {},
        messages: [],
        config: {},
        isUserMessage: false,
        userMessageFunction: null,
        validate: function (_data) {
            var i, msg, type, checker, result;
            this.messages = [];

            for (i in _data) {

                if (i.toString() === "constructor") {
                    continue;
                }

                type = this.config[i];
                checker = this.types[type];

                if (!type) {
                    continue;
                }

                if (!checker) {
                    msg = String.Format("No handle this kind of validation: {0}", type.toString());
                    throw new Error(msg);
                }

                result = checker.validate(_data[i]);
                if (!result) {
                    msg = String.Format("InValid value for {0}, Error Message: {1}, Constructor: {2}\n",
                        i.toString(),
                        checker.instructions.toString(),
                        MyFunction.getConstructorName(_data));
                    this.messages.push(msg);
                }
            }
            return this.hasErrors();
        },
        hasErrors: function () {
            if (this.messages.length !== 0 && !this.isUserMessage) {
                throw new Error(this.messages.join("\n"));
            }
            if (this.messages.length !== 0 && this.isUserMessage) {
                if (typeof this.userMessageFunction === "function") {
                    this.userMessageFunction(this.messages.join("\n"));
                } else {
                    alert(this.messages.join("\n"));
                }
            }
        }
    },
    //#endregion

    //#region getBrowserVersion
    getBrowserVersion: function () {
        var txtBrowserName = "",
            objBrowser = null;
        for (txtBrowserName in myBrowserVersion) {
            objBrowser = myBrowserVersion[txtBrowserName];

            if (navigator.userAgent.search(myBrowserVersion.Safari.Key) > -1) {
                if (navigator.userAgent.search(myBrowserVersion.Chrome.Key) > -1) {
                    return myBrowserVersion.Chrome.Value; //Chrome also
                } else {
                    return myBrowserVersion.Safari.Value;
                }
            } else {
                if (navigator.userAgent.search(objBrowser.Key) > -1) {
                    return objBrowser.Value;
                }
            }
        }

        return "other";
    },
    //#endregion

    //#region getConstructorName
    getConstructorName: function (obj) {
        var browser = this.getBrowserVersion(),
            reg = /\s{2,}/g,
            strConstructor = obj.constructor.toString().replace(reg, " "),
            arrConstructor = [],
            arrSubConstructor = [];
        if (browser.toString().indexOf("ie") > -1) {
            arrConstructor = strConstructor.split("(");
            if (arrConstructor[0].length > 0) {
                arrSubConstructor = arrConstructor[0].split("function ");
                return arrSubConstructor[1];
            }
        } else {
            return obj.constructor.name;
        }
    },
    //#endregion

    //#region mySession
    mySession: {
        set: function (txtKey, oValue) {
            if (!this.isSupportHTML5()) {
                $.jStorage.set(txtKey, JSON.stringify(oValue))
            } else {
                localStorage.setItem(txtKey, JSON.stringify(oValue));
            }
        },

        get: function (txtKey) {
            var oValue = null, txt;

            if (!this.isSupportHTML5()) {
                oValue = $.jStorage.get(txtKey) != null ? jQuery.parseJSON($.jStorage.get(txtKey)) : null;
            } else {
                oValue = localStorage[txtKey] != null ? jQuery.parseJSON(localStorage[txtKey]) : null;
            }
            return oValue;
        },

        clearAll: function () {
            if (!this.isSupportHTML5()) {
                $.jStorage.flush();
            } else {
                localStorage.clear();
            }
        },

        removeItem: function (txtKey) {
            if (!this.isSupportHTML5()) {
                $.jStorage.deleteKey(txtKey);
            } else {
                localStorage.removeItem(txtKey);
            }
        },

        isSupportHTML5: function () {
            var txtBrowserVersion = MyFunction.getBrowserVersion();
            if (txtBrowserVersion == myBrowserVersion.IE6.Value || txtBrowserVersion == myBrowserVersion.IE7.Value) {
                if ($.jStorage == null) {
                    throw new Error("Your browser not support HTML5, please use Chrome or FireFox, or import jStorage.js in your IE.")
                }
                return false;
            } else {
                return true;
            }
        }
    },
    //#endregion
};

var myBrowserVersion = {
    IE6: {
            Key:"MSIE 6",
            Value: "ie6"
        },

    IE7: {
            Key:"MSIE 7",
            Value: "ie7"
        },

    IE8: {
            Key: "MSIE 8",
            Value: "ie8"
    },

    IE9: {
            Key: "MSIE 9",
            Value: "ie9"
        },

    IE10: {
            Key: "MSIE 10",
            Value: "ie10"
        },

    IE11: {
            Key: "MSIE 11",
            Value: "ie11"
        },

    Chrome: {
            Key: "Chrome",
            Value: "chrome"
        },

    Firefox: {
            Key: "Firefox",
            Value: "firefox"
        },

    Safari: {
            Key: "Safari",
            Value: "safari"
        },

    Opera: {
            Key: "Opera",
            Value: "opera"
        }
    };

//#region MyFunction.validator.type extend
// when add new validation extend, please don't forget add new one in enum
var EnumValidation = {
    isExist: "isExist",
    isNumber: "isNumber",
    isBoolean: "isBoolean",
    isEmpty: "isEmpty",
    isSevenStarNumber: "isSevenStarNumber",
    isSevenStarNumberEnum: "isSevenStarNumberEnum",
    isSevenStarThousandNumber: "isSevenStarThousandNumber",
    isSevenStarHundredNumber: "isSevenStarHundredNumber",
    isSevenStarTenNumber: "isSevenStarTenNumber",
    isSevenStarUnitNumber: "isSevenStarUnitNumber"
};

MyFunction.validator.types.isExist = {
    validate: function (_val) {
        //exist is mean the value can not equal null or undefined
        if (_val == null) {
            return false;
        } else {
            return true;
        }
    },
    instructions: "the value cannot be null or empty"
};

MyFunction.validator.types.isNumber = {
    validate: function (_val) {
        return !isNaN(_val);
    },
    instructions: "the value must be number"
};

MyFunction.validator.types.isBoolean = {
    validate: function (_val) {
        return (typeof _val === "boolean");
    },
    instructions: "the value must be boolean"
};

MyFunction.validator.types.isEmpty = {
    validate: function (_val) {
        return (_val.toString() !== "");
    },
    instructions: "the value is empty"
};

MyFunction.validator.types.isSevenStarNumberEnum = {
    validate: function (_val) {
        if (_val === "thousand" || _val === "hundred" || _val === "ten" || _val === "unit") {
            return true;
        } else {
            return false;
        }
    },
    instructions: "the value is not seven star enum number format"
};

MyFunction.validator.types.isSevenStarNumber = {
    validate: function (_val) {
        if (!isNaN(_val) && Number(_val) >= -1 && Number(_val) <= 9) {
            return true;
        } else {
            return false;
        }
    },
    instructions: "the value is not seven star number format"
};

MyFunction.validator.types.isSevenStarThousandNumber = {
    validate: function (_val) {
        if (MyFunction.getConstructorName(_val) === "SevenStarNumber" && _val.type === "thousand") {
            return true;
        } else {
            return false;
        }
    },
    instructions: "the value is not seven star number format (Thousand)"
};

MyFunction.validator.types.isSevenStarHundredNumber = {
    validate: function (_val) {
        if (MyFunction.getConstructorName(_val) === "SevenStarNumber" && _val.type === "hundred") {
            return true;
        } else {
            return false;
        }
    },
    instructions: "the value is not seven star number format (Hundred)"
};

MyFunction.validator.types.isSevenStarTenNumber = {
    validate: function (_val) {
        if (MyFunction.getConstructorName(_val) === "SevenStarNumber" && _val.type === "ten") {
            return true;
        } else {
            return false;
        }
    },
    instructions: "the value is not seven star number format (Ten)"
};

MyFunction.validator.types.isSevenStarUnitNumber = {
    validate: function (_val) {
        if (MyFunction.getConstructorName(_val) === "SevenStarNumber" && _val.type === "unit") {
            return true;
        } else {
            return false;
        }
    },
    instructions: "the value is not seven star number format (Unit)"
};

//#endregion

//#endregion

//#region interface
function Interface(){};
Interface.prototype = {
    set: function (targetObject, interfaceObjects) {
        var txtErrMsg = "",
            isRepeat = false,
            isMapping = false,
            txtTargetObjectPropertys = ""
            arrPropertys = [];

        //#region validation
        if(!(targetObject instanceof Object)) {
            txtErrMsg = String.Format("[Error] Interface.set: targetObject must be a Object type.");
            throw new Error(txtErrMsg);
        }

        if (arguments.length != 2) {
            txtErrMsg = String.Format("[Error] Interface.set: this arguments have error, there are {0} argument(s)", arguments.length);
            throw new Error(txtErrMsg);
        }

        if(interfaceObjects == null || interfaceObjects.length < 1){
            txtErrMsg = String.Format("[Error] Interface.set: interfaceObjects is null or empty");
            throw new Error(txtErrMsg);
        }

        if(Object.prototype.toString.call(interfaceObjects) !== "[object Array]") {
            txtErrMsg = String.Format("[Error] Interface.set: interfaceObjects is not a Array");
            throw new Error(txtErrMsg);
        }
        //#endregion

        txtTargetObjectPropertys = String.SerializeProperty(targetObject);

        for(var numObj in interfaceObjects) {
            for(var txtProp in interfaceObjects[numObj]) {
                isRepeat = JSON.stringify(arrPropertys).toString().indexOf('"' + txtProp + '"') > -1;
                if(txtProp.toString() !== "constructor" && !isRepeat) {
                    if(txtTargetObjectPropertys.indexOf('"' + txtProp + '"') < 0) {
                        txtErrMsg = String.Format("[Error] Interface.set: targetObject '{0}' doesn't have '{1}' property", MyFunction.getConstructorName(targetObject), txtProp);
                        throw new Error(txtErrMsg);
                    }
                }
            }
        }
    }
}
Interface.prototype.constructor = Interface;

//#endregion