using Godot;

public class Popup : PanelContainer
{
	private AnimationPlayer _animationPlayer;
	
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
	
	private void OnTimerTimeout()
	{
		_animationPlayer.Play("FadeOut");
	}
}
