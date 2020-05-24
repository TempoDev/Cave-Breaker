using Godot;
using System;

public class MyMonsterCase : Control
{
	[Export] private Texture empty_slot;

	OwnedMonster monster = null;
	bool active = false;

	private TextureButton button;

	public void SetMonster(OwnedMonster newm)
	{
		monster = newm;
	}
	public void SetActive(bool activate)
	{
		active = activate;
	}
	public bool GetActive()
	{
		return active;
	}

	public void Active()
	{
		active = !active;
	}

	public OwnedMonster GetMonster()
	{
		return monster;
	}

	private void UpdateSprite()
	{
		if (monster == null)
		{
			button.TextureNormal = empty_slot;
		}
		else
		{
			if (active || monster.dataBase.isPassive())
			{
				if (monster.dataBase.GetSlotAction() != null) button.TextureNormal = monster.dataBase.GetSlotAction();
			}
			else
			{
				if (monster.dataBase.GetSlotSleep() != null) button.TextureNormal = monster.dataBase.GetSlotSleep();
			}
		}
	}

	public override void _Ready()
	{
		button = GetNode<TextureButton>("button");
	}

	public override void _Process(float delta)
	{
		UpdateSprite();
	}

}
