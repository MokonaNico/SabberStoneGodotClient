using Godot;
using System;
using System.Collections.Generic;

public class Main : Control
{
    static public Main windowHandler;

    private Node currentScene = null;
    private Godot.Collections.Dictionary<string, PackedScene> scenes = new Godot.Collections.Dictionary<string, PackedScene>();

    public string player1DeckSelected;
    public string player2DeckSelected;
    
    public override void _Ready()
    {
        windowHandler = this;
        scenes.Add("MainMenu", (PackedScene) ResourceLoader.Load("res://Scenes/MainMenu.tscn"));
        scenes.Add("DeckBuilder", (PackedScene) ResourceLoader.Load("res://Scenes/DeckBuilder.tscn"));
        scenes.Add("Game", (PackedScene) ResourceLoader.Load("res://Scenes/Game.tscn"));
        OS.MinWindowSize = new Vector2(1024,600);
        
        switchTo("MainMenu");
    }

    public void switchTo(string sceneName)
    {
        if (currentScene != null)
        {
            currentScene.QueueFree();
        }

        currentScene = scenes[sceneName].Instance();
        AddChild(currentScene);
    }

    public List<string> getPlayerDeck(int playerNumber)
    {
        if (playerNumber < 0 || playerNumber > 1 ) 
            return new List<string>();

        string deck;
        if (playerNumber == 0) 
            deck = player1DeckSelected;
        else 
            deck = player2DeckSelected;
        
        if (deck == null)
            return new List<string>();

        return SaveHandler.getCardsFromDeck(deck);
    }
}
