using Godot;
using Godot.Collections;

public class Global : Node
{
	private static Manager _manager;
	public static Manager Manager {
		get => _manager;
		private set
		{
			_manager = value;
			if (Manager != null)
			{
				foreach (var res in _uninitializedRes)
				{
					Manager.InitResource(res);
				}
			}
		}
	}

	public static InteractionManager InteractionManager { get; private set; }
	public static TransitionManager TransitionManager { get; private set; }
	public static DialogManager DialogManager { get; private set; }
	public static BattleManager BattleManager { get; private set; }
	public static PopupManager PopupManager { get; private set; }
	public static ShopManager ShopManager { get; private set; }

	private static Array<MyResource> _uninitializedRes = new Array<MyResource>();

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

	public static void InitResource(MyResource res)
	{
		if (Manager != null)
		{
			Manager.InitResource(res, true);
		}
		else
		{
			_uninitializedRes.Add(res);
		}
	}
}
