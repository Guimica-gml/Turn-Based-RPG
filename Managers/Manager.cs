using Godot;

public class Manager : CanvasLayer
{
	[Signal] private delegate void GamePaused();
	[Signal] private delegate void GameUnpaused();
	
	[Export] public int GridSize = 16;
	
	public bool InMenu = false;
	
	private PauseDisplayer _pauseDisplayer;
	
	private PackedScene _playerPreload = GD.Load<PackedScene>("res://Player/Player.tscn");
	private PackedScene _pauseDisplayerPreload = GD.Load<PackedScene>("res://PauseDisplayer/InventoryPauseDisplayer/InventoryPauseDisplayer.tscn");
	
	public override void _Input(InputEvent @event)
	{
		if (InMenu || Global.InteractionManager.Interaction != null || !@event.IsActionPressed("ui_pause")) return;
		
		if (!GetTree().Paused)
		{
			var pauseInteraction = _pauseDisplayerPreload.Instance<PauseDisplayer>();
			PauseGame(pauseInteraction);
		}
	}
	
	public void PauseGame(PauseDisplayer pauseDisplayer)
	{
		_pauseDisplayer = pauseDisplayer;
		_pauseDisplayer.Connect("Close", this, "UnpauseGame");
		AddChild(_pauseDisplayer);
		
		GetTree().Paused = true;
		EmitSignal(nameof(GamePaused));
	}
	
	private void UnpauseGame()
	{
		_pauseDisplayer.QueueFree();
		_pauseDisplayer = null;
		
		GetTree().SetDeferred("paused", false);
		EmitSignal(nameof(GameUnpaused));
	}
	
	public MainCamera GetMainCamera()
	{
		return GetTree().CurrentScene.FindNode("MainCamera") as MainCamera;
	}
	
	public YSort GetYSort()
	{
		return GetTree().CurrentScene.FindNode("YSort") as YSort;
	}
	
	public TileMap GetCollisionTileMap()
	{
		return GetTree().CurrentScene.FindNode("CollisionTileMap") as TileMap;
	}
}
