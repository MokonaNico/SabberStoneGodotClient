using Godot;
using System;

public class CardSpell : Control
{
    private TextureRect CardImg;
    
    public override void _Ready()
    {
        CardImg = GetNode<TextureRect>("CardImg");
    }

    public void setImage(Texture texture)
    {
        ShaderMaterial material = (ShaderMaterial)CardImg.Material.Duplicate();
        material.SetShaderParam("texture1", texture);
        CardImg.Material = material;
    }
}
