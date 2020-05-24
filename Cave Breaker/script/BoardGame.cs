using Godot;
using System;


/* =========================================================================================================== *
 *                                                BOARD CLASS                                                  *
 *                                   Contain the board that can be passed                                      *
 *                                       to initialize the board game                                          *
 * =========================================================================================================== */

public class Board
{
	public int width;
	public int height;
	public Godot.Collections.Array<Godot.Collections.Array<int>> map;

	public bool IsInMap(int x, int y)
	{
		if (x < 0)
			return false;
		if (x >= width)
			return false;
		if (y < 0)
			return false;
		if (y >= height)
			return false;
		return true;
	}

	public bool IsMap(int x, int y)
	{
		if (!IsInMap(x, y))
			return false;
		if (map[y][x] >= 0)
			return true;
		return false;
	}

	public int GetEgg(int x, int y)
	{
		if (!IsInMap(x, y))
			return -1;
		if (map[y][x] >= 0)
			return map[y][x];
		return -1;
	}
}

/* =========================================================================================================== *
 *                                             BOARD GAME CLASS                                                *
 *                                   Contain the board game, display it                                        *
 *                                              update it, etc                                                 *
 * =========================================================================================================== */

public class BoardGame : Node2D
{
	/* =================================================================================================================== *
	 *                                                  VARIABLES                                                          *
	 * =================================================================================================================== */

	//enums
	public enum Around {
		UP = 0, LEFT = 1, RIGHT = 2, DOWN = 3,
		UPPER_LEFT = 4, UPPER_RIGHT = 5, BOTTOM_LEFT = 6, BOTTOM_RIGHT = 7
	};

	private enum Tile {
		NONE = -1, CENTER = 7,
		LEFT = 6, RIGHT = 8, DOWN = 12, UP = 1,
		UPPER_LEFT = 0, UPPER_RIGHT = 2, BOTTOM_LEFT = 11, BOTTOM_RIGHT = 13,
		INSIDE_UPPER_LEFT = 3, INSIDE_UPPER_RIGHT = 4, INSIDE_BOTTOM_LEFT = 9, INSIDE_BOTTOM_RIGHT = 10
	};

	//Serialized fields
	[Export] private Godot.Collections.Array<TileSet> tilesets;
	[Export] private int tilesetID = 0;
	[Export] private PackedScene eggSampleScene;

	//components
	private TileMap tm;
	private TileSet ts;

	//private variables
	private Board board = new Board();
	private Godot.Collections.Array<Godot.Collections.Array<Node2D>> eggList;
	private Node2D eggFolder;
	private bool wait;
	private Node generalData;
	private bool secondStart = false;

	/* =================================================================================================================== *
	 *                                                  FUNCTIONS                                                          *
	 * =================================================================================================================== */

	// ---------------------CHANGE BOARD------------------------------//

	void ChangeBoard(Board newBoard)
	{
		board = newBoard;
		Redraw();
	}

	void ChangeBoard(Godot.Collections.Array<Godot.Collections.Array<int>> newBoard)
	{
		if (newBoard.Count > 0)
		{
			board.height = newBoard.Count;
			board.width = newBoard[0].Count;
			board.map = newBoard;
			Redraw();
		}
	}

	void ChangeBoard(int[][] newBoard)
	{
		if (newBoard.Length > 0)
		{
			Godot.Collections.Array<Godot.Collections.Array<int>> nb = new Godot.Collections.Array<Godot.Collections.Array<int>>();
			board.height = newBoard.Length;
			board.width = newBoard[0].Length;
			for (int y = 0; y < board.height; y++)
			{
				nb.Add(new Godot.Collections.Array<int>());
				for (int x = 0; x < board.width; x++)
				{
					nb[y].Add(newBoard[y][x]);
				}
			}
			board.map = nb;
			Redraw();
		}
	}


	//------------------UTILS-------------------//
	int Side(Around side)
	{
		return (int)side;
	}

	void ChangeTilesetID(int id)
	{
		tilesetID = id;

		if (tm.TileSet != tilesets[tilesetID])
		{
			tm.TileSet = tilesets[tilesetID];
			Redraw();
		}
	}

