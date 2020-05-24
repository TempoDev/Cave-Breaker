using Godot;
using System;

public class FarmScene : Node2D
{
	private Node generalData = null;

	private void _on_Back_pressed()
	{
		Godot.Collections.Array myTeam = (Godot.Collections.Array)generalData.Call("GetSlots");

		for (int i = 0; i < myTeam.Count; i++) {
			if (myTeam[i] != null)
			{
				generalData.Call("SwitchScene", general.Scene.MAP, null);
				return;
			}
		}
	}

	public override void _Ready()
	{
		generalData = GetNode<Node>("/root/GAME");
	}

	public override void _Process(float delta)
	{
		
	}
}
