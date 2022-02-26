using Godot;

public class TransitionInfo : Resource
{
	[Export] public float SmoothSize = 0;
	[Export] public string TexturePath = "";
	
	public TransitionInfo(float smoothSize, string texturePath)
	{
		SmoothSize = smoothSize;
		TexturePath = texturePath;
	}
	
	public override string ToString()
	{
		return $"[SmoothSize: {SmoothSize}, TexturePath: {TexturePath}]";
	}
}
