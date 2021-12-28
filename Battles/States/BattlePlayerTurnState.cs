using Godot;
using Godot.Collections;

public class BattlePlayerTurnState : State
{
	[Export] private NodePath _battleScenarioPath = "";
	private BattlePauseDisplayer _battleScenario;
	
	[Export] private NodePath _actionsContainerPath = "";
	private GridContainer _actionsContainer;
	
	[Export] private NodePath _itemsButtonPath = "";
	private Button _itemsButton;
	
	[Export] private NodePath _inventoryPath = "";
	private PauseDisplayer _inventory;
	
	private string _usedItemName = "";
	private bool _performedAction = false;
	private BattleCharacterDisplayer _infoDisplayer = null;
	private Inventory _playerInventory = null;
	
	public override void StateReady()
	{
		_playerInventory = GD.Load<Inventory>("res://Inventory/PlayerInventory.tres");
		
		_battleScenario = GetNode<BattlePauseDisplayer>(_battleScenarioPath);
		_actionsContainer = GetNode<GridContainer>(_actionsContainerPath);
		_itemsButton = GetNode<Button>(_itemsButtonPath);
		_inventory = GetNode<PauseDisplayer>(_inventoryPath);
		
		_battleScenario.CurrentTurn = BattlePauseDisplayer.Turns.Player;
		_battleScenario.SetActionButtonsVisibility(true);
		_battleScenario.SetNextMessageArrowVisibility(false);
		_battleScenario.SetBattleText("Select an action.");
		
		var actionButtons = new Array<ActionButton>(_actionsContainer.GetChildren());
		foreach (var actionButton in actionButtons)
		{
			actionButton.FocusMode = Control.FocusModeEnum.None;
			actionButton.Connect("ActionSelected", this, nameof(OnActionButtonSelected));
		}
		
		_itemsButton.FocusMode = Control.FocusModeEnum.None;
		_itemsButton.Connect("pressed", this, nameof(OnItemsButtonPressed));
		
		_playerInventory.Connect("ItemRemoved", this, nameof(OnInventoryItemRemoved));
	}
	
	public override void StateProcess(float delta)
	{
		if (_inventory.Visible && Input.IsActionJustPressed("ui_pause"))
		{
			_inventory.Visible = false;
		}
		
		if ((_performedAction || _usedItemName != "") && _battleScenario.MouseHoverTextBox && Input.IsActionJustPressed("left_click"))
		{
			EmitSignal(nameof(RequestState), "CheckForWinner");
		}
	}
	
	public override void StateFree()
	{
		_usedItemName = "";
		
		_itemsButton.Disconnect("pressed", this, nameof(OnItemsButtonPressed));
		_playerInventory.Disconnect("ItemRemoved", this, nameof(OnInventoryItemRemoved));
		DisconnectActionButtons();
		
		_actionsContainer = null;
		_battleScenario = null;
		_performedAction = false;
	}
	
	private void OnActionButtonSelected(Action action)
	{
		// Applying the action
		if (action.Heal)
		{
			_battleScenario.PlayerDisplayer.ApplyAction(action);
			_infoDisplayer = _battleScenario.PlayerDisplayer;
		}
		else
		{
			_battleScenario.EnemyDisplayer.ApplyAction(action, _battleScenario.PlayerDisplayer.Stats);
			_infoDisplayer = _battleScenario.EnemyDisplayer;
		}
		
		_infoDisplayer.Connect("AnimEnded", this, nameof(OnAnimEnded));
		_battleScenario.SetActionButtonsVisibility(false);
		_battleScenario.SetBattleText($"You selected action {action.Name}.");
	}
	
	private void DisconnectActionButtons()
	{
		var actionButtons = new Array<ActionButton>(_actionsContainer.GetChildren());
		foreach (var actionButton in actionButtons)
		{
			actionButton.Disconnect("ActionSelected", this, nameof(OnActionButtonSelected));
		}
	}
	
	private void OnAnimEnded()
	{
		_performedAction = true;
		_infoDisplayer.Disconnect("AnimEnded", this, nameof(OnAnimEnded));
		_battleScenario.SetNextMessageArrowVisibility(true);
		_infoDisplayer = null;
	}
	
	private void OnItemsButtonPressed()
	{
		_inventory.Visible = true;
	}
	
	private void OnInventoryItemRemoved(string itemName)
	{
		_usedItemName = itemName;
		_battleScenario.SetActionButtonsVisibility(false);
		_battleScenario.SetNextMessageArrowVisibility(true);
		_battleScenario.SetBattleText($"Used item {itemName}.");
		_inventory.SetDeferred("visible", false);
	}
}
