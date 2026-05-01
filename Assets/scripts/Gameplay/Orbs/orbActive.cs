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

        RecognizeMovement(args);

        hit = true;
        lifetime.Hit();
        Destroy(gameObject);
    }

    void RecognizeMovement(SelectEnterEventArgs args)
    {
        if (args.interactorObject is not Component interactorComponent)
            return;

        HandMovementTracker tracker = interactorComponent.GetComponentInParent<HandMovementTracker>();
        if (tracker == null)
            tracker = interactorComponent.GetComponentInChildren<HandMovementTracker>();

        if (tracker == null)
        {
            Debug.LogWarning("No HandMovementTracker found on interactor. Tap movement was not recognized.");
            return;
        }

        HandMovementType measuredMovement = tracker.RecognizeTapMovement(out Vector3 localDelta, out float distance);
        Debug.Log($"Tap movement recognized: {measuredMovement} | local delta {localDelta} | distance {distance:F2}");
    }

    void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnHit);
    }
}
