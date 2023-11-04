using Godot;
using System;

public class DeckBuilder : Control
{
    public PackedScene YourPackedScene; 
    private VBoxContainer _vBoxContainer;
    
    public override void _Ready()
    {
        YourPackedScene = (PackedScene)ResourceLoader.Load("res://Scenes/CardTableLine.tscn");
        _vBoxContainer = GetNode<VBoxContainer>("HBoxContainer/CardList/Table/Data/ScrollContainer/MarginContainer/VBoxContainer");

        for (int i = 0; i < 20; i++)
        {
            Node instance = YourPackedScene.Instance();
            Label idLabel = instance.GetNode<Label>("HBoxContainer/Id");
            Label nameLabel = instance.GetNode<Label>("HBoxContainer/Name");
            Label typeLabel = instance.GetNode<Label>("HBoxContainer/Type");
            Label costLabel = instance.GetNode<Label>("HBoxContainer/Cost");
            Label classLabel = instance.GetNode<Label>("HBoxContainer/Class");
            idLabel.Text = "CS2_189";
            nameLabel.Text = "Elven Archer";
            typeLabel.Text = "Minion";
            costLabel.Text = "1";
            classLabel.Text = "Neutral";
            
            _vBoxContainer.AddChild(instance);
        }

    }
}