	bool IsBetween(int x, int min, int max)
	{
		if (x < min)
			return false;
		if (x > max)
			return false;
		return true;
	}

	public bool CheckSides(bool[] source, bool[] target)
	{
		for (int i = 0; i < 8; i++)
		{
			if (source[i] != target[i])
			{
				return false;
			}
		}
		return true;
	}

	Vector2 GetIdFromPos(Vector2 pos)
	{
		Vector2 id = new Vector2(0, 0);
		Vector2 boardPos = this.Position;
		Vector2 tileSize = tm.Scale * ts.TileGetRegion(0).Size;
		Vector2 boardSize = new Vector2(board.width, board.height);

		id = (pos - boardPos) / (tileSize) + new Vector2(-1, -1);
		if (id.x < 0 || id.y < 0 || id.x >= boardSize.x || id.y >= boardSize.y)
			return new Vector2(-1, -1);
		return id;
	}

	int GetDataFromId(Vector2 id)
	{
		if (!board.IsInMap((int)id.x, (int)id.y))
			return -1;
		return board.map[(int)id.y][(int)id.x];
	}

	public int remainingEggs()
	{
		int nb = 0;
		for (int y = 0; y < board.height; y++)
		{
			for (int x = 0; x < board.width; x++)
			{
				if (board.map[y][x] > 0)
				{
					nb++;
				}
			}
		}
		return nb;
	}

	//-----------------------TILES SETTER------------------//

	public bool[] CheckAround(int x, int y)
	{
		bool[] sides = new bool[8];
		for (int i = 0; i < 8; i++) sides[i] = false;

		sides[Side(Around.RIGHT)] = (board.IsMap(x + 1, y)) ? true : false;
		sides[Side(Around.LEFT)] = (board.IsMap(x - 1, y)) ? true : false;
		sides[Side(Around.DOWN)] = (board.IsMap(x, y + 1)) ? true : false;
		sides[Side(Around.UP)] = (board.IsMap(x, y - 1)) ? true : false;
		sides[Side(Around.BOTTOM_RIGHT)] = (board.IsMap(x + 1, y + 1)) ? true : false;
		sides[Side(Around.UPPER_RIGHT)] = (board.IsMap(x + 1, y - 1)) ? true : false;
		sides[Side(Around.BOTTOM_LEFT)] = (board.IsMap(x - 1, y + 1)) ? true : false;
		sides[Side(Around.UPPER_LEFT)] = (board.IsMap(x - 1, y - 1)) ? true : false;
		return sides;
	}

