using Godot;
using System;

public class gameManager : Node
{

	Node boardGame;
	Node monsterBar;
	Node generalData;

	/* =================================================================================================================== *
	 *                                                        LOSE                                                         *
	 * =================================================================================================================== */

	public bool CheckLose()
	{
		int remaining = (int)monsterBar.Call("GetRemaining");
		bool alignment = (bool)boardGame.Call("CheckAlignement");
		if (remaining <= 0 || alignment)
			return true;
		return false;
	}

	void Lose()
	{
		if (CheckLose())
		{
			int egg = (int)boardGame.Call("GetLoseID");
			Hatchery OwnedEgg = (Hatchery)generalData.Call("GetEgg");

			if (OwnedEgg.egg <= 0)
				generalData.Call("SetEgg", egg);
			generalData.Call("SwitchScene", general.Scene.MAP, null);
		}
	}

	/* =================================================================================================================== *
	 *                                                        WIN                                                          *
	 * =================================================================================================================== */
	public bool CheckWin()
	{
		int remaining = (int)boardGame.Call("remainingEggs");
		if (remaining <= 0)
			return true;
		return false;
	}

	void Win()
	{
		if (CheckWin())
		{
			Godot.Collections.Array data = (Godot.Collections.Array)generalData.Call("GetSwitchData");

			Vector2 id = (Vector2)data[0];

			MapDiscovered md = new MapDiscovered();
			md.discovered = true;
			generalData.Call("SetMapDataNode", (int)id.x, (int)id.y, md);
			generalData.Call("SwitchScene", general.Scene.MAP, null);
		}
	}

	/* =================================================================================================================== *
	 *                                                   MAIN FUNCTIONS                                                    *
	 * =================================================================================================================== */

	public override void _Ready()
	{
		generalData = GetNode<Node>("/root/GAME");
		monsterBar = GetNode<Node>("MonsterBar/MonsterBar");
		boardGame = GetNode<Node>("gameboard/board");
	}

	public override void _Process(float delta)
	{
		Win();
		Lose();
	}
}
