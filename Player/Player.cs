using Godot;
using Godot.Collections;

public class Player : Entity
{
	private Sprite _interactionSprite;
	private RemoteTransform2D _remoteTransform;
	private RayCast2D _interactionRayCast;

	private readonly Dictionary<string, Vector2> _acceptedInputs = new Dictionary<string, Vector2>()
	{
		{ "ui_up", Vector2.Up },
		{ "ui_down", Vector2.Down },
		{ "ui_right", Vector2.Right },
		{ "ui_left", Vector2.Left },
	};

	private Array<string> _inputKeys = new Array<string>();

	public override void _Ready()
	{
		base._Ready();

		_inputKeys = _acceptedInputs.Keys as Array<string>;

		_remoteTransform = GetNode<RemoteTransform2D>("CameraRemote");
		_interactionRayCast = GetNode<RayCast2D>("InteractionRayCast2D");
		_interactionSprite = GetNode<Sprite>("InteractionSprite");

		Global.InteractionManager.Connect("InteractionTriggered", this, nameof(DisableMovement));
		Global.InteractionManager.Connect("InteractionEnded", this, nameof(EnableMovement));
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);

		_interactionRayCast.CastTo = _rayCast.CastTo;
		_interactionRayCast.ForceRaycastUpdate();
		var interactable = _interactionRayCast.GetCollider() as InteractableArea;

		_interactionSprite.Texture = null;
		if (interactable == null || !interactable.CanTrigger() || !CanMove) return;
		_interactionSprite.Texture = interactable.GetIcon();

		if (!Input.IsActionJustPressed("ui_interact")) return;
		interactable.Interact();
	}

	protected override Vector2 CheckForInput()
	{
		for (var i = 0; i < _inputKeys.Count; ++i)
		{
			var item = _inputKeys[i];
			if (Input.IsActionPressed(item)) return _acceptedInputs[item];
		}

		return Vector2.Zero;
	}

	public void SetCamera(MainCamera camera)
	{
		_remoteTransform.RemotePath = _remoteTransform.GetPathTo(camera);
	}

	private void DisableMovement(Interaction interaction)
	{
		CanMove = false;
	}

	private void EnableMovement(Interaction interaction)
	{
		CanMove = true;
	}
}
