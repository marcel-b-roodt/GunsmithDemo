using UnityEngine;

namespace Helpers
{
	public static class DebugExtensions
	{
		public static void DebugDirectionRay(this Transform transform)
		{
			Debug.DrawRay(transform.position, transform.forward, Color.red, 0.1f);
		}
	}
}