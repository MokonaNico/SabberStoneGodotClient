using Godot;
using System;
using SabberStoneCore.Enums;

public partial class Card : Control
{
    [Export] private Texture MinionCardImg;
    [Export] private Texture SpellCardImg;

    [Export] private Texture MinionCardMask;
    [Export] private Texture SpellCardMask;
    
    private TextureRect CardFrame;
    private Texture currentMask;
    private TextureRect CardImg;
    
    private Label nameLabel;
    private Control namePlace;

    private DynamicFont nameFont;
    
    private bool isInit = false;
    
    public override void _Ready()
    {
        CardFrame = GetNode<TextureRect>("CardFrame");
        CardImg = GetNode<TextureRect>("CardImg");
        nameLabel = GetNode<Label>("NameLabel");
        namePlace = createLabelPlace(nameLabel);
        
        // Load a font from the filesystem
        nameFont = new DynamicFont();
        nameFont.FontData = ResourceLoader.Load<DynamicFontData>("res://Assets/font/BelweBoldBT.ttf");

        // Modify the font properties
        
        nameFont.OutlineSize = 2; // Set the outline size
        nameFont.OutlineColor = new Color(0,0,0); // Set the outline color
        nameFont.Size = 1; // Set the font size
        // Apply the modified font to the label
        nameLabel.AddFontOverride("font", nameFont);
        
        isInit = true;
        OnCardResized();
    }

    public void setType(CardType type)
    {
        if (type == CardType.MINION)
        {
            CardFrame.Texture = MinionCardImg;
            currentMask = MinionCardMask;
        }
        else if (type == CardType.SPELL)
        {
            CardFrame.Texture = SpellCardImg;
            currentMask = SpellCardMask;
        }
    }

    public void setImage(Texture texture)
    {
        ShaderMaterial material = (ShaderMaterial)CardImg.Material.Duplicate();
        material.SetShaderParam("texture1", texture);
        material.SetShaderParam("texture2", currentMask);
        CardImg.Material = material;
    }

    public void setName(string name)
    {
        nameLabel.Text = name;
    }

    public void OnCardResized()
    {
        if (!isInit) return;
        int fontSize = (int) Math.Floor(namePlace.RectSize.y  * 72 / 96);
        nameFont.Size = fontSize; 
    }

    private Control createLabelPlace(Label label)
    {
        Control place = new Control();

        place.RectPosition = label.RectPosition;
        place.RectSize = label.RectSize;
        place.AnchorBottom = label.AnchorBottom;
        place.AnchorLeft = label.AnchorLeft;
        place.AnchorRight = label.AnchorRight;
        place.AnchorTop = label.AnchorTop;

        place.MarginBottom = label.MarginBottom;
        place.MarginLeft = label.MarginLeft;
        place.MarginRight = label.MarginRight;
        place.MarginTop = label.MarginTop;
        place.Connect("resized",this,"OnCardResized");
        
        AddChild(place);
        return place;
    }
}
