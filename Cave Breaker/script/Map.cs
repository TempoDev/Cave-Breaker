using Godot;
using System;

public class LevelData : Node
{
	public bool random = false;
	public Godot.Collections.Array<Godot.Collections.Array<int>> map = null;
}

public class Map : Node2D
{

	/* =================================================================================================================== *
	 *                                                      VARIABLES                                                      *
	 * =================================================================================================================== */

	[Export] private int CELL_HIDDEN = 0;
	[Export] private int CELL_DISCOVERED = 2;
	[Export] private int CELL_AVAIBLE = 1;

	[Export] private Vector2 start = new Vector2(2, 3);
	[Export] private Godot.Collections.Array<int> goDown;
	[Export] private Godot.Collections.Array<int> goUp;
	[Export] private Godot.Collections.Array<int> goLeft;
	[Export] private Godot.Collections.Array<int> goRight;
	[Export] int width = 4, height = 20;
	[Export] int offsetY = 10;
	private Godot.Collections.Array<Godot.Collections.Array<MapDiscovered>> map = null;
	private Godot.Collections.Array<Godot.Collections.Array<LevelData>> level_list = null;

	private bool needUpdate = true;
	private Vector2 lastPressedPosition = new Vector2(-1, -1);

	private Node generalData = null;
	private TileMap hidden;
	private TileMap tilemap;

	/* =================================================================================================================== *
	 *                                                     LEVELS DATA                                                     *
	 * =================================================================================================================== */

	Godot.Collections.Array<Godot.Collections.Array<int>> SetBoard(int[][] newBoard)
	{
		if (newBoard.Length > 0)
		{
			Godot.Collections.Array<Godot.Collections.Array<int>> board = new Godot.Collections.Array<Godot.Collections.Array<int>>();
			for (int y = 0; y < newBoard.Length; y++)
			{
				board.Add(new Godot.Collections.Array<int>());
				for (int x = 0; x < newBoard[0].Length; x++)
				{
					board[y].Add(newBoard[y][x]);
				}
			}
			return board;
		}
		return null;
	}

	private void SetLevelData()
	{
		// 2;4 :: START
		level_list[4][2].map = SetBoard(new int[][]{
			new int[]{1}
		});

		// 5;4 :: 002
		level_list[5][2].map = SetBoard(new int[][]{
			new int[]{-1, 2, -1},
			new int[]{2, 1, 2}
		});

		// 6;4 :: 003
		level_list[6][2].map = SetBoard(new int[][]{
			new int[]{0, 0, 0},
			new int[]{0, 0, 0},
			new int[]{0, 0, 0}
		});
	}

	/* =================================================================================================================== *
	 *                                                  GENERATE MAP                                                       *
	 * =================================================================================================================== */

	Vector2 GetMapNodeFromPosition(Vector2 pos)
	{
		Vector2 nodeSize = tilemap.CellSize * tilemap.Scale;
		Vector2 id = new Vector2(-1, -1);
		Vector2 start = new Vector2(0, offsetY * nodeSize.y);
		Vector2 slidePos = pos - tilemap.Position + start;

		id = new Vector2(width, height) - (slidePos / nodeSize);
		id.x = slidePos.x / nodeSize.x;
		return id;
	}

	private Godot.Collections.Array<Godot.Collections.Array<MapDiscovered>> SetGeneralMap(Godot.Collections.Array arr)
	{
		if (arr == null)
			return null;

		Godot.Collections.Array<Godot.Collections.Array<MapDiscovered>> result = new Godot.Collections.Array<Godot.Collections.Array<MapDiscovered>>();

		for (int y = 0; y < arr.Count; y++)
		{
			result.Add(new Godot.Collections.Array<MapDiscovered>());
			Godot.Collections.Array sub = (Godot.Collections.Array)arr[y];
			for (int x = 0; x < sub.Count; x++)
			{
				MapDiscovered md = (MapDiscovered)sub[x];

				result[y].Add((MapDiscovered)sub[x]);
			}
		}

		return result;
	}

	private void GenerateMap()
	{
		Godot.Collections.Array arr = (Godot.Collections.Array)generalData.Call("GetMapData");
		Godot.Collections.Array<Godot.Collections.Array<MapDiscovered>> generalMap = SetGeneralMap(arr);

		map = new Godot.Collections.Array<Godot.Collections.Array<MapDiscovered>>();
		level_list = new Godot.Collections.Array<Godot.Collections.Array<LevelData>>();
		for (int y = 0; y < height; y++)
		{
			map.Add(new Godot.Collections.Array<MapDiscovered>());
			level_list.Add(new Godot.Collections.Array<LevelData>());
			for (int x = 0; x < width; x++)
			{
				map[y].Add(new MapDiscovered());
				map[y][x].score = -1;
				map[y][x].discovered = false;
				level_list[y].Add(new LevelData());
			}
		}

		if (generalMap != null)
		{
			for (int y = 0; y < generalMap.Count && y < map.Count; y++)
			{
				for (int x = 0; x < generalMap[0].Count && x < map[0].Count; x++)
				{
					map[y][x].score = generalMap[y][x].score;
					map[y][x].discovered = generalMap[y][x].discovered;

				}
			}
		}
		generalData.Call("SetMapData", map);
	}

