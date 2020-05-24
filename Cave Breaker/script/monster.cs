using Godot;
using System;
using System.Diagnostics;

public enum ATTACKS { GROUNDBREAK, EXPLODE };

public class Attack
{
	public delegate bool LaunchCondition(ref Godot.Collections.Array data, Node2D board);
	public delegate bool LaunchAttack(ref Godot.Collections.Array data, Node2D board);
	public delegate void OnCLick(ref Godot.Collections.Array data, int x, int y);

	private LaunchCondition launchCondition;
	private LaunchAttack launchAttack;
	private OnCLick onClick;
	private Godot.Collections.Array data = new Godot.Collections.Array();
	private bool isPassive = false;

	public bool Launch(Node2D board)
	{
		if (launchCondition(ref data, board))
			return launchAttack(ref data, board);
		return false;
	}

	public void HasBeenClicked(int x, int y)
	{
		onClick(ref data, x, y);
	}

	public bool IsPassive()
	{
		return isPassive;
	}

	public Attack(LaunchCondition passiveCondition, LaunchAttack passiveAction)
	{
		isPassive = true;
		launchCondition = passiveCondition;
		launchAttack = passiveAction;
		onClick = null;
	}
	public Attack(LaunchCondition activeCondition, LaunchAttack activeAction, OnCLick activeOnClick)
	{
		isPassive = false;
		launchCondition = activeCondition;
		launchAttack = activeAction;
		onClick = activeOnClick;
	}
}

public class Monster
{
	private Attack attack;
	private Texture active;
	private Texture sleep;
	private int move_quantity;

	public bool isPassive()
	{
		return attack.IsPassive();
	}

	public bool Attack(Node2D board)
	{
		return attack.Launch(board);
	}
	public void isClicked(int x, int y)
	{
		attack.HasBeenClicked(x, y);
	}

	public Texture GetSlotSleep()
	{
		return sleep;
	}

	public int GetMoveQuantity()
	{
		return move_quantity;
	}

	public Texture GetSlotAction()
	{
		return active;
	}

	public Monster(ATTACKS move, string slot_action, int move_qt)
	{
		attack = AttackList.list[(int)move];
		active = (Texture)GD.Load("res://res/img/slots/" + slot_action);
		sleep = null;
		move_quantity = move_qt;
	}

	public Monster(ATTACKS move, string slot_action, string slot_sleep, int move_qt)
	{
		attack = AttackList.list[(int)move];
		active = (Texture)GD.Load("res://res/img/slots/" + slot_action);
		sleep = (Texture)GD.Load("res://res/img/slots/" + slot_sleep);
		move_quantity = move_qt;
	}
}

//lists 

public static class AttackList
{

	public static Attack[] list = {
		new Attack(BreakDownCondition, BreakDownAttack, BreakDownOnCLick),
		new Attack(ExplodeCondition, BreakDownAttack, BreakDownOnCLick)
	};

	//breakDown
	static bool BreakDownCondition(ref Godot.Collections.Array data, Node2D board)
	{
		if (data.Count <= 0)
			return false;
		if (data[0].GetType().Equals(new Vector2().GetType()))
		{
			Vector2 v = (Vector2)data[0];
			int under = (int)board.Call("GetDataFromId", new Vector2(v.x, v.y + 1));
			if (under < 0)
				return true;
			else
				data.RemoveAt(0);
		}
		return false;
	}

	static bool BreakDownAttack(ref Godot.Collections.Array data, Node2D board)
	{
		if (data.Count <= 0)
			return false;
		Vector2 pos = (Vector2)data[0];
		bool success = (bool)board.Call("BreakEgg", pos.x, pos.y);
		data.RemoveAt(0);
		return success;
	}

	static void BreakDownOnCLick(ref Godot.Collections.Array data, int x, int y)
	{
		data.Add(new Vector2(x, y));
	}

	//explode
	static bool ExplodeCondition(ref Godot.Collections.Array data, Node2D board)
	{
		if (data.Count <= 0)
			return false;
		if (data[0].GetType().Equals(new Vector2().GetType()))
		{
			return true;
		}
		return false;
	}
}

public static class MonsterList
{
	public enum monsters { MONSTA, STRONG, SPOODER };
	public static Monster[] list = {
		new Monster(ATTACKS.GROUNDBREAK, "Monsta/Monsta_action.png", "Monsta/Monsta_sleep.png", 10),
		new Monster(ATTACKS.GROUNDBREAK, "Monsta/Monsta_action.png", "Monsta/Monsta_sleep.png", 20),
		new Monster(ATTACKS.EXPLODE, "Monsta/Monsta_action.png", "Monsta/Monsta_sleep.png", 3)
	};

	public static int DataToInt(Monster m)
	{
		int i = 0;
		for (; i < list.Length; i++)
		{
			if (list[i].Equals(m))
				return i;
		}
		return -1;
	}
}
