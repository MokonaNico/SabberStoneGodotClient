using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;

public class CardInfo
{
    public string id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public int cost { get; set; }
    public string cardClass { get; set; }
    public int attack { get; set; }
    public int health { get; set; }
    public string text { get; set; }
    public string race { get; set; }
    public string spellSchool { get; set; }
    
    public CardType GetType()
    {
        if (type == "MINION") return CardType.MINION;
        if (type == "SPELL") return CardType.SPELL;
        return CardType.INVALID;
    }
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
    private Dictionary<string, CardInfo> cardsDict = new Dictionary<string, CardInfo>();
    private Card card;

    private OptionButton filterCardList;
    private OptionButton sortbyCardList;
    private OptionButton filterDeckList;
    private OptionButton sortbyDeckList;

    private LineEdit deckNameLine;
    
    private OptionButton loadDeckList;

    private Label cardCountLabel;
    private int cardCount = 0;
    
    public override void _Ready()
    {
        loadJson();
        
        PackedCardTableLine = (PackedScene)ResourceLoader.Load("res://Scenes/CardTableLine.tscn");
        
        CardListTable = GetNode<VBoxContainer>("VBoxContainer/HBoxContainer/CardList/Table/Data/ScrollContainer/MarginContainer/TableData");
        DeckListTable = GetNode<VBoxContainer>("VBoxContainer/HBoxContainer/DeckList/Table/Data/ScrollContainer/MarginContainer/TableData");
        card = GetNode<Card>("VBoxContainer/HBoxContainer/CardViewer/VBoxContainer/Card");
        cardCountLabel = GetNode<Label>("VBoxContainer/Panel/CardCountLabel");

        filterCardList = GetNode<OptionButton>("VBoxContainer/HBoxContainer/CardViewer/VBoxContainer/OptionPanel/HBoxContainer/CardListOption/FilterCardList");
        sortbyCardList = GetNode<OptionButton>("VBoxContainer/HBoxContainer/CardViewer/VBoxContainer/OptionPanel/HBoxContainer/CardListOption/SortByCardList");
        filterDeckList = GetNode<OptionButton>("VBoxContainer/HBoxContainer/CardViewer/VBoxContainer/OptionPanel/HBoxContainer/DeckListOption/FilterDeckList");
        sortbyDeckList = GetNode<OptionButton>("VBoxContainer/HBoxContainer/CardViewer/VBoxContainer/OptionPanel/HBoxContainer/DeckListOption/SortByDeckList");

        deckNameLine = GetNode<LineEdit>("VBoxContainer/Panel/DeckNameLineEdit");

        loadDeckList = GetNode<OptionButton>("VBoxContainer/Panel/LoadDeckList");

        bool firstCard = true;
        foreach (CardInfo card in cardsJson)
        {
            cardsDict.Add(card.id, card);
            CardTableLine instance = createCardTableLine(card, Table.CardTable);
            instance.Connect("SelectedCardChange", this, nameof(updateCardImage));
            CardListTable.AddChild(instance);

            if (firstCard)
            {
                CardTableLine.selectedCard = instance;
                firstCard = false;
            }
        }
        
        updateLoadDeckList();
        updateSortAndFilter();
        updateCardImage();
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
        string id = CardTableLine.selectedCard.id;

        CardInfo cardInfo = cardsJson.Where(i => i.id == id).First();

        string raceLowerCase = "";
        if (!cardInfo.race.Empty()) 
            raceLowerCase = cardInfo.race.Substring(0, 1) + cardInfo.race.Substring(1).ToLower();
        card.setCardInfo(cardInfo);
        
        card.setImage(CardTableLine.selectedCard.image);
    }

    private void onAddCardButtonpressed()
    {
        if (CardTableLine.selectedCard == null || CardTableLine.selectedCard.table != Table.CardTable) return;
        addCard(CardTableLine.selectedCard.id);
    }

    private void addCard(string id)
    {
        CardInfo card = cardsJson.Find(c => c.id == id);
        CardTableLine instance = createCardTableLine(card, Table.DeckTable);
        instance.Connect("SelectedCardChange", this, nameof(updateCardImage));
        DeckListTable.AddChild(instance);
        cardCount++;
        updateCardCount();
        updateSortAndFilter();
    }
    
    private void onRemoveCardButtonpressed()
    {
        if (CardTableLine.selectedCard == null || CardTableLine.selectedCard.table != Table.DeckTable) return;
        CardTableLine.selectedCard.QueueFree();
        CardTableLine.selectedCard = null;
        cardCount--;
        updateCardCount();
        updateSortAndFilter();
    }

    private void removeAllCards()
    {
        if (CardTableLine.selectedCard != null && CardTableLine.selectedCard.table == Table.DeckTable)
            CardTableLine.selectedCard = null;
        foreach (Node child in DeckListTable.GetChildren())
            child.QueueFree();
        cardCount = 0;
        updateCardCount();
        updateSortAndFilter();
    }

