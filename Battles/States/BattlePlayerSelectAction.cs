using Godot;
using Godot.Collections;

public class BattlePlayerSelectAction : BattleUpdateText
{
	public GridContainer ActionsContainer;
	public Button InventoryButton;
	public PauseDisplayer Inventory;
	
	private Inventory PlayerInventory;
	
	public BattlePlayerSelectAction(BattleDisplayer battleDisplayer, GridContainer actionsContainer, Button inventoryButton, PauseDisplayer inventory, string text, bool startInvisible, string nextScheme = "") :
	base(battleDisplayer, text, startInvisible:startInvisible, nextScheme:nextScheme)
	{
		ActionsContainer = actionsContainer;
		InventoryButton = inventoryButton;
		Inventory = inventory;
	}
	
	public override void OnReady()
	{
		BattleDisplayer.SetActionButtonsVisibility(true);
		base.OnReady();
		
		PlayerInventory = GD.Load<Inventory>("res://Inventory/PlayerInventory.tres");
		
		foreach (var actionButton in new Array<ActionButton>(ActionsContainer.GetChildren()))
		{
			actionButton.Connect("ActionSelected", this, nameof(OnActionButtonSelected));
		}
		
		InventoryButton.Connect("pressed", this, nameof(OnInventoryButtonPressed));
		PlayerInventory.Connect("ItemRemoved", this, nameof(OnInventoryItemRemoved));
	}
	
	public override void OnProcess(float delta)
	{
		if (Input.IsActionJustPressed("ui_pause") && Inventory.Visible)
		{
			Inventory.Visible = false;
		}
	}
	
	public override void OnFinish()
	{
		BattleDisplayer.SetActionButtonsVisibility(false);
		
		foreach (var actionButton in new Array<ActionButton>(ActionsContainer.GetChildren()))
		{
			actionButton.Disconnect("ActionSelected", this, nameof(OnActionButtonSelected));
		}
		
		InventoryButton.Disconnect("pressed", this, nameof(OnInventoryButtonPressed));
		PlayerInventory.Disconnect("ItemRemoved", this, nameof(OnInventoryItemRemoved));
	}
	
	private void OnInventoryItemRemoved(string itemName)
	{
		BattleDisplayer.UsedItemName = itemName;
		Inventory.Visible = false;
		EmitSignal(nameof(Finished), NextScheme);
	}
	
	private void OnInventoryButtonPressed()
	{
		Inventory.Visible = true;
	}
	
	private void OnActionButtonSelected(Action action)
	{
		BattleDisplayer.CurrentAction = action;
		EmitSignal(nameof(Finished), NextScheme);
	}
}