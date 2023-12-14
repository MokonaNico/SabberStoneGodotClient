using Godot;
using System;
using System.Collections.Generic;

public class MainMenu : Control
{
    [Export] private NodePath player1OptionPath;
    [Export] private NodePath player2OptionPath;
    
    private OptionButton player1Decks;
    private OptionButton player2Decks;
    
    public override void _Ready()
    {
        player1Decks = GetNode<OptionButton>(player1OptionPath);
        player2Decks = GetNode<OptionButton>(player2OptionPath);
        
        List<string> deckNames = SaveHandler.getListDeckNames();

        foreach (var deckName in deckNames)
        {
            player1Decks.AddItem(deckName);
            player2Decks.AddItem(deckName);
        }
    }

    private void onDeckBuilderButtonPressed()
    {
        Main.windowHandler.switchTo("DeckBuilder");
    }

    private void onStartButtonPressed()
    {
        Main.windowHandler.player1DeckSelected = player1Decks.GetItemText(player1Decks.Selected);
        Main.windowHandler.player2DeckSelected = player2Decks.GetItemText(player2Decks.Selected);
        
        Main.windowHandler.switchTo("Game");
    }
}
