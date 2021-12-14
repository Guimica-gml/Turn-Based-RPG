using Godot;

public class Global : Node
{
	public static Manager Manager { get; private set; }
	public static InteractionManager InteractionManager { get; private set; }
	public static TransitionManager TransitionManager { get; private set; }
	public static DialogManager DialogManager { get; private set; }
	public static BattleManager BattleManager { get; private set; }
	public static PopupManager PopupManager { get; private set; }
	public static ShopManager ShopManager { get; private set; }
	
	public override void _Ready()
	{
		InteractionManager = GetTree().Root.GetNode<InteractionManager>("InteractionManager");
		TransitionManager = GetTree().Root.GetNode<TransitionManager>("TransitionManager");
		DialogManager = GetTree().Root.GetNode<DialogManager>("DialogManager");
		BattleManager = GetTree().Root.GetNode<BattleManager>("BattleManager");
		PopupManager = GetTree().Root.GetNode<PopupManager>("PopupManager");
		ShopManager = GetTree().Root.GetNode<ShopManager>("ShopManager");
		Manager = GetTree().Root.GetNode<Manager>("Manager");
	}
}
