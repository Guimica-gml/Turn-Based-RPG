using Godot;
using Godot.Collections;

public class Npc : Entity
{
	[Export] private Vector2 _direction = Vector2.Zero;
	[Export] private bool _wander = false;
	[Export] private int _wanderRadius = 0;
	[Export] private float _wanderTime = 0;
	
	[Export] private NodePath _collisionTileMapPath = "";
	private TileMap _collisionTileMap = null;
	
	private Array<Vector2> _aStarPath = new Array<Vector2>();
	private Vector2 _initialPosition;
	private AStar2D _aStar;
	
	private Timer _idleTimer;
	
	public override void _Ready()
	{
		base._Ready();
		GD.Randomize();
		
		_aStar = new AStar2D();
		_initialPosition = GlobalPosition;
		
		_idleTimer = GetNode<Timer>("IdleTimer");
		
		if (_wander)
		{
			_collisionTileMap = GetNode<TileMap>(_collisionTileMapPath);
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
		foreach (var id in _aStar.GetPoints())
		{
			var pos = _aStar.GetPointPosition((int) id);
			if (pos == position) return (int) id;
		}
		
		return -1;
	}
	
	private void MapWorld()
	{
		var usedCells = new Array<Vector2>(_collisionTileMap.GetUsedCells());
		var gridSize = Global.Manager.GridSize;
		var radius = (int) (_wanderRadius / gridSize);
		var index = 0;
		
		for (var y = -radius; y <= radius; ++y)
		{
			for (var x = -radius; x <= radius; ++x)
			{
				var pos = new Vector2(GlobalPosition.x + (x * gridSize), GlobalPosition.y + (y * gridSize));
				var gridPos = new Vector2((int) (GlobalPosition.x / gridSize) + x, (int) (GlobalPosition.y / gridSize) + y);
				
				if (!usedCells.Contains(gridPos))
				{
					// Adding the point
					_aStar.AddPoint(index, pos);
					
					// Connecting the point to its neighbors (left and top)
					if (_aStar.HasPoint(index - 1)) _aStar.ConnectPoints(index, index - 1);
					var up = GetIdFromPosition(pos - new Vector2(0f, gridSize));
					if (_aStar.HasPoint(up)) _aStar.ConnectPoints(index, up);
				}
				index++;
			}
		}
	}
	
	private Vector2 GetRandomPosition()
	{
		var gridSize = Global.Manager.GridSize;
		var position = _initialPosition + new Vector2((int) GD.RandRange(-_wanderRadius, _wanderRadius), (int) GD.RandRange(-_wanderRadius, _wanderRadius));
		var inGridPosition = new Vector2(((int) position.x / gridSize) * gridSize, ((int) position.y / gridSize) * gridSize);
		
		return inGridPosition;
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
			{ "WanderRadius", _wanderRadius },
			{ "WanderTime", _wanderTime },
		};
		return saveDict;
	}
	
	public void Load(Dictionary infoDict)
	{
		_direction = new Vector2((float) infoDict["DirX"], (float) infoDict["DirY"]);
		_wander = (bool) infoDict["Wander"];
		_wanderRadius = (int) infoDict["WanderRadius"];
		_wanderTime = (float) infoDict["WanderTime"];
	}
}
