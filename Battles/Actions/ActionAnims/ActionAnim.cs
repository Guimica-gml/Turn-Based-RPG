using Godot;

public class ActionAnim : Node2D
{
	[Signal] private delegate void ApplyAction();
	[Signal] private delegate void Ended();
	
	private AnimationPlayer _animationPlayer;
	
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		
		if (!_animationPlayer.HasAnimation("Default"))
		{
			GD.PrintErr("All ActionAnims should be named `Default`");
			return;
		}
		
		_animationPlayer.Play("Default");
	}
	
	private void OnAnimationPlayerAnimationFinished(string animName)
	{
		EmitSignal(nameof(Ended));
		QueueFree();
	}
}
