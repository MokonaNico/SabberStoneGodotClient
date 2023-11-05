using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SabberStoneCore.Model;

public class CardInfo
{
    public string id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public int cost { get; set; }
    public string cardClass { get; set; }
}

public enum Table
{
    CardTable,DeckTable
}

public class DeckBuilder : Control
{
    public PackedScene PackedCardTableLine; 
    private VBoxContainer CardListTable;
    private VBoxContainer DeckListTable;
    private List<CardInfo> cardsJson;
    
    private TextureRect cardImg;
    private TextureRect cardImgSpell;
    private TextureRect card;
    private TextureRect cardSpell;
    
    public override void _Ready()
    {
        loadJson();
        
        PackedCardTableLine = (PackedScene)ResourceLoader.Load("res://Scenes/CardTableLine.tscn");
        
        CardListTable = GetNode<VBoxContainer>("VBoxContainer/HBoxContainer/CardList/Table/Data/ScrollContainer/MarginContainer/TableData");
        DeckListTable = GetNode<VBoxContainer>("VBoxContainer/HBoxContainer/DeckList/Table/Data/ScrollContainer/MarginContainer/TableData");
        
        cardImg = GetNode<TextureRect>("VBoxContainer/HBoxContainer/CardViewer/VBoxContainer/Card/CardImg");
        cardImgSpell = GetNode<TextureRect>("VBoxContainer/HBoxContainer/CardViewer/VBoxContainer/Card/CardImgSpell");
        card = GetNode<TextureRect>("VBoxContainer/HBoxContainer/CardViewer/VBoxContainer/Card/Card");
        cardSpell = GetNode<TextureRect>("VBoxContainer/HBoxContainer/CardViewer/VBoxContainer/Card/CardSpell");
        
        foreach (CardInfo card in cardsJson)
        {
            CardTableLine instance = createCardTableLine(card, Table.CardTable);
            instance.Connect("SelectedCardChange", this, nameof(updateCardImage));
            CardListTable.AddChild(instance);
        }
    }

    private void loadJson()
    {
        string filePath = "res://Assets/data/cards.json";
        Godot.File jsonFile = new Godot.File();
        Error error = jsonFile.Open(filePath, Godot.File.ModeFlags.Read);
        if (error != Error.Ok)
        {
            GD.Print("Error loading the JSON file.");
            return;
        }

        string text = jsonFile.GetAsText();
        
        cardsJson = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CardInfo>>(text);
    }

    private void updateCardImage()
    {
        Console.WriteLine("Update");
        string id = CardTableLine.selectedCard.id;
        Texture texture = (Texture)ResourceLoader.Load("res://Assets/cards/" + id + ".jpg");

        bool isCardMinion = CardTableLine.selectedCard.type == "MINION";
        bool isCardSpell = CardTableLine.selectedCard.type == "SPELL";
        cardImg.Visible = isCardMinion;
        card.Visible = isCardMinion;
        cardImgSpell.Visible = isCardSpell;
        cardSpell.Visible = isCardSpell;
        
        TextureRect currentCard;
        if (isCardMinion) currentCard = cardImg;
        if (isCardSpell) currentCard = cardImgSpell;
        else currentCard = cardImg;
        
        ShaderMaterial material = (ShaderMaterial)currentCard.Material.Duplicate();
        material.SetShaderParam("texture1", texture);
        currentCard.Material = material;
    }

    private void onAddCardButtonpressed()
    {
        if (CardTableLine.selectedCard.table != Table.CardTable) return;
        CardInfo card = cardsJson.Find(c => c.id == CardTableLine.selectedCard.id);
        CardTableLine instance = createCardTableLine(card, Table.DeckTable);
        instance.Connect("SelectedCardChange", this, nameof(updateCardImage));
        DeckListTable.AddChild(instance);
    }
    
    private void onRemoveCardButtonpressed()
    {
        if (CardTableLine.selectedCard.table != Table.DeckTable) return;
        CardTableLine.selectedCard.QueueFree();
        CardTableLine.selectedCard = null;
    }

    private CardTableLine createCardTableLine(CardInfo card, Table table)
    {
        CardTableLine instance = (CardTableLine) PackedCardTableLine.Instance();
        Label nameLabel = instance.GetNode<Label>("HBoxContainer/Name");
        Label typeLabel = instance.GetNode<Label>("HBoxContainer/Type");
        Label costLabel = instance.GetNode<Label>("HBoxContainer/Cost");
        Label classLabel = instance.GetNode<Label>("HBoxContainer/Class");
        instance.id = card.id;
        nameLabel.Text = card.name;
        typeLabel.Text = card.type;
        instance.type = card.type;
        costLabel.Text = card.cost.ToString();
        classLabel.Text = card.cardClass;
        instance.table = table;
        return instance;
    }
}