	//UP, LEFT, RIGHT, DOWN, UPPER_LEFT, UPPER_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT
	private int GetTileAroundID(bool[] side)
	{
		if (CheckSides(side, new bool[] {false, false, false, false, false, false, false, false})) return (int)Tile.NONE;

		//Down only
		if (CheckSides(side, new bool[] {true, false, false, false, false, false, false, false})) return (int)Tile.DOWN;
		if (CheckSides(side, new bool[] {true, false, false, false, true, true, false, false})) return (int)Tile.DOWN;
		if (CheckSides(side, new bool[] {true, false, false, false, false, true, false, false})) return (int)Tile.DOWN;
		if (CheckSides(side, new bool[] { true, false, false, false, true, false, false, false })) return (int)Tile.DOWN;

		//Right only
		if (CheckSides(side, new bool[] {false, true, false, false, false, false, false, false})) return (int)Tile.RIGHT;
		if (CheckSides(side, new bool[] {false, true, false, false, true, false, true, false})) return (int)Tile.RIGHT;
		if (CheckSides(side, new bool[] {false, true, false, false, false, false, true, false})) return (int)Tile.RIGHT;
		if (CheckSides(side, new bool[] {false, true, false, false, true, false, false, false})) return (int)Tile.RIGHT;

		//Left only
		if (CheckSides(side, new bool[] {false, false, true, false, false, false, false, false})) return (int)Tile.LEFT;
		if (CheckSides(side, new bool[] {false, false, true, false, false, true, false, true})) return (int)Tile.LEFT;
		if (CheckSides(side, new bool[] {false, false, true, false, false, true, false, false})) return (int)Tile.LEFT;
		if (CheckSides(side, new bool[] {false, false, true, false, false, false, false, true})) return (int)Tile.LEFT;

		//Up only
		if (CheckSides(side, new bool[] {false, false, false, true, false, false, false, false})) return (int)Tile.UP;
		if (CheckSides(side, new bool[] {false, false, false, true, false, false, true, true})) return (int)Tile.UP;
		if (CheckSides(side, new bool[] {false, false, false, true, false, false, false, true})) return (int)Tile.UP;
		if (CheckSides(side, new bool[] {false, false, false, true, false, false, true, false})) return (int)Tile.UP;

		//outside corners
		if (CheckSides(side, new bool[] {false, false, false, false, true, false, false, false})) return (int)Tile.BOTTOM_RIGHT;
		if (CheckSides(side, new bool[] {false, false, false, false, false, true, false, false})) return (int)Tile.BOTTOM_LEFT;
		if (CheckSides(side, new bool[] {false, false, false, false, false, false, true, false})) return (int)Tile.UPPER_RIGHT;
		if (CheckSides(side, new bool[] {false, false, false, false, false, false, false, true})) return (int)Tile.UPPER_LEFT;

		//inside corners
		if (CheckSides(side, new bool[] {true, true, false, false, true, false, false, false})) return (int)Tile.INSIDE_BOTTOM_RIGHT;
		if (CheckSides(side, new bool[] {true, false, true, false, false, true, false, false})) return (int)Tile.INSIDE_BOTTOM_LEFT;
		if (CheckSides(side, new bool[] {false, true, false, true, false, false, true, false})) return (int)Tile.INSIDE_UPPER_RIGHT;
		if (CheckSides(side, new bool[] {false, false, true, true, false, false, false, true})) return (int)Tile.INSIDE_UPPER_LEFT;

		return (int)Tile.NONE;
	}

	//-----------BOARD INTERACTION-------------------//

	public bool BreakEgg(int x, int y)
	{
		if (!board.IsMap(x, y))
		{
			return false;
		}
		if (board.map[y][x] > 0)
		{
			board.map[y][x] = 0;
			Redraw();
			return true;
		}
		Redraw();
		return false;
	}

	public bool CheckAlignement()
	{
		for (int y = 0; y < board.height; y++)
		{
			for (int x = 0; x < board.width; x++)
			{
				if (board.GetEgg(x, y) > 0 && board.GetEgg(x - 1, y) == board.GetEgg(x, y) && board.GetEgg(x + 1, y) == board.GetEgg(x, y))
					return true;
				if (board.GetEgg(x, y) > 0 && board.GetEgg(x, y - 1) == board.GetEgg(x, y) && board.GetEgg(x, y + 1) == board.GetEgg(x, y))
					return true;
			}
		}
		return false;
	}

	//-----------BOARD SETTER--------------------//

	void ResizeBoard()
	{
		Vector2 new_size = new Vector2();
		Vector2 screen_size = OS.WindowSize;
		Vector2 tile_size = ts.TileGetRegion(0).Size;
		screen_size.x *= 0.8f;
		screen_size.y *= 0.6f;
		new_size = screen_size / new Vector2(board.width + 3, board.height + 3);
		new_size = new_size / tile_size;

		if (new_size.x < new_size.y)
			new_size.y = new_size.x;
		else
			new_size.x = new_size.y;
		tm.Scale = new_size;

		new_size = tile_size * tm.Scale;
		new_size *= new Vector2(board.width + 2, board.height + 2);
		screen_size = OS.WindowSize;
		Vector2 offset = new Vector2(0, 0);
		offset = (screen_size - new_size) / new Vector2(2, 10);
		this.Position = offset;
	}

	void DrawMap()
	{
		for (int y = -1; y <= board.height; y++)
		{
			for (int x = -1; x <= board.width; x++)
			{
				if (y < 0 || y >= board.height || x < 0 || x >= board.width || board.map[y][x] < 0)
				{
					bool[] sides = CheckAround(x, y);
					tm.SetCell(x + 1, y + 1, GetTileAroundID(sides));
				}
				else
				{
					tm.SetCell(x + 1, y + 1, (int)Tile.CENTER);
				}
			}
		}
	}

