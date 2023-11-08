using Godot;
using System;

public class CardMinion : Control
{
    private TextureRect CardBack;
    private TextureRect CardImg;
    
    private Label nameLabel;
    private Control namePlace;
    
    private DynamicFont dynamicFont;
    
    private bool isInit = false;
    
    public override void _Ready()
    {
        CardImg = GetNode<TextureRect>("CardImg");
        CardBack = GetNode<TextureRect>("Card");
        nameLabel = GetNode<Label>("NameLabel");
        namePlace = GetNode<Control>("NamePlace");
        
        // Load a font from the filesystem
        dynamicFont = new DynamicFont();
        dynamicFont.FontData = ResourceLoader.Load<DynamicFontData>("res://Assets/font/BelweBoldBT.ttf");

        // Modify the font properties
        
        dynamicFont.OutlineSize = 2; // Set the outline size
        dynamicFont.OutlineColor = new Color(0,0,0); // Set the outline color
        dynamicFont.Size = 1; // Set the font size
        // Apply the modified font to the label
        nameLabel.AddFontOverride("font", dynamicFont);
        
        isInit = true;
        OnCardResized();
    }

    public void setImage(Texture texture)
    {
        ShaderMaterial material = (ShaderMaterial)CardImg.Material.Duplicate();
        material.SetShaderParam("texture1", texture);
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
        dynamicFont.Size = fontSize; 
    }
}
