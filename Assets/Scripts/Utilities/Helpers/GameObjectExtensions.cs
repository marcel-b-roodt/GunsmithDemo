using UnityEngine;

namespace Helpers
{
	public static class GameObjectExtensions
	{
		public static GameObject FindGameObjectInChildren(this GameObject gameObject, string gameObjectName)
		{
			Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
			foreach (Transform item in children)
			{
				if (item.name == gameObjectName)
				{
					return item.gameObject;
				}
			}

			return null;
		}

		public static GameObject FindTaggedGameObjectInChildren(this GameObject gameObject, string tag)
		{
			Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
			foreach (Transform item in children)
			{
				if (item.tag == tag)
				{
					return item.gameObject;
				}
			}

			return null;
		}
	}
}