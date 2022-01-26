using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private void Awake()
    {
        gameObject.tag = Helpers.Tags.Interactable;
        gameObject.layer = LayerMask.NameToLayer(Helpers.Layers.Interactable);
    }

    public abstract void Activate();
}