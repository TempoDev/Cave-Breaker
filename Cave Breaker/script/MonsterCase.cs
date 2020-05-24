using Godot;
using System;

public class MonsterCase : Control
{
	[Export] private Texture empty_slot;

	OwnedMonster monster = null;
	bool active = false;
	int remaining = 0;

	private TextureButton button;
	private Label label;

	public void SetMonster(OwnedMonster newm)
	{
		monster = newm;
		if (monster != null)
			remaining = monster.dataBase.GetMoveQuantity();
	}
	public void SetActive(bool activate)
	{
		active = activate;
	}

	public OwnedMonster GetMonster()
	{
		return monster;
	}

	public int GetRemaining()
	{
		return remaining;
	}

	public void AddRemaining(int nb)
	{
		remaining += nb;
	}

	public void SetRemaining(int nb)
	{
		remaining = nb;
	}

	public void RemoveRemaining(int nb)
	{
		remaining -= nb;
	}

	private void UpdateSprite()
	{
		if (monster == null)
		{
			button.TextureNormal = empty_slot;
		} else
		{
			if (active || monster.dataBase.isPassive())
			{
				if (monster.dataBase.GetSlotAction() != null) button.TextureNormal = monster.dataBase.GetSlotAction();
			} else
			{
				if (monster.dataBase.GetSlotSleep() != null) button.TextureNormal = monster.dataBase.GetSlotSleep();
			}
		}
	}

	private void UpdateLabel()
	{
		if (monster == null)
			label.Text = "";
		else
			label.Text = remaining.ToString();
	}

	public override void _Ready()
	{
		button = GetNode<TextureButton>("button");
		label = GetNode<Label>("Label");
	}

	public override void _Process(float delta)
	{
		UpdateSprite();
		UpdateLabel();
	}

}
