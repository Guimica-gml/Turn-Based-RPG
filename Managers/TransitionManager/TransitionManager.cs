using Godot;
using Godot.Collections;

public class TransitionManager : Node
{
	[Signal] private delegate void TransitionTriggered();
	[Signal] private delegate void SceneChanged();
	[Signal] private delegate void TransitionEnded();
	
	public bool Active = false;
	private TransitionEffect _transitionEffect;
	
	private string _scenePath = "";
	private string _sceneEntryIndetifier = "";
	private PackedScene _playerPackedScene = null;
	
	private PackedScene DialogBoxPreload = GD.Load<PackedScene>("res://DialogBox/DialogBox.tscn");
	
	public override void _Ready()
	{
		PackedScene sceneTransitionPacked = GD.Load<PackedScene>("res://SceneTransition/TransitionEffect.tscn");
		
		_transitionEffect = sceneTransitionPacked.Instance<TransitionEffect>();
		AddChild(_transitionEffect);
		
		_transitionEffect.Connect("EffectTransition", this, nameof(OnEffectTransition));
		_transitionEffect.Connect("EffectStarted", this, nameof(OnEffectStarted));
		_transitionEffect.Connect("EffectEnded", this, nameof(OnEffectEnded));
	}
	
	public void ChangeSceneTo(string scenePath, string sceneEntryIndetifier = "none", TransitionTypes transitionType = TransitionTypes.Default)
	{
		if (Active)
		{
			GD.PrintErr("Trying to start a transition while another transition is already happening");
			return;
		}
		
		_scenePath = scenePath;
		_sceneEntryIndetifier = sceneEntryIndetifier;
		
		_transitionEffect.StartEffect(transitionType);
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
		
		// Searchs for the correct entry to spawn the player at
		foreach (SceneEntry sceneEntry in sceneEntriesList)
		{
			if (sceneEntry.Indentifier != sceneEntryIndetifier) continue;
			var player = CreatePlayer(sceneEntry.GlobalPosition, packedPlayer);
			Global.Manager.GetYSort().AddChild(player);
			return;
		}
		
		GD.PrintErr($"TransitionManager was not able to find a SceneEntry with the indentifier `{sceneEntryIndetifier}`");
	}
	
	private void OnEffectStarted()
	{
		Active = true;
		
		// Saving player information if it exists
		var player = GetTree().CurrentScene.FindNode("Player", true, false) as Player;
		_playerPackedScene = null;
		if (player != null)
		{
			_playerPackedScene = new PackedScene();
			_playerPackedScene.Pack(player);
		}
		
		EmitSignal(nameof(TransitionTriggered));
	}
	
	private async void OnEffectTransition()
	{
		GetTree().ChangeScene(_scenePath);
		await ToSignal(GetTree(), "idle_frame");
		
		// Placing the player in the new scene
		PlacePlayer(_sceneEntryIndetifier, _playerPackedScene);
		
		EmitSignal(nameof(SceneChanged));
	}
	
	private void OnEffectEnded()
	{
		Active = false;
		EmitSignal(nameof(TransitionEnded));
	}
}
