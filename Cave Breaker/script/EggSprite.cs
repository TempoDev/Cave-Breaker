using Godot;
using System;

/* =========================================================================================================== *
 *                                             EGG SPRITE CLASS                                                *
 *                                   change egg appearance depending on                                        *
 *											       it's id                                                     *
 * =========================================================================================================== */

public class EggSprite : Node2D
{
	/* =================================================================================================================== *
	 *                                                  VARIABLES                                                          *
	 * =================================================================================================================== */

	[Export] private Godot.Collections.Array<Texture> sprites;
	[Export] public int id;

	private Sprite sprite;

	public void change_id(int newId)
	{
		if (newId < 0 || newId > sprites.Count)
			return;
		id = newId - 1;
	}

	/* =================================================================================================================== *
	 *                                                   MAIN FUNCTIONS                                                    *
	 * =================================================================================================================== */

	public override void _Ready()
	{
		sprite = GetNode<Sprite>("sprite");
	}


	public override void _Process(float delta)
	{
		if (id < 0)
			sprite.Texture = null;
		if (id < 0 || id >= sprites.Count)
			return;
		if (sprite.Texture != sprites[id])
			sprite.Texture = sprites[id];
	}
}
