﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenPoker.Models.Update;

namespace OpenPoker.GameEngine
{
    public class GameUpdateArgs
    {
        public List<KeyValuePair <Action, object>> ActionArgumentsPairs { get; set; } = new List<KeyValuePair<Action, object>>();
        public bool isEndGameUpdate { get; set; } = false;
    }
    public class UpdateComposer
    {
        
        private Game game;
        public UpdateComposer(Game game)
        {
            this.game = game;
        }
        public GameUpdateArgs UpdateAll(bool showCards = false)
        {
            GameUpdateArgs args = new GameUpdateArgs();
            for(int i = 0; i < game.players.Count; i++)
            {
                List<Card> cards;
                //don't show players' cards who folded
                if (showCards && game.players[i].bet != -1)
                    cards = game.players[i].cards;
                else
                    cards = new List<Card>();
                args.ActionArgumentsPairs.Add(
                    new KeyValuePair<Action, object>(
                        new Action("UpdatePlayer"), 
                    new PlayerUpdateModel(game.players[i].Id, new List<Card>(cards), game.players[i].bet, game.players[i].Name,
                    "None", game.players[i].IsDisconnected)
                    ));

                //Personal update for specific user (i.e. showing only his cards)
                //Happen only once when player gets his cards
                if(game.players[i] is NetworkPlayer && game.players[i].bet != -1)
                {
                    var netPlayer = game.players[i] as NetworkPlayer;
                    args.ActionArgumentsPairs.Add(
                        new KeyValuePair<Action, object>(
                            new Action("UpdatePlayer", netPlayer.ConnectionId),
                        new PlayerUpdateModel(netPlayer.Id, new List<Card>(netPlayer.cards), netPlayer.bet, game.players[i].Name,
                        "None", netPlayer.IsDisconnected)
                        ));
                }
            }
            args.ActionArgumentsPairs.Add(
                    new KeyValuePair<Action, object>(
                        new Action("UpdateTable"),
                    new TableUpdateModel(new List<Card>(game.cards))
                    ));
            return args;
        }

        public GameUpdateArgs UpdateOnlyOnePlayer(int id, int diff, bool showCards = false)
        {
            string choice = "None";
            if (diff == -2)
                choice = "Check";
            if (diff == -1)
                choice = "Fold";
            if(diff == 0)
                choice = "Call";
            if (diff > 0)
                choice = "Bet";
            GameUpdateArgs args = new GameUpdateArgs();
            IPlayer p = game.players.Find(p => p.Id == id);
            List<Card> cards;
            if (showCards)
                cards = new List<Card>(p.cards);
            else
                cards = new List<Card>();
            args.ActionArgumentsPairs.Add(
                    new KeyValuePair<Action, object>(
                        new Action("UpdatePlayer"),
                    new PlayerUpdateModel(id, cards, p.bet, p.Name, choice, p.IsDisconnected)
                    ));
            return args;
        }
        public GameUpdateArgs EndGameUpdate(string final)
        {
            GameUpdateArgs args = UpdateAll(true);
            args.isEndGameUpdate = true;
            args.ActionArgumentsPairs.Add(
                    new KeyValuePair<Action, object>(
                        new Action("EndGameUpdate"),
                    new EndGameUpdateModel(final)
                    ));
            return args;
        }
        public GameUpdateArgs EndGameUpdate(string final, IPlayer Winner, int cash)
        {
            GameUpdateArgs args = UpdateAll(true);
            args.isEndGameUpdate = true;
            string WinnerId = "BOT";
            if (Winner is NetworkPlayer)
                WinnerId = ((NetworkPlayer)Winner).User.Id;
            args.ActionArgumentsPairs.Add(
                    new KeyValuePair<Action, object>(
                        new Action("EndGameUpdate"),
                    new EndGameUpdateModel(final, WinnerId, cash)
                    ));
            return args;
        }
        public GameUpdateArgs GameClose(string reason)
        {
            GameUpdateArgs args = new GameUpdateArgs();

            args.ActionArgumentsPairs.Add(
                    new KeyValuePair<Action, object>(
                        new Action("GameClose"),
                    reason
                    ));
            return args;
        }
        public GameUpdateArgs UpdateTable()
        {
            GameUpdateArgs args = new GameUpdateArgs();
            args.ActionArgumentsPairs.Add(
                    new KeyValuePair<Action, object>(
                        new Action("UpdateTable"),
                    new TableUpdateModel( new List<Card>( game.cards))
                    ));
            return args;
        }
    }
}
