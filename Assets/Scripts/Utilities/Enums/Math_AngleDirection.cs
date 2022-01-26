public enum AngleDirection
{
	Forward = 0,
	ForwardRight = 45,
	Right = 90,
	BackwardRight = 135,
	Backward = 180,
	BackwardLeft = -135,
	Left = -90,
	ForwardLeft = -45
}

public class MathExtensions
{
	public static AngleDirection GetAngleDirection(float angle)
	{
		return (AngleDirection)(UnityEngine.Mathf.Round(angle / 45) * 45f);
	}
}