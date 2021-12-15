using Godot;
using Godot.Collections;

public class Item : Node2D
{
	[Export] public ItemStats ItemStats;
	
	private Sprite _sprite;
	private PlayerDetectionArea _playerDetectionArea;
	private AnimationPlayer _animationPlayer;
	
	private bool _caught = false;
	
	private Inventory _playerInventory;
	
	public override void _Ready()
	{
		_sprite = GetNode<Sprite>("Sprite");
		_playerDetectionArea = GetNode<PlayerDetectionArea>("PlayerDetectionArea");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		
		_playerInventory = GD.Load<Inventory>("res://Inventory/PlayerInventory.tres");
		
		if (ItemStats == null)
		{
			GD.PrintErr("ItemStats was not defined at Item.cs");
			return;
		}
		
		_playerDetectionArea.Connect("PlayerEnteredArea", this, nameof(OnPlayerEnteredArea));
		_sprite.Texture = ItemStats.Texture;
	}
	
	private void OnPlayerEnteredArea(Player player)
	{
		if (_caught) return;
		
		var added = _playerInventory.AddItem(ItemStats);
		if (!added) return;
		
		_caught = true;
		_animationPlayer.Play("PickUp");
	}
	
	public Dictionary Save()
	{
		var saveDict = new Dictionary()
		{
			{ "ItemStats", ItemStats }
		};
		return saveDict;
	}
	
	public void Load(Dictionary infoDict)
	{
		ItemStats = infoDict["ItemStats"] as ItemStats;
	}
}
