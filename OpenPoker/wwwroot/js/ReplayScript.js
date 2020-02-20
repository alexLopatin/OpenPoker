
var listOfActions = JSON.parse(text);

for (var i = 0; i < listOfActions.length; i++) {
    setTimeout(Tick, listOfActions[i].Value.time, i);
}

function Tick(i) {
    console.log(listOfActions[i]);
    DoAction(listOfActions[i].Key.name, listOfActions[i].Value.argument)
}

function DoAction(name, args) {
    switch (name) {
        case "UpdatePlayer":
            UpdatePlayer(args);
            break;
        case "UpdateTable":
            UpdateTable(args);
            break;
        case "EndGameUpdate":
            EndGameUpdate(args);
            break;
    }
}