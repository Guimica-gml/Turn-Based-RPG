using Godot;

public class MainCamera : Camera2D
{
	[Export] private bool _useLimits = false;
	
	private Position2D _topLeftLimit;
	private Position2D _bottomRightLimit;
	
	public override void _Ready()
	{
		_topLeftLimit = GetNode<Position2D>("Limits/TopLeftLimit");
		_bottomRightLimit = GetNode<Position2D>("Limits/BottomRightLimit");
		
		if (!_useLimits) return;
		
		LimitRight = (int) _bottomRightLimit.GlobalPosition.x - (int) Offset.x;
		LimitBottom = (int) _bottomRightLimit.GlobalPosition.y - (int) Offset.y;
		LimitLeft = (int) _topLeftLimit.GlobalPosition.x - (int) Offset.x;
		LimitTop = (int) _topLeftLimit.GlobalPosition.y - (int) Offset.y;
	}
}
