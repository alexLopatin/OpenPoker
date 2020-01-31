using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsolePoker;

namespace UnitTestConsole
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void CheckPair1()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Clubs, 10));
            cards.Add(new Card(Card.Suit.Clubs, 3));
            cards.Add(new Card(Card.Suit.Spades, 14));
            cards.Add(new Card(Card.Suit.Hearts, 6));
            cards.Add(new Card(Card.Suit.Diamonds, 12));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Spades, 10));
            playerCards.Add(new Card(Card.Suit.Hearts, 11));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res >= 100000000 && res < 200000000);
        }
        [TestMethod]
        public void CheckPair2()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Spades, 10));
            cards.Add(new Card(Card.Suit.Hearts, 3));
            cards.Add(new Card(Card.Suit.Spades, 14));
            cards.Add(new Card(Card.Suit.Hearts, 7));
            cards.Add(new Card(Card.Suit.Spades, 8));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Diamonds, 14));
            playerCards.Add(new Card(Card.Suit.Diamonds, 2));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res >= 100000000 && res < 200000000);
        }
        [TestMethod]
        public void CheckDoublePair1()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Spades, 10));
            cards.Add(new Card(Card.Suit.Hearts, 2));
            cards.Add(new Card(Card.Suit.Spades, 14));
            cards.Add(new Card(Card.Suit.Hearts, 7));
            cards.Add(new Card(Card.Suit.Spades, 8));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Diamonds, 14));
            playerCards.Add(new Card(Card.Suit.Diamonds, 2));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res >= 200000000 && res < 300000000);
        }
        [TestMethod]
        public void CheckDoublePair2()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Diamonds, 14));
            cards.Add(new Card(Card.Suit.Hearts, 3));
            cards.Add(new Card(Card.Suit.Spades, 14));
            cards.Add(new Card(Card.Suit.Hearts, 7));
            cards.Add(new Card(Card.Suit.Clubs, 2));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Hearts, 5));
            playerCards.Add(new Card(Card.Suit.Diamonds, 7));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res >= 200000000 && res < 300000000);
        }
        [TestMethod]
        public void CheckHighCard1()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Diamonds, 13));
            cards.Add(new Card(Card.Suit.Hearts, 3));
            cards.Add(new Card(Card.Suit.Spades, 14));
            cards.Add(new Card(Card.Suit.Hearts, 7));
            cards.Add(new Card(Card.Suit.Clubs, 2));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Hearts, 5));
            playerCards.Add(new Card(Card.Suit.Diamonds, 8));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res == 805);
        }
        [TestMethod]
        public void CheckHighCard2()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Hearts, 2));
            cards.Add(new Card(Card.Suit.Hearts, 3));
            cards.Add(new Card(Card.Suit.Spades, 11));
            cards.Add(new Card(Card.Suit.Hearts, 7));
            cards.Add(new Card(Card.Suit.Clubs, 9));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Clubs, 6));
            playerCards.Add(new Card(Card.Suit.Clubs, 14));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res == 1406);
        }
        [TestMethod]
        public void CheckTriple1()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Hearts, 3));
            cards.Add(new Card(Card.Suit.Spades, 3));
            cards.Add(new Card(Card.Suit.Spades, 11));
            cards.Add(new Card(Card.Suit.Hearts, 7));
            cards.Add(new Card(Card.Suit.Clubs, 9));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Clubs, 6));
            playerCards.Add(new Card(Card.Suit.Clubs, 3));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res >= 300000000 && res < 400000000);
        }
        [TestMethod]
        public void CheckTriple2()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Hearts, 3));
            cards.Add(new Card(Card.Suit.Spades, 8));
            cards.Add(new Card(Card.Suit.Spades, 11));
            cards.Add(new Card(Card.Suit.Hearts, 7));
            cards.Add(new Card(Card.Suit.Clubs, 9));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Hearts, 3));
            playerCards.Add(new Card(Card.Suit.Clubs, 3));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res >= 300000000 && res < 400000000);
        }
        [TestMethod]
        public void CheckFullHouse1()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Hearts, 3));
            cards.Add(new Card(Card.Suit.Spades, 7));
            cards.Add(new Card(Card.Suit.Spades, 11));
            cards.Add(new Card(Card.Suit.Hearts, 7));
            cards.Add(new Card(Card.Suit.Clubs, 9));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Hearts, 3));
            playerCards.Add(new Card(Card.Suit.Clubs, 3));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res == 600000307);
        }
        [TestMethod]
        public void CheckStreet1()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Hearts, 2));
            cards.Add(new Card(Card.Suit.Spades, 7));
            cards.Add(new Card(Card.Suit.Spades, 4));
            cards.Add(new Card(Card.Suit.Hearts, 7));
            cards.Add(new Card(Card.Suit.Clubs, 5));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Hearts, 3));
            playerCards.Add(new Card(Card.Suit.Clubs, 6));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res >= 400000000 && res < 500000000);
        }
        [TestMethod]
        public void CheckFlash1()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Hearts, 9));
            cards.Add(new Card(Card.Suit.Spades, 7));
            cards.Add(new Card(Card.Suit.Spades, 4));
            cards.Add(new Card(Card.Suit.Hearts, 7));
            cards.Add(new Card(Card.Suit.Hearts, 5));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Hearts, 3));
            playerCards.Add(new Card(Card.Suit.Hearts, 6));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res >= 500000000 && res < 600000000);
        }
        [TestMethod]
        public void CheckNotFlash1()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Hearts, 10));
            cards.Add(new Card(Card.Suit.Hearts, 11));
            cards.Add(new Card(Card.Suit.Hearts, 12));
            cards.Add(new Card(Card.Suit.Hearts, 13));
            cards.Add(new Card(Card.Suit.Hearts, 14));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Hearts, 3));
            playerCards.Add(new Card(Card.Suit.Hearts, 6));
            int res = game.CalculateCombination(playerCards);
            Assert.IsFalse( res >= 500000000 && res < 600000000);
        }
        [TestMethod]
        public void CheckStreetFlash1()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Hearts, 8));
            cards.Add(new Card(Card.Suit.Spades, 7));
            cards.Add(new Card(Card.Suit.Spades, 4));
            cards.Add(new Card(Card.Suit.Hearts, 7));
            cards.Add(new Card(Card.Suit.Hearts, 5));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Hearts, 4));
            playerCards.Add(new Card(Card.Suit.Hearts, 6));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res >= 800000000 && res < 900000000);
        }
        [TestMethod]
        public void CheckFlashRoyal1()
        {
            Game game = new Game();
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Spades, 10));
            cards.Add(new Card(Card.Suit.Spades, 12));
            cards.Add(new Card(Card.Suit.Spades, 14));
            cards.Add(new Card(Card.Suit.Hearts, 7));
            cards.Add(new Card(Card.Suit.Hearts, 5));
            game.cards = cards;
            List<Card> playerCards = new List<Card>();
            playerCards.Add(new Card(Card.Suit.Spades, 11));
            playerCards.Add(new Card(Card.Suit.Spades, 13));
            int res = game.CalculateCombination(playerCards);
            Assert.IsTrue(res >= 900000000 && res < 1000000000);
        }
    }
}
