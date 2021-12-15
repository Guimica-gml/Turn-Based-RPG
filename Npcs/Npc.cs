using Godot;
using Godot.Collections;
using System;

public class Npc : Entity
{
	[Export] private Vector2 _direction = Vector2.Zero;
	[Export] private bool _wander = false;
	[Export] private int _wanderRadius = 0;
	[Export] private float _wanderTime = 0;
	
	private Vector2 _initialPosition;
	private Vector2 _targetPosition;
	private Random _randomGenerator;
	
	private Timer _timer;
	private Timer _idleTimer;
	
	public override void _Ready()
	{
		base._Ready();
		GD.Randomize();
		
		_initialPosition = GlobalPosition;
		_randomGenerator = new Random();
		
		_timer = GetNode<Timer>("Timer");
		_idleTimer = GetNode<Timer>("IdleTimer");
		
		if (_wander)
		{
			_targetPosition = GetRandomPosition();
			_timer.Start(_wanderTime);
		}
		
		UpdateDirection(_direction);
	}
	
	protected override Vector2 CheckForInput()
	{
		if (!_wander) return Vector2.Zero;
		
		if (GlobalPosition.x < _targetPosition.x) return Vector2.Right;
		if (GlobalPosition.x > _targetPosition.x) return Vector2.Left;
		if (GlobalPosition.y > _targetPosition.y) return Vector2.Up;
		if (GlobalPosition.y < _targetPosition.y) return Vector2.Down;
		
		return Vector2.Zero;
	}
	
	protected override void OnCollisionAhead(Godot.Object collider)
	{
		return;
	}
	
	private Vector2 GetRandomPosition()
	{
		var gridSize = Global.Manager.GridSize;
		var position = _initialPosition + new Vector2(_randomGenerator.Next(-_wanderRadius, _wanderRadius), _randomGenerator.Next(-_wanderRadius, _wanderRadius));
		var inGridPosition = new Vector2(((int) position.x / gridSize) * gridSize, ((int) position.y / gridSize) * gridSize);
		
		return inGridPosition;
	}
	
	static float NextFloat(Random random, float minValue, float maxValue)
	{
		var result = (random.NextDouble() * (maxValue - minValue)) + minValue;
		return (float)result;
	}
	
	private void OnTimerTimeout()
	{
		_idleTimer.Start(NextFloat(_randomGenerator, 0.5f, 1.2f));
	}
	
	private void OnIdleTimerTimeout()
	{
		_targetPosition = GetRandomPosition();
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
