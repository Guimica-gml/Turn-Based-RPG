using Godot;
using Godot.Collections;

public enum TransitionTypes
{
	Default,
	Courtain,
	FromCenter
}

public class TransitionEffect : Control
{
	[Signal] private delegate void EffectStarted();
	[Signal] private delegate void EffectTransition();
	[Signal] private delegate void EffectEnded();

	public bool Active { get; private set; } = false;
	private Tween _tween;

	private Dictionary<TransitionTypes, TransitionInfo> _transitionsInfo = new Dictionary<TransitionTypes, TransitionInfo>()
	{
		{ TransitionTypes.Default, new TransitionInfo(1f, "res://SceneTransition/Masks/Default.png") },
		{ TransitionTypes.Courtain, new TransitionInfo(0.25f, "res://SceneTransition/Masks/Courtains.png") },
		{ TransitionTypes.FromCenter, new TransitionInfo(0.25f, "res://SceneTransition/Masks/FromCenter.png") },
	};

	public override void _Ready()
	{
		_tween = GetNode<Tween>("Tween");
	}

	public async void StartEffect(TransitionTypes transitionType = TransitionTypes.Default)
	{
		if (Active)
		{
			GD.PrintErr("Trying to start an effect while TransitionEffect is already active");
			return;
		}
		Active = true;

		// Updating the transition type
		var info = _transitionsInfo[transitionType];
		Material.Set("shader_param/smoothsize", info.SmoothSize);
		Material.Set("shader_param/mask", GD.Load<Texture>(info.TexturePath));

		// Transition
		EmitSignal(nameof(EffectStarted));
		StartTween(1f, 0f);

		await ToSignal(_tween, "tween_completed");
		EmitSignal(nameof(EffectTransition));

		await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
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
