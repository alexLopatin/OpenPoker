﻿"use strict";

var queryDict = {}
location.search.substr(1).split("&").forEach(function (item) { queryDict[item.split("=")[0]] = item.split("=")[1] });

var connection = new signalR.HubConnectionBuilder().withUrl("/roomHub").build();

connection.on("UpdatePlayer", function (args) {
    document.querySelector("#player" + (args.id + 1) + " #bet").textContent = args.bet;
    var cards = document.querySelectorAll("#player" + (args.id + 1) + " #card");
    cards[0].src = GetImagePath(args.cards[0].rank, args.cards[0].suit);
    cards[1].src = GetImagePath(args.cards[1].rank, args.cards[1].suit);
    document.getElementById("table").removeAttribute("hidden");

    var choice = args.choice;
    if (choice == "None")
        return;
    var tag = document.createElement("p");
    tag.textContent = choice;
    tag.setAttribute("id", "caption")
    document.querySelector("#player" + (args.id + 1)).appendChild(tag);
    setTimeout(function () {
        document.querySelector("#player" + (args.id + 1)).removeChild(document.querySelector("#player" + (args.id + 1)).lastChild);
    }, 500);

});

connection.on("UpdateTable", function (args) {
    //deck update
    var deckDom = document.querySelector("#deck");
    var child = deckDom.lastElementChild;
    while (child) {
        deckDom.removeChild(child);
        child = deckDom.lastElementChild;
    }
    for (var i = 0; i < args.cards.length; i++) {
        var tag = document.createElement("img");
        tag.setAttribute("id", "card");
        tag.setAttribute("src", GetImagePath(args.cards[i].rank, args.cards[i].suit));
        deckDom.appendChild(tag);
    }
    document.getElementById("table").removeAttribute("hidden");
});

connection.on("EndGameUpdate", function (args) {
    //deck update
    var endText = document.querySelector("#endResult");
    endText.textContent = args.final;
    setTimeout(function () {
        endText.textContent = "";
    }, 4000);
    document.getElementById("table").removeAttribute("hidden");
});

connection.on("Reject", function (reason) {
    alert(reason);
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

function MakeBet() {
    var bet = document.getElementById("betText").textContent;
    connection.invoke("MakeBet", bet).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("menu").setAttribute("hidden", "hidden");
}

function GetImagePath(rank, suit) {
    return "cards/" + RankToString(rank) + "_of_" + SuitToString(suit) + ".png";
}

function SuitToString(suit) {
    var s;
    switch (suit) {
        case 0:
            s = "spades";
            break;
        case 1:
            s = "hearts";
            break;
        case 2:
            s = "diamonds";
            break;
        case 3:
            s = "clubs";
            break;
        default:
    }
    return s;
}

function RankToString(rank)
{
    var r = rank;
    switch (rank) {
        case 11:
            r = "jack";
            break;
        case 12:
            r = "queen";
            break;
        case 13:
            r = "king";
            break;
        case 14:
            r = "ace";
            break;
        default:
    }
    return r;
}

connection.start().then(function () {
    connection.invoke("JoinRoom", queryDict["roomId"]).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

function updateText(val) {
    document.getElementById("betText").textContent = val;
}