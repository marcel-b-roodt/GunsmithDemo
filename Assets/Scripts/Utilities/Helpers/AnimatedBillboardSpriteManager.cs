using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class AnimatedBillboardSpriteManager : MonoBehaviour
{
	public int directions = 8;
	public bool MirrorLeft = true;

	private Camera mainCamera;

	public Func<bool> KeyFrameAction;

	private Animator m_Anim;
	private SpriteRenderer m_SpriteRenderer;
	private float minMirrorAngle = 0;
	private float maxMirrorAngle = 0;

	private const string ViewAngle = "ViewAngle";

	private void Start()
	{
		m_Anim = this.GetComponent<Animator>();
		m_SpriteRenderer = this.GetComponent<SpriteRenderer>();
		if (directions <= 0)
		{
			directions = 1;
		}
		minMirrorAngle = (360 / directions) / 2;
		maxMirrorAngle = 180 - minMirrorAngle;
	}

	private void Update()
	{
		Vector3 viewDirection = -new Vector3(GlobalReferences.MainCamera.transform.forward.x, 0, GlobalReferences.MainCamera.transform.forward.z);
		transform.LookAt(transform.position + viewDirection);
		m_Anim.SetFloat(ViewAngle, transform.localEulerAngles.y);
		if (MirrorLeft)
		{
			m_SpriteRenderer.flipX = !(transform.localEulerAngles.y >= minMirrorAngle && transform.localEulerAngles.y <= maxMirrorAngle);
		}
	}

	public void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			SceneView sceneView = GetActiveSceneView();
			if (sceneView)
			{
				// Editor camera stands in for player camera in edit mode
				Vector3 viewDirection = -new Vector3(sceneView.camera.transform.forward.x, 0, sceneView.camera.transform.forward.z);
				transform.LookAt(transform.position + viewDirection);
			}
		}

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.parent.position, transform.parent.forward * 2);

	}

	private SceneView GetActiveSceneView()
	{
		// Return the focused window if it is a SceneView
		if (EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.GetType() == typeof(SceneView))
			return (SceneView)EditorWindow.focusedWindow;

		return null;
	}
}
