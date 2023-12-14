using Godot;
using System;
using System.Collections.Generic;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;

public class Game : Control
{
    private List<string> player1Deck;
    private List<string> player2Deck;

    private SabberStoneCore.Model.Game game;

    private PackedScene cardsTemplate;
    
    public override void _Ready()
    {
        cardsTemplate = (PackedScene)ResourceLoader.Load("res://Scenes/Card.tscn");
        
        // Load the cards from the save file with the deck name
        player1Deck = SaveHandler.getCardsFromDeck(Main.windowHandler.player1DeckSelected);
        player2Deck = SaveHandler.getCardsFromDeck(Main.windowHandler.player2DeckSelected);

        // Create and start the game
        GameConfig gameConfig = new GameConfig
        {
            StartPlayer = 1,
            
            Player1Name = "Yui",
            Player1HeroClass = CardClass.MAGE,
            Player1Deck = getCardListById(player1Deck),
            
            Player2Name = "Alice",
            Player2HeroClass = CardClass.MAGE,
            Player2Deck = getCardListById(player2Deck),
            
            FillDecks = false,
            History = true,
            Shuffle = true,
        };
        
        game = new SabberStoneCore.Model.Game(gameConfig);
        game.StartGame();
    }

    private void renderGameState()
    {
        // Player 1 board card
        foreach (var cards in game.CurrentPlayer.HandZone)
        {
            Card instance = (Card) cardsTemplate.Instance();
            CardInfo cardInfo = new CardInfo
            {
                 cost = cards.Cost,
                 id = cards
            };
            
            instance.setCardInfo();
        }
    }
    
    private List<SabberStoneCore.Model.Card> getCardListById(List<string> ids)
    {
        List<SabberStoneCore.Model.Card> outCards = new List<SabberStoneCore.Model.Card>();
        foreach (string id in ids)
        {
            outCards.Add(SabberStoneCore.Model.Cards.FromId(id));
        }
        return outCards;
    }
}
