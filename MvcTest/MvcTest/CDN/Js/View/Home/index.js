/// <reference path="../../Lib/jquery-1.10.2.js" />
/// <reference path="../../Lib/jquery.extend.js" />
/// <reference path="../../Lib/jStorage.js" />
/// <reference path="../../Lib/json2.js" />
/// <reference path="../../Lib/knockout-2.3.0.js" />
/*global alert,$,Enumerable,console,dialog,l,document,
location,Utility,setTimeout,jQuery,require,window,arguments,navigator*/

function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showLocation);
    } else {
        alert("Your browser not support location.")
    }
};

function showLocation(position) {
    var latitude = position.coords.latitude,
        longitude = position.coords.longitude;
    $("#location").text("you are at Latitude:" + latitude + ", Longitude:" + longitude);
};
$(function () {
    getLocation();
});