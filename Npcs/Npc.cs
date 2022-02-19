using Godot;
using Godot.Collections;

public class Npc : Entity
{
	[Export] private Vector2 _direction = Vector2.Zero;
	[Export] private bool _wander = false;
	[Export] private int _wanderBlocks = 0;
	[Export] private float _wanderTime = 0;
	
	private Array<Vector2> _aStarPath = new Array<Vector2>();
	private Vector2 _initialPosition;
	private AStar2D _aStar;
	
	private Timer _idleTimer;
	private KinematicBody2D _worldChecker;
	
	public override void _Ready()
	{
		base._Ready();
		GD.Randomize();
		
		_aStar = new AStar2D();
		_initialPosition = GlobalPosition;
		
		_idleTimer = GetNode<Timer>("IdleTimer");
		
		if (_wander)
		{
			MapWorld();
			_idleTimer.Start(_wanderTime);
		}
		
		UpdateDirection(_direction);
	}
	
	protected override Vector2 CheckForInput()
	{
		if (!_wander) return Vector2.Zero;
		
		if (_aStarPath.Count > 0)
		{
			var savePos = _aStarPath[0];
			_aStarPath.RemoveAt(0);
			return GlobalPosition.DirectionTo(savePos);
		}
		
		if (_idleTimer.IsStopped()) _idleTimer.Start(_wanderTime);
		return Vector2.Zero;
	}
	
	private Array<Vector2> GetPathToTargetPosition()
	{
		var gridSize = Global.Manager.GridSize;
		var targPosition = GetRandomPosition();
		
		var currPositionId = GetIdFromPosition(new Vector2(((int) GlobalPosition.x / gridSize) * gridSize, ((int) GlobalPosition.y / gridSize) * gridSize));
		var targPositionId = GetIdFromPosition(new Vector2(((int) targPosition.x / gridSize) * gridSize, ((int) targPosition.y / gridSize) * gridSize));
		
		if (currPositionId == -1 || targPositionId == -1) return new Array<Vector2>();
		var _aStarPath = _aStar.GetPointPath(currPositionId, targPositionId);
		
		return new Array<Vector2>(_aStarPath);
	}
	
	// Replace to AStar2D.GetClosestPoint since I had some problems with that method
	private int GetIdFromPosition(Vector2 position)
	{
		var points = new Array<int>(_aStar.GetPoints());
		
		foreach (var id in points)
		{
			var pos = _aStar.GetPointPosition(id);
			if (pos == position) return id;
		}
		
		return -1;
	}
	
	private void MapWorld()
	{
		var gridSize = Global.Manager.GridSize;
		var index = 0;
		
		CreateWorldChecker();
		
		for (var y = -_wanderBlocks; y <= _wanderBlocks; ++y)
		{
			for (var x = -_wanderBlocks; x <= _wanderBlocks; ++x)
			{
				var pos = new Vector2(GlobalPosition.x + (x * gridSize), GlobalPosition.y + (y * gridSize));
				
				if (!TestCollisionAtPosition(pos))
				{
					// Adding the point
					_aStar.AddPoint(index, pos);
					
					// Connecting the point to its neighbours (left and top)
					if (_aStar.HasPoint(index - 1)) _aStar.ConnectPoints(index, index - 1);
					var up = GetIdFromPosition(pos - new Vector2(0f, gridSize));
					if (_aStar.HasPoint(up)) _aStar.ConnectPoints(index, up);
				}
				
				index++;
			}
		}
		
		_worldChecker.QueueFree();
		_worldChecker = null;
	}
	
	private void CreateWorldChecker()
	{
		_worldChecker = new KinematicBody2D();
		var collisionShape = new CollisionShape2D();
		var shape = new RectangleShape2D();
		
		_worldChecker.AddChild(collisionShape);
		
		_worldChecker.CollisionLayer = 0;
		_worldChecker.CollisionMask = 1;
		shape.Extents = new Vector2(2f, 2f);
		collisionShape.Position = Vector2.Zero;
		collisionShape.Shape = shape;
		
		AddChild(_worldChecker);
	}
	
	private bool TestCollisionAtPosition(Vector2 position)
	{
		var savePos = _worldChecker.GlobalPosition;
		_worldChecker.GlobalPosition = position + new Vector2(8f, 8f);
		var result = _worldChecker.MoveAndCollide(Vector2.Zero);
		_worldChecker.GlobalPosition = savePos;
		return (result != null);
	}
	
	private Vector2 GetRandomPosition()
	{
		var gridSize = Global.Manager.GridSize;
		var wanderRadius = _wanderBlocks * gridSize;
		
		var position = _initialPosition + new Vector2((int) GD.RandRange(-wanderRadius, wanderRadius), (int) GD.RandRange(-wanderRadius, wanderRadius));
		var randomPosition = new Vector2((int) position.x / gridSize * gridSize, (int) position.y / gridSize * gridSize);
		
		return randomPosition;
	}
	
	private void OnIdleTimerTimeout()
	{
		_aStarPath = GetPathToTargetPosition();
	}
	
	public Dictionary Save()
	{
		var saveDict = new Dictionary()
		{
			{ "DirX", _direction.x },
			{ "DirY", _direction.y },
			{ "Wander", _wander },
			{ "WanderRadius", _wanderBlocks },
			{ "WanderTime", _wanderTime },
		};
		return saveDict;
	}
	
	public void Load(Dictionary infoDict)
	{
		_direction = new Vector2((float) infoDict["DirX"], (float) infoDict["DirY"]);
		_wander = (bool) infoDict["Wander"];
		_wanderBlocks = (int) infoDict["WanderRadius"];
		_wanderTime = (float) infoDict["WanderTime"];
	}
}
