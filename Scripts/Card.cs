using Godot;
using System;
using System.Collections.Generic;
using SabberStoneCore.Enums;

public partial class Card : Control
{
    private class LabelHandler
    {
        public enum Type
        {
            MINION,
            SPELL,
            BOTH
        };

        public LabelHandler(Label _label, Control _place, DynamicFont _font, float _fontSize, Type _type)
        {
            label = _label;
            place = _place;
            font = _font;
            fontSize = _fontSize;
            type = _type;
        }
        
        public Label label { get; set; }
        public Control place { get; set; }
        public DynamicFont font { get; set; }
        public float fontSize { get; set; }
        public Type type { get; set; }
    }

    private Dictionary<string, LabelHandler> labelDict = new Dictionary<string, LabelHandler>();

    // First 4 are neutral and 4 next are mage
    // In this order : minion, minion with race, spell, spell with school
    private List<Texture> CardBorderImgs = new List<Texture>()
    {
        ResourceLoader.Load<Texture>("res://Assets/sprite/inhand_minion_neutral_upscayl.png"),
        ResourceLoader.Load<Texture>("res://Assets/sprite/inhand_minion_neutral_withrace_upscayl.png"),
        ResourceLoader.Load<Texture>("res://Assets/sprite/inhand_spell_neutral_upscayl.png"),
        ResourceLoader.Load<Texture>("res://Assets/sprite/inhand_spell_neutral_withschool_upscayl.png"),
        ResourceLoader.Load<Texture>("res://Assets/sprite/inhand_minion_mage_upscayl.png"),
        ResourceLoader.Load<Texture>("res://Assets/sprite/inhand_minion_mage_withrace_upscayl.png"),
        ResourceLoader.Load<Texture>("res://Assets/sprite/inhand_spell_mage_upscayl.png"),
        ResourceLoader.Load<Texture>("res://Assets/sprite/inhand_spell_mage_withschool_upscayl.png"),
    };

    private Texture MinionCardMask = ResourceLoader.Load<Texture>("res://Assets/mask/minion_mask.png");
    private Texture SpellCardMask = ResourceLoader.Load<Texture>("res://Assets/mask/spell_mask.png");

    private Control cardTemplate;
    private TextureRect CardFrame;
    private Texture currentMask;
    private TextureRect CardImg;
    
    private bool isInit = false;

    private DynamicFont BelweFont = new DynamicFont();
    private DynamicFont LibreFranklinRegular = new DynamicFont();
    private DynamicFont LibreFranklinBold = new DynamicFont();

    private RichTextLabel TextLabel;
    private Control TextPlace;
    
    public override void _Ready()
    {
        InitFont();
        
        cardTemplate = GetNode<Control>("CardTemplate");
        CardFrame = GetNode<TextureRect>("CardTemplate/CardFrame");
        CardImg = GetNode<TextureRect>("CardTemplate/CardImg");
        TextLabel = GetNode<RichTextLabel>("CardTemplate/TextLabel");
        TextPlace = createLabelPlace(TextLabel);
        cardTemplate.AddChild(TextPlace);
        
        TextLabel.AddFontOverride("normal_font", LibreFranklinRegular);
        TextLabel.AddFontOverride("bold_font", LibreFranklinBold);
        
        addNewCardToDict("CardTemplate/NameLabel", "name", BelweFont, 1.0f, LabelHandler.Type.BOTH);
        addNewCardToDict("CardTemplate/ManaLabel", "mana", BelweFont, 1.0f, LabelHandler.Type.MINION);
        addNewCardToDict("CardTemplate/ManaLabelSpell", "manaSpell", BelweFont, 1.0f, LabelHandler.Type.SPELL);
        addNewCardToDict("CardTemplate/AttackLabel", "attack", BelweFont, 1.0f, LabelHandler.Type.MINION);
        addNewCardToDict("CardTemplate/HealthLabel", "health", BelweFont, 1.0f, LabelHandler.Type.MINION);
        addNewCardToDict("CardTemplate/RaceLabel", "race", BelweFont, 1.0f, LabelHandler.Type.MINION);
        addNewCardToDict("CardTemplate/SchoolLabel", "school", BelweFont, 1.0f, LabelHandler.Type.SPELL);

        TextLabel.BbcodeText = "[b]Battlecry:[/b] Deal 1 damage";
        
        isInit = true;
        OnCardResized();
    }

