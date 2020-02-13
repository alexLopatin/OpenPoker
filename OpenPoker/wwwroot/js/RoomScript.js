"use strict";

var queryDict = {}
location.search.substr(1).split("&").forEach(function (item) { queryDict[item.split("=")[0]] = item.split("=")[1] });

var connection = new signalR.HubConnectionBuilder().withUrl("/roomHub").build();

connection.on("UpdateGame", function (players, deck) {
    //player state update
    for (var i = 0; i < players.length; i++) {
        document.querySelector("#player" + (i + 1) + " #bet").textContent = players[i].bet;
        var cards = document.querySelectorAll("#player" + (i + 1) + " #card");
        cards[0].src = GetImagePath(players[i].cards[0].rank, players[i].cards[0].suit);
        cards[1].src = GetImagePath(players[i].cards[1].rank, players[i].cards[1].suit);
    }
    //deck update
    var deckDom = document.querySelector("#deck");
    var child = deckDom.lastElementChild;
    while (child) {
        deckDom.removeChild(child);
        child = deckDom.lastElementChild;
    }
    for (var i = 0; i < deck.length; i++) {
        var tag = document.createElement("img");
        tag.setAttribute("id", "card");
        tag.setAttribute("src", GetImagePath(deck[i].rank, deck[i].suit));
        deckDom.appendChild(tag);
    }
});

connection.on("UpdatePlayer", function (playerId, action) {
    var tag = document.createElement("p");
    var node = document.createTextNode(action);
    tag.textContent = action;
    tag.setAttribute("style", "text-align:center")
    //document.querySelector("#player" + (playerId + 1) + " #bet").textContent = action;
    document.querySelector("#player" + (playerId + 1)).appendChild(tag);
    setTimeout(function () {
        document.querySelector("#player" + (playerId + 1)).removeChild(document.querySelector("#player" + (playerId + 1)).lastChild);
    }, 500);
});


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