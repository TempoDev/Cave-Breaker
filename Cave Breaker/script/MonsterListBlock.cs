using Godot;
using System;
using System.Dynamic;

public class MonsterListBlock : Node2D
{
	/* =================================================================================================================== *
	 *                                                     VARIABLE                                                        *
	 * =================================================================================================================== */

	//-----------MEMORY---------------
	private Vector2 MEM_SCREEN_SIZE;

	//-----------normal---------------
	private Godot.Collections.Array<Control> content = new Godot.Collections.Array<Control>();
	int width = 7;
	int height = 10;

	//-----------ENTITIES---------------
	[Export] PackedScene iconScene;

	//-----------COMPONENTS---------------
	private Node generalData = null;
	private TileMap background = null;
	private ScrollContainer container = null;
	private GridContainer grid = null;

	/* =================================================================================================================== *
	 *                                                       REDRAW                                                        *
	 * =================================================================================================================== */

	private void RedrawBackground()
	{
		//settings
		Vector2 bounds = new Vector2(0.9f, 0.7f);

		//reset background
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				background.SetCell(x, y, 7);
		for (int x = 0; x < width; x++)
		{
			background.SetCell(x, 0, 1);
			background.SetCell(x, height - 1, 12);
		}
		for (int y = 0; y < height; y++)
		{
			background.SetCell(0, y, 6);
			background.SetCell(width - 1, y, 8);
		}
		background.SetCell(0, 0, 0);
		background.SetCell(width - 1, 0, 2);
		background.SetCell(0, height - 1, 11);
		background.SetCell(width - 1, height - 1, 13);

		//set cell size
		Rect2 tileSize = background.TileSet.TileGetRegion(0);
		background.CellSize = new Vector2(tileSize.Size);

		Vector2 new_size = new Vector2();
		Vector2 objectBound = new Vector2(MEM_SCREEN_SIZE.x * bounds.x, MEM_SCREEN_SIZE.y * bounds.y);
		new_size = objectBound / new Vector2(width, height);
		new_size = new_size / tileSize.Size;

		if (new_size.x < new_size.y)
			new_size.y = new_size.x;
		else
			new_size.x = new_size.y;
		background.Scale = new_size;

		new_size = tileSize.Size * background.Scale;
		new_size *= new Vector2(width, height);
		Vector2 offset = new Vector2(0, 0);
		offset = (MEM_SCREEN_SIZE - new_size) / new Vector2(2, 2);
		this.Position = new Vector2(offset.x, MEM_SCREEN_SIZE.y - new_size.y - 10);
	}

	private void RedrawContent()
	{
		container.SetPosition(background.Position + (background.CellSize * background.Scale));
		container.RectSize = (background.CellSize * background.Scale) * new Vector2(width - 2, height - 2);

		grid.Columns = width - 2;

		//GD.Print("Rect Size: " + container.RectSize);
		for (int i = 0; i < content.Count; i++)
		{
			Control ct = content[i].GetChild<Control>(0);
			Vector2 newSize = (background.CellSize * background.Scale) / ct.RectSize;
			ct.RectScale = newSize;
			content[i].RectSize = (background.CellSize * background.Scale);
			content[i].RectMinSize = (background.CellSize * background.Scale);
		}
	}

	private void Redraw()
	{
		RedrawBackground();
		RedrawContent();
	}

	public void ButtonAction(int buttonId)
	{
		Godot.Collections.Array myTeam = (Godot.Collections.Array)generalData.Call("GetSlots");
		Godot.Collections.Array myMonsters = (Godot.Collections.Array)generalData.Call("GetMyMonsters");

		if (myMonsters == null)
			return;
		if ((bool)content[buttonId].Call("GetActive"))
		{
			int id = myTeam.IndexOf(myMonsters[buttonId]);

			myTeam[id] = null;
			content[buttonId].Call("Active");
		} else
		{
			for (int i = 0; i < myTeam.Count; i++)
			{
				if (myTeam[i] == null)
				{
					myTeam[i] = myMonsters[buttonId];
					content[buttonId].Call("Active");
					return;
				}
			}
		}
	}

	/* =================================================================================================================== *
	 *                                                        START                                                        *
	 * =================================================================================================================== */

	private void SetMemory()
	{
		MEM_SCREEN_SIZE = OS.WindowSize;
	}

	private void CreateMonsterNode(int buttonId)
	{
		Node node = iconScene.Instance();
		Control button = node.GetChild<Control>(0);
		node.RemoveChild(button);
		node.QueueFree();
		grid.AddChild(button);
		content.Add(button);
		BaseButton bb = button.GetNode<BaseButton>("button");
		bb.Connect("button_down", this, "ButtonAction", new Godot.Collections.Array() { buttonId });
	}

	private void CreateMonsterList()
	{
		Godot.Collections.Array myMonsters = (Godot.Collections.Array)generalData.Call("GetMyMonsters");

		GD.Print(myMonsters.Count);
		for (int i = 0; i < myMonsters.Count; i++)
		{
			CreateMonsterNode(i);
			OwnedMonster om = (OwnedMonster)myMonsters[i];
			content[i].Call("SetMonster", om);
		}
	}

	private void SetTeamMonsters()
	{
		Godot.Collections.Array myTeam = (Godot.Collections.Array)generalData.Call("GetSlots");
		Godot.Collections.Array myMonsters = (Godot.Collections.Array)generalData.Call("GetMyMonsters");

		for (int i = 0; i < myTeam.Count; i++)
		{
			if (myTeam[i] == null)
				continue;
			int id = myMonsters.IndexOf(myTeam[i]);
			content[id].Call("SetActive", true);
		}
	}

	private void GetComponents()
	{
		generalData = GetNode<Node>("/root/GAME");
		background = GetNode<TileMap>("background");
		container = GetNode<ScrollContainer>("box");
		grid = container.GetNode<GridContainer>("grid");
	}

	/* =================================================================================================================== *
	 *                                                   MAIN FUNCTIONS                                                    *
	 * =================================================================================================================== */

	public override void _Ready()
	{
		SetMemory();
		GetComponents();
		CreateMonsterList();
		SetTeamMonsters();
		Redraw();
	}

	public override void _Process(float delta)
	{
		Vector2 screenSize = OS.WindowSize;

		if ((int) MEM_SCREEN_SIZE.x != (int) screenSize.x || (int) MEM_SCREEN_SIZE.y != (int) screenSize.y)
		{
			MEM_SCREEN_SIZE = OS.WindowSize;
			Redraw();
		}
	}
}
