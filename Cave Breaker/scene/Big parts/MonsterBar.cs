using Godot;
using System;

/* =========================================================================================================== *
 *                                            MONSTER BAR CLASS                                                *
 *                                               To explain                                                    *
 *                                                                                                             *
 * =========================================================================================================== */

public class MonsterBar : Control
{
	/* =================================================================================================================== *
	 *                                                  VARIABLES                                                          *
	 * =================================================================================================================== */

	[Export] int distance = 10;
	[Export] PackedScene buttonScene;

	private Godot.Collections.Array<Control> bar = new Godot.Collections.Array<Control>();
	private Control buttonContainer;
	private int id = -1;
	private Node2D board;
	private Node generalData;

	/* =================================================================================================================== *
	 *                                                  FUNCTIONS                                                          *
	 * =================================================================================================================== */

	//----------------Utils------------------------//

	public int GetRemaining()
	{
		int remaining = 0;

		for (int i = 0; i < bar.Count; i++)
		{
			int nb = (int)bar[i].Call("GetRemaining");
			if (nb != -1)
				remaining += nb;
		}
		return remaining;
	}

	//----------------Button Action----------------//

	public void ButtonAction(int buttonId)
	{
		OwnedMonster m = (OwnedMonster)bar[buttonId].Call("GetMonster");
		if (m == null || m.dataBase.isPassive())
			return;
		if (id != -1)
			bar[id].Call("SetActive", false);
		if (id == buttonId)
			id = -1;
		else
		{
			id = buttonId;
			bar[id].Call("SetActive", true);
		}
	}

	//----------------Create bar----------------//

	public void CreateButton(int buttonId)
	{
		Node node = buttonScene.Instance();
		Control button = node.GetChild<Control>(0);
		node.RemoveChild(button);
		node.QueueFree();
		buttonContainer.AddChild(button);
		bar.Add(button);
		BaseButton bb = button.GetNode<BaseButton>("button");
		bb.Connect("button_down", this, "ButtonAction", new Godot.Collections.Array() { buttonId });
		Redraw();
	}

	private void CreateBar(int len)
	{
		for (int i = 0; i < len; i++)
		{
			CreateButton(i);
		}
	}

	//-----------------Draw--------------------------//

	private void Redraw()
	{
		if (bar == null || bar.Count <= 0)
			return;
		double step = (2 * Math.PI) / bar.Count;
		for (int i = 0; i < bar.Count; i++)
		{
			Control button = bar[i];
			Vector2 rotation = new Vector2((float)Math.Sin(step * i), (float)Math.Cos(step * i));
			button.RectPosition = new Vector2(distance, distance) * rotation;
		}
	}

	//-----------------Manage player input--------------------------//
	public override void _Input(InputEvent inputEvent)
	{
		if (id < 0)
			return;
		OwnedMonster m = (OwnedMonster)bar[id].Call("GetMonster");

		if (inputEvent is InputEventScreenTouch touchEvent)
		{
			if (touchEvent.Pressed)
			{
				
			}
			else
			{
				Vector2 pos = (Vector2)board.Call("GetIdFromPos", touchEvent.Position);
				if (pos.x < 0 || pos.y < 0)
					return;
				m.dataBase.isClicked((int)pos.x, (int)pos.y);
			}
		}
	}

	//-----------------Test--------------------------//

	/* =================================================================================================================== *
	 *                                                   MAIN FUNCTIONS                                                    *
	 * =================================================================================================================== */

	public override void _Ready()
	{
		generalData = GetNode<Node>("/root/GAME");
		board = GetNode<Node2D>("../../gameboard/board");
		buttonContainer = GetNode<Control>("buttons");
		int slotNb = (int)generalData.Call("GetSlotsNb");
		CreateBar(slotNb);
		for (int i = 0; i < slotNb; i++)
			bar[i].Call("SetMonster", (OwnedMonster)generalData.Call("GetSlotsAt", i));
	}

	public override void _Process(float delta)
	{
		for (int i = 0; i < bar.Count; i++)
		{
			OwnedMonster m = (OwnedMonster)bar[i].Call("GetMonster");
			int remaining = (int)bar[i].Call("GetRemaining");
			if (m != null && (m.dataBase.isPassive() || id == i) && (remaining == -1 || remaining > 0))
			{
				if (m.dataBase.Attack(board))
					bar[i].Call("RemoveRemaining", 1);
			}
		}
	}
}
