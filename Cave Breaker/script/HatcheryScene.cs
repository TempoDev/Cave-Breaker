using Godot;
using System;

public class HatcheryScene : Node2D
{

	private Node generalData = null;

	Hatchery egg;
	double waitTime = 30;

	Label label;
	Node2D eggSprite;

	private void _on_background_pressed()
	{
		Godot.RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.Randomize();
		TimeSpan ts = DateTime.Now - egg.start;

		if (ts.TotalSeconds >= waitTime)
		{
			generalData.Call("AddMonster", rng.RandiRange(0, MonsterList.list.Length - 1));
			generalData.Call("RemoveEgg");
			egg.egg = 0;
			SetEgg();
		}
	}

	private void _on_Back_pressed()
	{
		generalData.Call("SwitchScene", general.Scene.MAP, null);
	}

	private void SetEgg()
	{
		egg = (Hatchery)generalData.Call("GetEgg");
		eggSprite.Call("change_id", egg.egg);
	}

	private void UpdateLabel()
	{
		TimeSpan ts = DateTime.Now - egg.start;

		if (egg.egg <= 0)
			label.Text = "NO EGG";
		else
		{
			if (ts.TotalSeconds >= waitTime)
				label.Text = "HATCH";
			else
			{
				TimeSpan elapsed = TimeSpan.FromSeconds(waitTime - ts.TotalSeconds);
				label.Text = elapsed.Hours.ToString() + ":" + elapsed.Minutes.ToString() + ":" + elapsed.Seconds.ToString();
			}
		}
	}

	public override void _Process(float delta)
	{
		UpdateLabel();
	}

	public override void _Ready()
	{
		generalData = GetNode<Node>("/root/GAME");
		label = GetNode<Label>("nest/button/Label");
		eggSprite = GetNode<Node2D>("nest/egg/egg");
		SetEgg();
	}
}
