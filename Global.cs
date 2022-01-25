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
		var root = GetTree().Root;
		
		InteractionManager = root.GetNode<InteractionManager>("InteractionManager");
		TransitionManager = root.GetNode<TransitionManager>("TransitionManager");
		DialogManager = root.GetNode<DialogManager>("DialogManager");
		BattleManager = root.GetNode<BattleManager>("BattleManager");
		PopupManager = root.GetNode<PopupManager>("PopupManager");
		ShopManager = root.GetNode<ShopManager>("ShopManager");
		Manager = root.GetNode<Manager>("Manager");
	}
}