	/* =================================================================================================================== *
	 *                                                        REDRAW                                                       *
	 * =================================================================================================================== */

	void ResizeMap()
	{
		Vector2 new_size = new Vector2();
		Vector2 screen_size = OS.WindowSize;
		Vector2 tile_size = tilemap.TileSet.TileGetRegion(0).Size;
		new_size = screen_size / new Vector2(width, height);
		new_size = new_size / tile_size;
		new_size.y = new_size.x;

		tilemap.Scale = new_size * 2;
		hidden.Scale = new_size * 2;
	}

	private void Redraw()
	{
		ResizeMap();
	}

	private void UpdateMapDisplay()
	{
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				int idToPos = -y + offsetY - 1;
				bool discovered = map[y][x].discovered;

				//discovered
				if (discovered)
				{
					hidden.SetCell(x, idToPos, CELL_DISCOVERED);
				} else
				{
					hidden.SetCell(x, idToPos, CELL_HIDDEN);
					if (x == (int)start.x && y == (int)start.y)
						hidden.SetCell(x, idToPos, CELL_AVAIBLE);
				}
				if (y + 1 < height && goDown.Contains(tilemap.GetCell(x, idToPos - 1))
					&& map[y + 1][x].discovered && !discovered)
					hidden.SetCell(x, idToPos, CELL_AVAIBLE);
				if (y - 1 >= 0 && goUp.Contains(tilemap.GetCell(x, idToPos + 1))
					&& map[y - 1][x].discovered && !discovered)
					hidden.SetCell(x, idToPos, CELL_AVAIBLE);
				if (x + 1 < width && goLeft.Contains(tilemap.GetCell(x + 1, idToPos))
					&& map[y][x + 1].discovered && !discovered)
					hidden.SetCell(x, idToPos, CELL_AVAIBLE);
				if (x - 1 >= 0 && goRight.Contains(tilemap.GetCell(x - 1, idToPos))
					&& map[y][x - 1].discovered && !discovered)
					hidden.SetCell(x, idToPos, CELL_AVAIBLE);

			}
		}
		needUpdate = false;
	}

	/* =================================================================================================================== *
	 *                                                   MAIN FUNCTIONS                                                    *
	 * =================================================================================================================== */

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventScreenDrag dragEvent)
		{
			Vector2 pos = dragEvent.Relative;

			tilemap.Position += new Vector2(tilemap.Position.x, pos.y);
			hidden.Position += new Vector2(hidden.Position.x, pos.y);

			if (tilemap.Position.y + pos.y < 0)
			{
				tilemap.Position = new Vector2(tilemap.Position.x, 0);
				hidden.Position = new Vector2(hidden.Position.x, 0);
			}

		}
		if (inputEvent is InputEventScreenTouch touchEvent)
		{
			if (touchEvent.Pressed)
			{
				lastPressedPosition = touchEvent.Position;
			} else {
				Vector2 pos = touchEvent.Position;
				Vector2 id = GetMapNodeFromPosition(pos);

				Vector2 testID = GetMapNodeFromPosition(lastPressedPosition);
				if ((int)id.x != (int)testID.x || (int)id.y != (int)testID.y)
					return;
				if ((int)id.x < 0 || (int)id.x >= width || (int)id.y < 0 || (int)id.y >= height)
					return;

				GD.Print(tilemap.GetCell((int)id.x, (int)(-id.y + offsetY)));
				if (map[(int)id.y][(int)id.x].discovered || hidden.GetCell((int)id.x, (int)(-id.y + offsetY)) == CELL_AVAIBLE)
				{
					Godot.Collections.Array data = new Godot.Collections.Array();

					data.Add(new Vector2(id));
					data.Add(level_list[(int)id.y][(int)id.x]);
					generalData.Call("SwitchScene", general.Scene.GAME, data);
				}
			}
		}
	}

	public override void _Ready()
	{
		generalData = GetNode<Node>("/root/GAME");
		tilemap = GetNode<TileMap>("map");
		hidden = GetNode<TileMap>("hide");
		GenerateMap();
		SetLevelData();
	}

	public override void _Process(float delta)
	{
		if (needUpdate)
			UpdateMapDisplay();
		Redraw();
	}
}
