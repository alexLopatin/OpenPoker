
var place = 4;

function Shift() {
    if (place <= 3)
        return 3 - place;
    else
        return 9 - place;
}

function Modulus(a, b) {
    // Calculate
    return ((a % b) + b) % b;
}

function EndGameUpdate(args) {
    //deck update
    var endText = document.querySelector("#endResult");
    endText.textContent = args.final;
    setTimeout(function () {
        endText.textContent = "";
        for (var i = 0; i < 6; i++) {
            var cards = document.querySelectorAll("#player" + (i + 1) + " #card");
            cards[0].src = "cards/back.png";
            cards[1].src = "cards/back.png";
        }
    }, 4000);
    document.getElementById("table").removeAttribute("hidden");
}

function UpdateTable(args) {
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
}

function UpdatePlayer(args) {
    var newId = (Math.abs(args.id + 6 - place + 3)) % 6;
    document.querySelector("#player" + (newId + 1) + " #name").textContent = "Player " + (args.id + 1);
    args.id = newId;
    document.querySelector("#player" + (args.id + 1) + " #bet").textContent = args.bet;
    var cards = document.querySelectorAll("#player" + (args.id + 1) + " #card");
    if (args.cards.length > 0) {
        cards[0].src = GetImagePath(args.cards[0].rank, args.cards[0].suit);
        cards[1].src = GetImagePath(args.cards[1].rank, args.cards[1].suit);
    }
    document.querySelector("#player" + (args.id + 1) + " #name").textContent = args.name;
    document.getElementById("table").removeAttribute("hidden");
    if (!args.isDisconnected)
        document.getElementById("player" + (args.id + 1)).removeAttribute("hidden");
    else
        document.getElementById("player" + (args.id + 1)).setAttribute("hidden", "hidden");
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

function RankToString(rank) {
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

function updateText(val) {
    document.getElementById("betText").textContent = val;
}

function ShowCards(isShown) {
    if (isShown) {
        connection.invoke("ShowCards").catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {

        for (var i = 0; i < 6; i++) {
            if (i == 3)
                continue;
            var cards = document.querySelectorAll("#player" + (i + 1) + " #card");
            cards[0].src = "cards/back.png";
            cards[1].src = "cards/back.png";
        }
    }
}

function redirect(url, returnUrl) {
    var ua = navigator.userAgent.toLowerCase(),
        isIE = ua.indexOf('msie') !== -1,
        version = parseInt(ua.substr(4, 2), 10);

    // Internet Explorer 8 and lower
    if (isIE && version < 9) {
        var link = document.createElement('a');
        link.href = url;
        document.body.appendChild(link);
        link.click();
    }

    // All other browsers can use the standard window.location.href (they don't lose HTTP_REFERER like Internet Explorer 8 & lower does)
    else {
        if (returnUrl != null)
            window.location.href = url + "?returnUrl=" + returnUrl;
        else
            window.location.href = url;
    }
}