    private void updateCardCount()
    {
        cardCountLabel.Text = cardCount.ToString();
    }

    private CardTableLine createCardTableLine(CardInfo card, Table table)
    {
        CardTableLine instance = (CardTableLine) PackedCardTableLine.Instance();
        Label nameLabel = instance.GetNode<Label>("HBoxContainer/Name");
        Label typeLabel = instance.GetNode<Label>("HBoxContainer/Type");
        Label costLabel = instance.GetNode<Label>("HBoxContainer/Cost");
        Label classLabel = instance.GetNode<Label>("HBoxContainer/Class");
        nameLabel.Text = card.name;
        typeLabel.Text = card.type;
        costLabel.Text = card.cost.ToString();
        classLabel.Text = card.cardClass;
        
        instance.id = card.id;
        
        instance.table = table;
        instance.image = (Texture)ResourceLoader.Load("res://Assets/cards/" + card.id + ".jpg");
        return instance;
    }
    
    private void onFilterCardListItemSelected(int index)
    {
        filterCards(CardListTable, index);
    }
    
    private void onSortByCardListItemSelected(int index)
    {
        sortCards(CardListTable, index);
    }
    
    private void onFilterDeckListItemSelected(int index)
    {
        filterCards(DeckListTable, index);
    }
    
    private void onSortByDeckListItemSelected(int index)
    {
        sortCards(DeckListTable, index);
    }

    private void sortCards(VBoxContainer listTable, int index)
    {
        Godot.Collections.Array children = listTable.GetChildren();
        
        List<CardTableLine> childrenList = new List<CardTableLine>();
        foreach (CardTableLine child in children)
            childrenList.Add(child);
        
        childrenList.Sort((a, b) =>
        {
            int cmp = 0;
            if (index == 0)
                cmp = cardsDict[a.id].name.CompareTo(cardsDict[b.id].name);
            else if (index == 1)
                cmp = cardsDict[a.id].type.CompareTo(cardsDict[b.id].type);
            else if (index == 2)
                cmp = cardsDict[a.id].cost.CompareTo(cardsDict[b.id].cost);
            else if (index == 3)
                cmp = cardsDict[a.id].cardClass.CompareTo(cardsDict[b.id].cardClass);

            if (cmp != 0) return cmp; 
            return cardsDict[a.id].name.CompareTo(cardsDict[b.id].name);
        });
        
        foreach (Node child in children)
            listTable.RemoveChild(child);
        
        foreach (Node child in childrenList)
            listTable.AddChild(child);
    }

    private void filterCards(VBoxContainer listTable, int index)
    {
        foreach (CardTableLine child in listTable.GetChildren())
        {
            child.Visible = false;
            
            // Filter is none so we make the card visible
            if (index == 0)
                child.Visible = true;
            else if (index == 1 && cardsDict[child.id].cardClass == "NEUTRAL")
                child.Visible = true;
            else if (index == 2 && cardsDict[child.id].cardClass == "MAGE")
                child.Visible = true;
        }
    }

    private void updateSortAndFilter()
    {
        sortCards(CardListTable, sortbyCardList.Selected);
        filterCards(CardListTable, filterCardList.Selected);
        sortCards(DeckListTable, sortbyDeckList.Selected);
        filterCards(DeckListTable, filterDeckList.Selected);
    }
    
    private void onSaveDeckButtonPressed()
    {
        string deckName = deckNameLine.Text;

        // TODO : error message if deck name is invalid
        if (deckName.Empty()) return;

        List<string> listIds = new List<string>();
        foreach (CardTableLine child in DeckListTable.GetChildren())
        {
            listIds.Add(child.id);
        }

        SaveHandler.saveDeck(listIds, deckName);
        updateLoadDeckList();
    }

    private void updateLoadDeckList()
    {
        loadDeckList.Clear();

        foreach (string deckName in SaveHandler.getListDeckNames())
        {
            loadDeckList.AddItem(deckName);
        }
    }
    
    private void onLoadDeckButtonPressed()
    {
        removeAllCards();
        
        string deckName = (string) loadDeckList.Text;

        if (deckName == null || deckName == "") return;
        
        List<string> cardIds = SaveHandler.getCardsFromDeck(deckName);

        foreach (string cardId in cardIds)
            addCard(cardId);
    }
    
    private void onDeleteDeckButtonPressed()
    {
        removeAllCards();
        
        string deckName = (string) loadDeckList.Text;
        if (deckName == null || deckName == "") return;
        
        SaveHandler.removeDeck(deckName);
        updateLoadDeckList();
    }
    
    private void onClearDeckButtonPressed()
    {
        removeAllCards();
    }
}
