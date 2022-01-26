using UnityEngine;

public class SpriteHover : MonoBehaviour
{
	float originalY;

	public float floatStrength = 1;
	public float floatSpeed = 1;

	private float phase = 0;

	void Start()
	{
		this.originalY = this.transform.localPosition.y;
	}

	void Update()
	{
		if (gameObject.activeSelf)
		{
			phase += Time.deltaTime;
			transform.localPosition = new Vector3(transform.localPosition.x,
			originalY + ((float)Mathf.Sin(phase * floatSpeed) * floatStrength),
			transform.localPosition.z);
		}
		else
		{
			phase = 0;
			transform.localPosition = new Vector3(transform.localPosition.x, originalY, transform.localPosition.z);
		}
	}
}
