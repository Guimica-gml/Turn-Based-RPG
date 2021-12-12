using Godot;
using Godot.Collections;

public class ShopPauseDisplayer : PauseDisplayer
{
	[Export] public Array<ItemStats> Items = new Array<ItemStats>();
}
