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
			generalData.Call("SwitchScene", general.Scene.MAP, null);
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
