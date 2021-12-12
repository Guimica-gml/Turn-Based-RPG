using Godot;

public class Global : Node
{
	public static Manager Manager;
	public static InteractionManager InteractionManager;
	public static TransitionManager TransitionManager;
	public static DialogManager DialogManager;
	public static BattleManager BattleManager;
	public static PopupManager PopupManager;
	public static ShopManager ShopManager;
	
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
