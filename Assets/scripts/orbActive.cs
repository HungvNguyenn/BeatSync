using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class orbActive : MonoBehaviour
{
    private XRSimpleInteractable interactable;
    private OrbLifetime lifetime;

    private bool hit = false;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        lifetime = GetComponent<OrbLifetime>();

        interactable.selectEntered.AddListener(OnHit);
    }

    void OnHit(SelectEnterEventArgs args)
    {
        if (hit) return;
        hit = true;

        lifetime.Hit();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnHit);
    }
}