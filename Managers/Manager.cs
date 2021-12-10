using Godot;

public class Manager : CanvasLayer
{
	[Export] public int GridSize = 16;
	[Export] public Vector2 _playerGridInitialPosition = Vector2.Zero;
	
	public bool InMenu = false;
	
	private Control _pauseDisplayer;
	
	private PackedScene _playerPreload = GD.Load<PackedScene>("res://Player/Player.tscn");
	private PackedScene _pauseDisplayerPreload = GD.Load<PackedScene>("res://PauseDisplayer/PauseDisplayer.tscn");
	
	public override void _Input(InputEvent @event)
	{
		if (Global.BattleManager.InBattle || InMenu) return;
		if (Global.InteractionManager.Interaction != null || !@event.IsActionPressed("ui_pause")) return;
		
		if (GetTree().Paused)
		{
			UnpauseGame();
		}
		else
		{
			PauseGame();
		}
	}
	
	public void PauseGame()
	{
		_pauseDisplayer = _pauseDisplayerPreload.Instance<Control>();
		AddChild(_pauseDisplayer);
		
		GetTree().Paused = true;
	}
	
	public void UnpauseGame()
	{
		_pauseDisplayer.QueueFree();
		_pauseDisplayer = null;
		
		GetTree().Paused = false;
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
