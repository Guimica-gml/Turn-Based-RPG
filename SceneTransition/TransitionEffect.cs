using Godot;
using Godot.Collections;

public class TransitionEffect : Control
{
	public enum Types { Default, Courtain, FromCenter }
	
	[Signal] private delegate void EffectStarted();
	[Signal] private delegate void EffectTransition();
	[Signal] private delegate void EffectEnded();
	
	public bool Active { get; private set; }
	private Tween _tween;
	
	private Dictionary<Types, TransitionInfo> _transitionsInfo = new Dictionary<Types, TransitionInfo>()
	{
		{ Types.Default, new TransitionInfo(1f, "res://SceneTransition/Masks/Default.png") },
		{ Types.Courtain, new TransitionInfo(0.25f, "res://SceneTransition/Masks/Courtains.png") },
		{ Types.FromCenter, new TransitionInfo(0.25f, "res://SceneTransition/Masks/FromCenter.png") },
	};
	
	public override void _Ready()
	{
		_tween = GetNode<Tween>("Tween");
	}
	
	public async void ChangeSceneTo(string scenePath, string sceneEntryIndetifier = "none", Types transitionType = Types.Default)
	{
		if (Active)
		{
			GD.Print("ERROR: SceneTransition.cs - Line: 29");
			return;
		}
		Active = true;
		
		// Updating the transition type
		var info = _transitionsInfo[transitionType];
		Material.Set("shader_param/smoothsize", info.SmoothSize);
		Material.Set("shader_param/mask", GD.Load<Texture>(info.TexturePath));
		
		// Transtion
		
		EmitSignal(nameof(EffectStarted));
		StartTween(1f, 0f);
		
		await ToSignal(_tween, "tween_completed");
		EmitSignal(nameof(EffectTransition));
		
		await ToSignal(GetTree().CreateTimer(0.25f), "timeout");
		StartTween(0f, 1f);
		
		await ToSignal(_tween, "tween_completed");
		EmitSignal(nameof(EffectEnded));
		
		Active = false;
	}
	
	private void StartTween(float initialValue, float finalValue)
	{
		_tween.InterpolateProperty(Material, "shader_param/cutoff", initialValue, finalValue, 0.5f);
		_tween.Start();
	}
}
