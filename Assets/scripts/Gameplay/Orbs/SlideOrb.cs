using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SlideOrb : MonoBehaviour
{
    private OrbLifetime lifetime;
    private XRGrabInteractable grabInteractable;
    private MeshRenderer meshRenderer;
    private Collider grabCollider;
    private SlideVisual visual;
    private Collider endGoalCollider;
    private IXRSelectInteractor activeInteractor;
    private Transform activeTrackingTransform;

    private bool started = false;
    private bool completed = false;
    private float requiredHoldDuration = 0.5f;
    private float completionGraceSeconds = 0.2f;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 pathDirection;
    private float pathLength = 0f;
    private float travelElapsed = 0f;
    private float draggedProgress = 0f;
    private float grabDistanceOffset = 0f;

    void Awake()
    {
        lifetime = GetComponent<OrbLifetime>();
        meshRenderer = GetComponent<MeshRenderer>();
        grabCollider = GetComponent<Collider>();
        visual = GetComponent<SlideVisual>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            XRSimpleInteractable existingSimpleInteractable = GetComponent<XRSimpleInteractable>();
            if (existingSimpleInteractable != null)
                Destroy(existingSimpleInteractable);

            grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
        }

        if (grabCollider != null)
            grabCollider.isTrigger = false;

        if (meshRenderer != null)
            meshRenderer.enabled = true;

        grabInteractable.trackPosition = false;
        grabInteractable.trackRotation = false;
        grabInteractable.throwOnDetach = false;
        grabInteractable.selectEntered.AddListener(OnGrabStarted);
        grabInteractable.selectExited.AddListener(OnGrabEnded);
    }

    public void ConfigurePath(Vector3 start, Vector3 end, float holdDuration, float graceSeconds)
    {
        startPosition = start;
        endPosition = end;
        requiredHoldDuration = Mathf.Max(0.1f, holdDuration);
        completionGraceSeconds = Mathf.Max(0.05f, graceSeconds);
        pathLength = Vector3.Distance(startPosition, endPosition);
        pathDirection = pathLength > 0.001f ? (endPosition - startPosition).normalized : Vector3.right;
        travelElapsed = 0f;
        draggedProgress = 0f;
        grabDistanceOffset = 0f;
        started = false;
        completed = false;
        activeInteractor = null;
        activeTrackingTransform = null;
        endGoalCollider = visual != null && visual.endMarker != null ? visual.endMarker.GetComponent<Collider>() : null;

        if (grabInteractable != null)
            grabInteractable.enabled = true;

        transform.position = startPosition;
        transform.rotation = Quaternion.identity;

        if (lifetime == null)
            lifetime = GetComponent<OrbLifetime>();

        if (lifetime != null)
        {
            lifetime.missMessage = "Slider Miss";
            lifetime.SetRemainingLifetime(requiredHoldDuration + completionGraceSeconds);
        }
    }

    void Update()
    {
        if (!started || completed || activeInteractor == null || pathLength <= 0.001f)
            return;

        Transform trackingTransform = GetActiveTrackingTransform();
        if (trackingTransform == null)
            return;

        travelElapsed += Time.deltaTime;

        float trackingDistance = Vector3.Dot(trackingTransform.position - startPosition, pathDirection) + grabDistanceOffset;
        float clampedDistance = Mathf.Clamp(trackingDistance, 0f, pathLength);
        draggedProgress = clampedDistance / pathLength;
        transform.position = startPosition + pathDirection * clampedDistance;

        if (travelElapsed >= requiredHoldDuration && lifetime != null)
        {
            ReleaseGrabSelection();
            lifetime.ForceMiss();
        }
    }

    void OnGrabStarted(SelectEnterEventArgs args)
    {
        activeInteractor = args.interactorObject;
        activeTrackingTransform = GetActiveTrackingTransform();

        if (activeTrackingTransform == null)
            return;

        started = true;
        completed = false;
        travelElapsed = 0f;
        draggedProgress = 0f;

        float currentDistance = Vector3.Dot(transform.position - startPosition, pathDirection);
        float interactorDistance = Vector3.Dot(activeTrackingTransform.position - startPosition, pathDirection);
        grabDistanceOffset = currentDistance - interactorDistance;

        if (lifetime != null)
            lifetime.SetRemainingLifetime(requiredHoldDuration + completionGraceSeconds);
    }

    void OnGrabEnded(SelectExitEventArgs args)
    {
        bool shouldMiss = started && !completed;
        activeInteractor = null;
        activeTrackingTransform = null;

        if (shouldMiss && lifetime != null)
            lifetime.ForceMiss();
    }

    void OnTriggerEnter(Collider other)
    {
        if (completed || !started || activeInteractor == null || other == null)
            return;

        if (other != endGoalCollider)
            return;

        completed = true;
        Debug.Log("Slider Hit");
        ReleaseGrabSelection();

        if (grabInteractable != null)
            grabInteractable.enabled = false;

        if (lifetime != null)
            lifetime.Hit();
    }

    void ReleaseGrabSelection()
    {
        if (grabInteractable != null && grabInteractable.interactionManager != null && grabInteractable.isSelected)
            grabInteractable.interactionManager.CancelInteractableSelection((IXRSelectInteractable)grabInteractable);

        activeInteractor = null;
        activeTrackingTransform = null;
    }

    Transform GetActiveTrackingTransform()
    {
        if (activeInteractor == null)
            return null;

        if (activeInteractor is Component interactorComponent)
        {
            activeTrackingTransform = interactorComponent.transform;
            return activeTrackingTransform;
        }

        activeTrackingTransform = activeInteractor.GetAttachTransform(grabInteractable);
        return activeTrackingTransform;
    }

    void OnDestroy()
    {
        if (grabInteractable == null)
            return;

        grabInteractable.selectEntered.RemoveListener(OnGrabStarted);
        grabInteractable.selectExited.RemoveListener(OnGrabEnded);
    }
}
