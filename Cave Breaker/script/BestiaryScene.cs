using Godot;
using System;

public class BestiaryScene : Node2D
{

	private Node generalData = null;

	private void _on_Back_pressed()
	{
		generalData.Call("SwitchScene", general.Scene.MAP, null);
	}

	public override void _Ready()
	{
		generalData = GetNode<Node>("/root/GAME");
	}

	public override void _Process(float delta)
	{
		
	}
}
