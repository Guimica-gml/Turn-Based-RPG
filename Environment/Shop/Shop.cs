using Godot;
using Godot.Collections;

public class Shop : StaticBody2D
{
	[Export] private Array<ItemStats> _items = new Array<ItemStats>();
	
	private InteractableArea _interactableArea;
	
	public override void _Ready()
	{
		_interactableArea = GetNode<InteractableArea>("InteractableArea");
		
		// Making the items unique
		for (int i = 0; i < _items.Count; ++i)
		{
			_items[i] = _items[i].Duplicate() as ItemStats;
		}
		
		var interaction = new ShopInteraction();
		interaction.Items = _items;
		
		_interactableArea.Interaction = interaction;
	}
}
