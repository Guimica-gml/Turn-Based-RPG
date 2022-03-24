using Godot;

public abstract class Entity : KinematicBody2D
{
	[Export] protected float _speed = 1f;

	public bool CanMove = true;
	protected bool _moving = false;

	protected Vector2 _moveVector = Vector2.Zero;

	protected AnimationTree _animationTree;
	protected AnimationNodeStateMachinePlayback _animationTreePlayback;
	protected Sprite _sprite;
	protected RayCast2D _rayCast;

	protected abstract Vector2 CheckForInput();

	public override void _Ready()
	{
		_animationTree = GetNode<AnimationTree>("AnimationTree");
		_animationTreePlayback = _animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
		_sprite = GetNode<Sprite>("Sprite");
		_rayCast = GetNode<RayCast2D>("RayCast2D");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!_moving && CanMove) _moveVector = CheckForInput();

		if (_moveVector != Vector2.Zero)
		{
			UpdateDirection(_moveVector);
			if (CheckCollision())
			{
				_animationTreePlayback.Travel("Idle");
				return;
			}
			_moving = true;
		}

		_animationTreePlayback.Travel((_moveVector == Vector2.Zero) ? "Idle" : "Run");

		if (!_moving) return;
		GlobalPosition += _moveVector * _speed;

		if (InGrid())
		{
			_moving = false;
			_moveVector = Vector2.Zero;
		}
	}

	protected void UpdateDirection(Vector2 direction)
	{
		_rayCast.CastTo = direction * (Global.Manager.GridSize / 2);
		_animationTree.Set("parameters/Idle/blend_position", direction);
		_animationTree.Set("parameters/Run/blend_position", direction);
	}

	protected bool CheckCollision()
	{
		_rayCast.ForceRaycastUpdate();
		return _rayCast.IsColliding();
	}

	protected bool InGrid()
	{
		var gridSize = Global.Manager.GridSize;
		return (GlobalPosition.x % gridSize == 0 && GlobalPosition.y % gridSize == 0);
	}
}