	//-------------------EGG SPAWNER----------------//

	Node2D SpawnEgg()
	{
		Node node = eggSampleScene.Instance();
		Node2D eggSample = node.GetChild<Node2D>(0);
		node.RemoveChild(eggSample);
		eggFolder.AddChild(eggSample);
		node.QueueFree();
		return eggSample;
	}

	private void RandomizeBoard(int[] eggList)
	{
		Godot.RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.Randomize();

		for (int y = 0; y < board.height; y++)
		{
			for (int x = 0; x < board.width; x++)
			{
				while (board.map[y][x] == 0 || CheckAlignement())
				{
					board.map[y][x] = eggList[rng.RandiRange(0, eggList.Length - 1)];
				}
			}
		}
	}

	void InitEggList()
	{
		eggList = new Godot.Collections.Array<Godot.Collections.Array<Node2D>>();

		for (int y = 0; y < board.height; y++)
		{
			eggList.Add(new Godot.Collections.Array<Node2D>());
			for (int x = 0; x < board.width; x++)
			{
				if (board.map[y][x] >= 0)
				{
					Node2D egg = SpawnEgg();
					Sprite eggSp = egg.GetChild<Sprite>(0);
					Vector2 scale = (ts.TileGetRegion(0).Size / eggSp.Texture.GetSize()) * tm.Scale;
					scale *= new Vector2(0.95f, 0.95f);
					Vector2 pos = (scale * eggSp.Texture.GetSize() / 2) + new Vector2(x + 1, y + 1) * ts.TileGetRegion(0).Size * tm.Scale;
					egg.Position = pos;
					egg.Scale = scale;
					egg.Call("change_id", board.map[y][x]);
					eggList[y].Add(egg);
				} else
					eggList[y].Add(new Node2D());
			}
		}
	}

	void gravity()
	{
		for (int y = board.height - 1; y > 0; y--)
		{
			for (int x = 0; x < board.width; x++)
			{
				if (board.map[y - 1][x] > 0 && board.map[y][x] == 0)
				{
					board.map[y][x] = board.map[y - 1][x];
					board.map[y - 1][x] = 0;
				}
			}
		}
	}

	void UpdateEggs()
	{
		for (int y = 0; y < board.height; y++)
		{
			for (int x = 0; x < board.width; x++)
			{
				if (board.map[y][x] >= 0)
				{
					Node2D egg = eggList[y][x];
					egg.Call("change_id", board.map[y][x]);
				}
			}
		}
	}

	//--------------------CALLERS------------------//

	void Redraw()
	{
		ResizeBoard();
		DrawMap();
		tm.UpdateDirtyQuadrants();
	}	

	//---------------TESTS-------------------------//

	void SetBasicMap()
	{
		int[][] newBoard = new int[][]{
			new int[]{1}
		};
		ChangeBoard(newBoard);
	}

	/* =================================================================================================================== *
	 *                                                   MAIN FUNCTIONS                                                    *
	 * =================================================================================================================== */

	public override void _Ready()
	{
		//get components
		generalData = GetNode<Node>("/root/GAME");
		eggFolder = GetNode<Node2D>("eggs");
		tm = GetNode<TileMap>("TileMap");
		ts = tm.TileSet;

		SetBasicMap();
	}

	public void SecondStart()
	{
		Godot.Collections.Array switchData = (Godot.Collections.Array)generalData.Call("GetSwitchData");

		LevelData ld = (LevelData)switchData[1];

		if (ld.map == null)
			SetBasicMap();
		else
		{
			ChangeBoard(ld.map);
			RandomizeBoard(new int[] {1, 2, 3});
		}
		InitEggList();
		Redraw();
	}

	public override void _Process(float delta)
	{
		if (!secondStart)
		{
			SecondStart();
			secondStart = true;
		}
		gravity();
		UpdateEggs();
	}
}
