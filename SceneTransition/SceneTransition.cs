using Godot;
using Godot.Collections;

public class SceneTransition : Control
{
	[Signal] private delegate void TransitionTriggered();
	[Signal] private delegate void SceneChanged();
	[Signal] private delegate void TransitionEnded();
	
	public bool Active { get; private set; }
	private Tween _tween;
	
	struct TransitionInfo
	{
		public float SmoothSize;
		public string TexturePath;
		
		public TransitionInfo(float smoothSize, string texturePath)
		{
			SmoothSize = smoothSize;
			TexturePath = texturePath;
		}
	}
	
	// ... Yes, it needs to be done this way
	private System.Collections.Generic.Dictionary<TransitionManager.Types, TransitionInfo> _transitionsInfo = new System.Collections.Generic.Dictionary<TransitionManager.Types, TransitionInfo>();
	
	public override void _Ready()
	{
		_transitionsInfo.Add(TransitionManager.Types.Default, new TransitionInfo(1f, "res://SceneTransition/Masks/Default.png"));
		_transitionsInfo.Add(TransitionManager.Types.Courtain, new TransitionInfo(0.25f, "res://SceneTransition/Masks/Courtains.png"));
		_transitionsInfo.Add(TransitionManager.Types.FromCenter, new TransitionInfo(0.25f, "res://SceneTransition/Masks/FromCenter.png"));
		
		_tween = GetNode<Tween>("Tween");
	}
	
	public async void ChangeSceneTo(string scenePath, string sceneEntryIndetifier = "none", TransitionManager.Types transitionType = TransitionManager.Types.Default)
	{
		if (Active) return;
		Active = true;
		
		// Updating the transition type
		var info = _transitionsInfo[transitionType];
		Material.Set("shader_param/smoothsize", info.SmoothSize);
		Material.Set("shader_param/mask", GD.Load<Texture>(info.TexturePath));
		
		// Transtion
		Player player = GetTree().CurrentScene.FindNode("Player", true, false) as Player;
		PackedScene packedPlayer = null;
		if (player != null)
		{
			packedPlayer = new PackedScene();
			packedPlayer.Pack(player);
		}
		
		EmitSignal(nameof(TransitionTriggered));
		StartTween(1f, 0f);
		await ToSignal(_tween, "tween_completed");
		GetTree().ChangeScene(scenePath);
		
		await ToSignal(GetTree().CreateTimer(0.25f), "timeout");
		
		await ToSignal(GetTree(), "idle_frame");
		PlacePlayer(sceneEntryIndetifier, packedPlayer);
		EmitSignal(nameof(SceneChanged));
		
		StartTween(0f, 1f);
		await ToSignal(_tween, "tween_completed");
		EmitSignal(nameof(TransitionEnded));
		
		Active = false;
	}
	
	private Player CreatePlayer(Vector2 position, PackedScene packedPlayer = null)
	{
		if (packedPlayer == null) packedPlayer = GD.Load<PackedScene>("res://Player/Player.tscn");
		
		var player = packedPlayer.Instance<Player>();
		player.CanMove = false;
		player.GlobalPosition = position;
		player.CallDeferred("SetCamera", Global.Manager.GetMainCamera());
		return player;
	}
	
	private void PlacePlayer(string sceneEntryIndetifier, PackedScene packedPlayer)
	{
		var sceneEntriesList = new Array<SceneEntry>(GetTree().GetNodesInGroup("SceneEntry"));
		if (sceneEntryIndetifier == "none") return;
		
		foreach (SceneEntry sceneEntry in sceneEntriesList)
		{
			if (sceneEntry.Indentifier != sceneEntryIndetifier) continue;
			var player = CreatePlayer(sceneEntry.GlobalPosition, packedPlayer);
			Global.Manager.GetYSort().AddChild(player);
			return;
		}
		
		GD.PrintErr($"SceneTransition was not able to find a SceneEntry with the indentifier `{sceneEntryIndetifier}`");
	}
	
	private void StartTween(float initialValue, float finalValue)
	{
		_tween.InterpolateProperty(Material, "shader_param/cutoff", initialValue, finalValue, 0.5f);
		_tween.Start();
	}
}