    private void InitFont()
    {
        BelweFont.FontData = ResourceLoader.Load<DynamicFontData>("res://Assets/font/BelweBoldBT.ttf");
        BelweFont.OutlineSize = 2; 
        BelweFont.OutlineColor = new Color(0,0,0); 
        BelweFont.Size = 1;
        
        LibreFranklinRegular.FontData = ResourceLoader.Load<DynamicFontData>("res://Assets/font/LibreFranklin-Regular.ttf");
        LibreFranklinRegular.Size = 1;
        
        LibreFranklinBold.FontData = ResourceLoader.Load<DynamicFontData>("res://Assets/font/LibreFranklin-Bold.ttf");
        LibreFranklinBold.Size = 1;
    }

    private DynamicFont GetCopy(DynamicFont _font)
    {
        DynamicFont res = new DynamicFont();
        res.FontData = _font.FontData;
        res.OutlineSize = _font.OutlineSize;
        res.OutlineColor = _font.OutlineColor;
        res.Size = _font.Size;
        return res;
    }

    private void addNewCardToDict(string nodePath, string key, DynamicFont _font, float fontSize, LabelHandler.Type type)
    {
        Label label = GetNode<Label>(nodePath);
        Control place = createLabelPlace(label);
        cardTemplate.AddChild(place);
        DynamicFont font = GetCopy(_font);
        label.AddFontOverride("font", font);
        labelDict.Add(key, new LabelHandler(label, place, font, fontSize, type));
    }

    public void setCardInfo(CardInfo cardInfo)
    {
        labelDict["race"].label.Text = cardInfo.race;
        labelDict["school"].label.Text = cardInfo.spellSchool;
        
        int baseIndex = -1;
        if (cardInfo.cardClass == "NEUTRAL")
            baseIndex = 0;
        else if (cardInfo.cardClass == "MAGE")
            baseIndex = 4;
        
        if (cardInfo.GetType() == CardType.MINION)
        {
            if (!cardInfo.race.Empty()) baseIndex++;
            currentMask = MinionCardMask;
        }
        else if (cardInfo.GetType() == CardType.SPELL)
        {
            baseIndex = baseIndex + 2;
            if (!cardInfo.spellSchool.Empty()) baseIndex++;
            currentMask = SpellCardMask;
        }

        CardFrame.Texture = CardBorderImgs[baseIndex];
        
        foreach (LabelHandler labelHandler in labelDict.Values)
        {
            labelHandler.label.Visible = false;
            
            if (labelHandler.type == LabelHandler.Type.BOTH)
            {
                labelHandler.label.Visible = true;
                continue;
            }
            
            if (cardInfo.GetType() == CardType.MINION && labelHandler.type == LabelHandler.Type.MINION)
                labelHandler.label.Visible = true;
            if (cardInfo.GetType() == CardType.SPELL && labelHandler.type == LabelHandler.Type.SPELL)
                labelHandler.label.Visible = true;
        }
        
        labelDict["name"].label.Text = cardInfo.name;
        labelDict["mana"].label.Text = cardInfo.cost.ToString();
        labelDict["manaSpell"].label.Text = cardInfo.cost.ToString();
        labelDict["attack"].label.Text = cardInfo.attack.ToString();
        labelDict["health"].label.Text = cardInfo.health.ToString();
        TextLabel.BbcodeText = "[center]"+cardInfo.text+"[/center]";
    }

    public void setImage(Texture texture)
    {
        ShaderMaterial material = (ShaderMaterial)CardImg.Material.Duplicate();
        material.SetShaderParam("texture1", texture);
        material.SetShaderParam("texture2", currentMask);
        CardImg.Material = material;
    }
    
    public void OnCardResized()
    {
        if (!isInit) return;
        foreach (LabelHandler labelHandler in labelDict.Values)
        {
            labelHandler.font.Size = (int) Math.Floor(labelHandler.place.RectSize.y  * 72 / 96 * labelHandler.fontSize);
        }
        LibreFranklinRegular.Size = (int) Math.Floor(TextPlace.RectSize.y  * 72 / 96);
        LibreFranklinBold.Size = (int) Math.Floor(TextPlace.RectSize.y  * 72 / 96);
    }

    private Control createLabelPlace(Control label)
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
    
}
