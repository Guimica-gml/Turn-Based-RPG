using Godot;

public class BattleCharacterDisplayer : Node2D
{
	[Signal] private delegate void AnimEnded();
	
	private Stats _stats = null;
	[Export] public Stats Stats
	{
		get => _stats;
		set
		{
			_stats = value;
			UpdateInfo();
		}
	}
	
	private Action _action;
	private Stats _userStats;
	
	private Sprite _sprite;
	private Node2D _scalePivot;
	
	public bool Active { get; private set; } = false;
	
	public override void _Ready()
	{
		_sprite = GetNode<Sprite>("ScalePivot/Sprite");
		_scalePivot = GetNode<Node2D>("ScalePivot");
	}
	
	public void ApplyAction(Action action, Stats userStats = null)
	{
		if (_action != null)
		{
			GD.PrintErr("Trying to start an animation while another animation is already happening");
			return;
		}
		
		var animScenePacked = GD.Load<PackedScene>(action.AnimScenePath);
		_action = action;
		_userStats = userStats;
		
		var anim = animScenePacked.Instance<ActionAnim>();
		anim.Position += new Vector2(0f, -20f);
		anim.Connect("ApplyAction", this, nameof(ApplyActionEffect));
		anim.Connect("Ended", this, nameof(OnAnimEnded));
		_scalePivot.AddChild(anim);
		
		Active = true;
	}
	
	private void ApplyActionEffect()
	{
		if (_action == null)
		{
			GD.PrintErr("Trying to apply an action effect without having an action");
			return;
		}
		
		if (_action.Heal)
		{
			Stats.Heal(_action.Value);
		}
		else
		{
			_userStats.AttackTarget(Stats, _action);
		}
	}
	
	private void OnAnimEnded()
	{
		Active = false;
		EmitSignal(nameof(AnimEnded));
		_userStats = null;
		_action = null;
	}
	
	private void UpdateInfo()
	{
		if (Stats == null)
		{
			GD.PrintErr("Stats was not defined in StatsInfoDisplayer.cs");
			return;
		}
		
		_sprite.Texture = Stats.SpriteSheet;
	}
}
