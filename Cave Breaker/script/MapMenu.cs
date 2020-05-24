using Godot;
using System;

public class MapMenu : Node2D
{
	/* =================================================================================================================== *
	 *                                                      VARIABLES                                                      *
	 * =================================================================================================================== */

	//components
	private Node generalData = null;
	private Sprite backMenu = null;

	private Vector2 lastPressedPosition = new Vector2(-1, -1);
	private bool status = false;
	private bool draging = false;

	/* =================================================================================================================== *
	 *                                                      FUNCTIONS                                                      *
	 * =================================================================================================================== */
	private void RecenterMenu(float delta)
	{
		Vector2 screen_size = OS.WindowSize;

		if (draging)
			return;

		if (status)
		{
			if (this.Position.y > 0)
				this.Position = this.Position -  new Vector2(0, delta * 3000);
		} else
		{
			if (this.Position.y < screen_size.y * 0.98f)
				this.Position = this.Position + new Vector2(0, delta * 3000);
		}
	}

	private void ResizeBackMenu()
	{
		Vector2 screen_size = OS.WindowSize;

		backMenu.Scale = screen_size * 1.1f;
	}

	private void Redraw()
	{
		ResizeBackMenu();
	}

	/* =================================================================================================================== *
	 *                                                       BUTTONS                                                       *
	 * =================================================================================================================== */

	private void _on_hatchery_pressed()
	{
		generalData.Call("SwitchScene", general.Scene.HATCHERY, null);
	}

	private void _on_farm_pressed()
	{
		generalData.Call("SwitchScene", general.Scene.FARM, null);
	}

	private void _on_bestiary_pressed()
	{
		generalData.Call("SwitchScene", general.Scene.BESTIARY_BOOK, null);
	}

	private void _on_store_pressed()
	{
		//generalData.Call("SwitchScene", general.Scene.STORE, null);
	}

	private void _on_backpack_pressed()
	{
		//generalData.Call("SwitchScene", general.Scene.INVENTORY, null);
	}

	private void _on_back_pressed()
	{
		status = false;
	}

	/* =================================================================================================================== *
	 *                                                   MAIN FUNCTIONS                                                    *
	 * =================================================================================================================== */

	public override void _Input(InputEvent inputEvent)
	{
		Vector2 screen_size = OS.WindowSize;

		if (inputEvent is InputEventScreenDrag dragEvent)
		{
			Vector2 pos = dragEvent.Position;

			if (lastPressedPosition.y > screen_size.y * 0.9f)
			{
				draging = true;
				this.Position = new Vector2(0, pos.y);
			}
		}
		if (inputEvent is InputEventScreenTouch touchEvent)
		{
			if (touchEvent.Pressed)
			{
				lastPressedPosition = touchEvent.Position;
				GD.Print(lastPressedPosition);
			}
			else
			{
				draging = false;
				Vector2 pos = touchEvent.Position;

				if (pos.y > screen_size.y * 0.9f && lastPressedPosition.y > 0.9f)
				{
					status = !status;
				}
			}
		}
	}

	public override void _Ready()
	{
		Vector2 screen_size = OS.WindowSize;

		generalData = GetNode<Node>("/root/GAME");
		backMenu = GetNode<Sprite>("menu");

		backMenu.Position = new Vector2(0, 0);
		this.Position = new Vector2(0, screen_size.y);
		Redraw();
	}

	public override void _Process(float delta)
	{
		RecenterMenu(delta);
	}
}
