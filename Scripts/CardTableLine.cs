using Godot;
using System;
using SabberStoneCore.Enums;

public class CardTableLine : ColorRect
{
    [Signal]
    public delegate void SelectedCardChange();
    
    public static CardTableLine selectedCard;
    public string id;
    public string name;
    public CardType type;
    public Table table;
    public Texture image;
    
    private bool MouseOver = false;

    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == (int)ButtonList.Left && mouseEvent.Pressed)
        {
            if (MouseOver)
            {
                Color = new Color(0.3f,0.7f,1.0f,0.3f);
                if (selectedCard != null && selectedCard != this)
                {
                    selectedCard.Color = new Color(0,0,0,0);
                }
                selectedCard = this;

                EmitSignal(nameof(SelectedCardChange));
            }
        }
    }

    public void OnControlMouseEntered()
    {
        MouseOver = true;
    }
    
    public void OnControlMouseExited()
    {
        MouseOver = false;
    }
}
