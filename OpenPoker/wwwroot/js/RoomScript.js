"use strict";

var queryDict = {}
location.search.substr(1).split("&").forEach(function (item) { queryDict[item.split("=")[0]] = item.split("=")[1] });

var connection = new signalR.HubConnectionBuilder().withUrl("/roomHub").build();

connection.on("UpdatePlayer", function (args) {
    UpdatePlayer(args);
});

connection.on("UpdateTable", function (args) {
    UpdateTable(args);
});

connection.on("EndGameUpdate", function (args) {
    EndGameUpdate(args);
});

connection.on("Reject", function (reason) {
    alert(reason);
});

connection.on("SetupData", function (id) {
    place = id;
});

connection.on("DoBet", function (minBet) {
    //MakeBet(minBet);
    document.getElementById("menu").removeAttribute("hidden");
    var rng = document.getElementById("betRange");
    rng.setAttribute("min", minBet);
    rng.setAttribute("max", minBet * 10);
    rng.setAttribute("step", minBet);
    rng.setAttribute("value", minBet);
    updateText(minBet);
});

connection.on("RedirectToLogin", function () {
    redirect('account/login', window.location.pathname + window.location.search);
});

function KickPlayer(id) {
    connection.invoke("Kick",  Modulus(id - Shift(), 6)).catch(function (err) {
        return console.error(err.toString());
    });
}

function MakeBet() {
    var bet = document.getElementById("betText").textContent;
    connection.invoke("MakeBet", bet).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("menu").setAttribute("hidden", "hidden");
}

function Fold() {
    connection.invoke("MakeBet", "-1").catch(function (err) {
        return console.error(err.toString());
    });
    var cards = document.querySelectorAll("#player" + (4) + " #card");
    cards[0].src = "cards/back.png";
    cards[1].src = "cards/back.png";
    document.getElementById("menu").setAttribute("hidden", "hidden");

}

connection.start().then(function () {
    connection.invoke("JoinRoom", queryDict["roomId"]).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});