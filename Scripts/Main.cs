using Godot;
using System;

public class Main : Control
{
    public override void _Ready()
    {
        OS.MinWindowSize = new Vector2(1024,600);
    }


}
