using Godot;
using System;

public class MapDiscovered : Node
{
	public int score;
	public bool discovered;
}

public class OwnedMonster : Node
{
	public Monster dataBase;
	public int level;
	public int xp;

	public OwnedMonster(MonsterList.monsters m)
	{
		level = 1;
		xp = 0;
		dataBase = MonsterList.list[(int)m];
	}

	public OwnedMonster(int index)
	{
		level = 1;
		xp = 0;
		dataBase = MonsterList.list[index];
	}
}

public class general : Node
{
	public enum Scene {MAP, GAME, BESTIARY_BOOK, HATCHERY};
	[Export] Godot.Collections.Array<PackedScene> scenes;

	private int level = 1;
	private int xp = 0;
	private bool[] Bestiary;
	private Godot.Collections.Array<OwnedMonster> fight_team;
	private Godot.Collections.Array<OwnedMonster> myMonsters;
	private Godot.Collections.Array<Godot.Collections.Array<MapDiscovered>> map = null;

	private Node launched = null;

	private Godot.Collections.Array changeSceneData = null;

	/* =================================================================================================================== *
	 *                                                      getter                                                         *
	 * =================================================================================================================== */

	public int GetXP()
	{
		return xp;
	}

	public int GetLevel()
	{
		return level;
	}

	public int GetSlotsNb()
	{
		return fight_team.Count;
	}

	public Godot.Collections.Array<OwnedMonster> GetSlots()
	{
		return fight_team;
	}

	public OwnedMonster GetSlotsAt(int id)
	{
		if (id < fight_team.Count && id >= 0)
			return fight_team[id];
		return null;
	}

	public int GetMyMonstersNb()
	{
		return myMonsters.Count;
	}

	public Godot.Collections.Array<OwnedMonster> GetMyMonsters()
	{
		return myMonsters;
	}

	public bool[] GetBestiary()
	{
		return Bestiary;
	}

	public Node GetLaunched()
	{
		return launched;
	}

	public Godot.Collections.Array<Godot.Collections.Array<MapDiscovered>> GetMapData()
	{
		return map;
	}

	public MapDiscovered GetMapDataNode(int x, int y)
	{
		return map[x][y];
	}

	/* =================================================================================================================== *
	 *                                                      setter                                                         *
	 * =================================================================================================================== */

	public void AddBestiary(MonsterList.monsters discovered)
	{
		Bestiary[(int)discovered] = true;
	}

	public void AddFightSlot(OwnedMonster monster)
	{
		fight_team.Add(monster);
	}

	public void AddMonster(MonsterList.monsters monster)
	{
		fight_team.Add(new OwnedMonster(monster));
	}

	public void AddXp(int add)
	{
		xp += add;
	}

	public void AddLevel()
	{
		level++;
	}
	public void SetMapDataNode(int x, int y, MapDiscovered data)
	{
		map[y][x] = data;
	}
	public void SetMapData(Godot.Collections.Array<Godot.Collections.Array<MapDiscovered>> data)
	{
		map = data;
	}

	public void SetMapScore(int x, int y, int score)
	{
		map[x][y].score = score;
	}

	/* =================================================================================================================== *
	 *                                                   save and load                                                     *
	 * =================================================================================================================== */

	public void Save()
	{
		File saveGame = new File();
		saveGame.Open("user://savegame.save", File.ModeFlags.Write);
		saveGame.StoreLine(JSON.Print(level));

		for (int i = 0; i < Bestiary.Length; i++)
			saveGame.StoreLine(JSON.Print(Bestiary[i]));
		saveGame.StoreLine(JSON.Print("end"));

		for (int i = 0; i < myMonsters.Count; i++)
		{
			saveGame.StoreLine(JSON.Print(MonsterList.DataToInt(myMonsters[i].dataBase)));
			saveGame.StoreLine(JSON.Print(myMonsters[i].level));
			saveGame.StoreLine(JSON.Print(myMonsters[i].xp));
		}
		saveGame.StoreLine(JSON.Print("end"));

		saveGame.StoreLine(JSON.Print(myMonsters.Count));
		for (int i = 0; i < myMonsters.Count; i++)
		{
			int om = myMonsters.IndexOf(fight_team[i]);
			saveGame.StoreLine(JSON.Print(om));
		}

		saveGame.Close();
	}

	public bool Load()
	{
		var saveGame = new File();
		if (!saveGame.FileExists("user://savegame.save"))
			return false;
		saveGame.Open("user://savegame.save", File.ModeFlags.Read);

		level = (int)JSON.Parse(saveGame.GetLine()).Result;

		object result = null;

		string type = "";
		result = JSON.Parse(saveGame.GetLine()).Result;
		for (int i = 0; i < Bestiary.Length && !(result.GetType().Equals(type.GetType()) && ((string)result).Equals("end")); i++)
		{
			Bestiary[i] = (bool)result;
		}

		result = JSON.Parse(saveGame.GetLine()).Result;
		for (int i = 0; !(result.GetType().Equals(type.GetType()) && ((string)result).Equals("end")); i++)
		{
			OwnedMonster om = new OwnedMonster((int)result);
			om.level = (int)JSON.Parse(saveGame.GetLine()).Result;
			om.xp = (int)JSON.Parse(saveGame.GetLine()).Result;
			myMonsters.Add(om);
		}

		int slotsNb = (int)JSON.Parse(saveGame.GetLine()).Result;
		fight_team = new Godot.Collections.Array<OwnedMonster>();
		for (int i = 0; i < slotsNb; i++)
		{
			int id = (int)JSON.Parse(saveGame.GetLine()).Result;
			OwnedMonster om = myMonsters[i];
			fight_team.Add(om);
		}

		saveGame.Close();
		return true;
	}

	public void NewGame()
	{
		Bestiary[0] = true;
		myMonsters.Add(new OwnedMonster(MonsterList.monsters.MONSTA));
		fight_team = new Godot.Collections.Array<OwnedMonster>();
		fight_team.Add(myMonsters[0]);
	}

	public void SwitchScene(Scene newScene, Godot.Collections.Array data)
	{
		launched.QueueFree();
		launched = LoadScene(newScene);
		changeSceneData = data;
	}
	public Godot.Collections.Array GetSwitchData()
	{
		return changeSceneData;
	}

	private Node LoadScene(Scene newScene)
	{
		PackedScene scene = scenes[(int)newScene];
		Node ns = scene.Instance();
		this.AddChild(ns);
		return ns;
	}

	/* =================================================================================================================== *
	 *                                                   MAIN FUNCTIONS                                                    *
	 * =================================================================================================================== */

	public override void _Ready()
	{
		myMonsters = new Godot.Collections.Array<OwnedMonster>();
		Bestiary = new bool[MonsterList.list.Length];
		for (int i = 0; i < Bestiary.Length; i++)
			Bestiary[i] = false;
		if (!Load())
			NewGame();
		launched = LoadScene(Scene.MAP);
	}

	public override void _Process(float delta)
	{

	}
}
