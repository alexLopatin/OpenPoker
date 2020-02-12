"use strict";

var queryDict = {}
location.search.substr(1).split("&").forEach(function (item) { queryDict[item.split("=")[0]] = item.split("=")[1] });

var connection = new signalR.HubConnectionBuilder().withUrl("/roomHub").build();

connection.on("UpdatePlayer", function (player) {

});

connection.start().then(function () {
    connection.invoke("JoinRoom", queryDict["roomId"]).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});