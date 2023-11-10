using Godot;
using System;
using SabberStoneCore.Enums;

public partial class Card : Control
{
    private class LabelHandler
    {
        public Label label { get; set; }
        public Control place { get; set; }
        public DynamicFont font { get; set; }
    }
    
    [Export] private Texture MinionCardImg;
    [Export] private Texture SpellCardImg;

    [Export] private Texture MinionCardMask;
    [Export] private Texture SpellCardMask;

    private Control cardTemplate;
    private TextureRect CardFrame;
    private Texture currentMask;
    private TextureRect CardImg;
    
    private Label nameLabel;
    private Control namePlace;
    private DynamicFont nameFont = new DynamicFont();

    private Label manaLabel;
    private Control manaPlace;
    private DynamicFont manaFont = new DynamicFont();
    
    private bool isInit = false;
    
    public override void _Ready()
    {
        cardTemplate = GetNode<Control>("CardTemplate");
        CardFrame = GetNode<TextureRect>("CardTemplate/CardFrame");
        CardImg = GetNode<TextureRect>("CardTemplate/CardImg");
        
        nameLabel = GetNode<Label>("CardTemplate/NameLabel");
        namePlace = createLabelPlace(nameLabel);
        cardTemplate.AddChild(namePlace);
        InitFont(nameFont, nameLabel);

        manaLabel = GetNode<Label>("CardTemplate/ManaLabel");
        manaPlace = createLabelPlace(manaLabel);
        cardTemplate.AddChild(manaPlace);
        InitFont(manaFont, manaLabel);
        
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

    public void setCost(string mana)
    {
        manaLabel.Text = mana;
    }

    public void OnCardResized()
    {
        if (!isInit) return;
        nameFont.Size = (int) Math.Floor(namePlace.RectSize.y  * 72 / 96);
        manaFont.Size = (int) Math.Floor(manaPlace.RectSize.y  * 72 / 96);
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

        return place;
    }

    private void InitFont(DynamicFont font, Label label)
    {
        // Load a font from the filesystem
        font.FontData = ResourceLoader.Load<DynamicFontData>("res://Assets/font/BelweBoldBT.ttf");
        
        // Modify the font properties
        font.OutlineSize = 2; // Set the outline size
        font.OutlineColor = new Color(0,0,0); // Set the outline color
        font.Size = 1; // Set the font size
        // Apply the modified font to the label
        label.AddFontOverride("font", font);
    }
}
