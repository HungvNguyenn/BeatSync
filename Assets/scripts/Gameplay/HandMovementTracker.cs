using System.Collections.Generic;
using UnityEngine;

public class HandMovementTracker : MonoBehaviour
{
    [System.Serializable]
    struct MovementSample
    {
        public float time;
        public Vector3 position;
    }

    public Transform trackingTarget;
    public Transform referenceSpace;
    [Min(0.05f)] public float sampleWindowSeconds = 0.18f;
    [Min(0f)] public float minGestureDistance = 0.12f;
    [Min(0f)] public float minGestureVelocity = 0.75f;
    [Min(0f)] public float minTapTravelDistance = 0.08f;
    [Min(0f)] public float minSlideTravelDistance = 0.15f;

    readonly Queue<MovementSample> samples = new Queue<MovementSample>();
    HandMovementType currentMovement = HandMovementType.Still;
    Vector3 currentWorldDelta = Vector3.zero;
    Vector3 currentLocalDelta = Vector3.zero;
    float currentVelocity = 0f;
    bool hasTapAnchor = false;
    Vector3 lastTapAnchorWorld = Vector3.zero;
    bool slideTrackingActive = false;
    Vector3 slideStartWorld = Vector3.zero;
    HandMovementType lastRecognizedTapMovement = HandMovementType.Unknown;

    public HandMovementType CurrentMovement => currentMovement;
    public Vector3 CurrentWorldDelta => currentWorldDelta;
    public Vector3 CurrentLocalDelta => currentLocalDelta;
    public float CurrentVelocity => currentVelocity;
    public HandMovementType LastRecognizedTapMovement => lastRecognizedTapMovement;

    void Awake()
    {
        if (trackingTarget == null)
            trackingTarget = transform;
    }

    void Update()
    {
        if (trackingTarget == null)
            return;

        float now = Time.time;
        samples.Enqueue(new MovementSample
        {
            time = now,
            position = trackingTarget.position
        });

        while (samples.Count > 1 && now - samples.Peek().time > sampleWindowSeconds)
            samples.Dequeue();

        EvaluateMovement();
    }

    public bool Matches(HandMovementType expectedMovement)
    {
        if (expectedMovement == HandMovementType.Unknown)
            return true;

        if (expectedMovement == HandMovementType.Still)
            return currentMovement == HandMovementType.Still;

        return currentMovement == expectedMovement;
    }

    public HandMovementType RecognizeTapMovement(out Vector3 localDelta, out float distance)
    {
        localDelta = Vector3.zero;
        distance = 0f;

        if (trackingTarget == null)
            return HandMovementType.Unknown;

        Vector3 currentPosition = trackingTarget.position;

        if (!hasTapAnchor)
        {
            lastTapAnchorWorld = currentPosition;
            hasTapAnchor = true;
            lastRecognizedTapMovement = HandMovementType.Unknown;
            return lastRecognizedTapMovement;
        }

        Vector3 worldDelta = currentPosition - lastTapAnchorWorld;
        localDelta = ToLocalDelta(worldDelta);
        distance = worldDelta.magnitude;
        lastRecognizedTapMovement = ClassifyDelta(worldDelta, minTapTravelDistance);
        lastTapAnchorWorld = currentPosition;

        return lastRecognizedTapMovement;
    }

    public void BeginSlideTracking()
    {
        if (trackingTarget == null)
            return;

        slideTrackingActive = true;
        slideStartWorld = trackingTarget.position;
    }

    public HandMovementType EndSlideTracking(out Vector3 localDelta, out float distance)
    {
        localDelta = Vector3.zero;
        distance = 0f;

        if (!slideTrackingActive || trackingTarget == null)
            return HandMovementType.Unknown;

        slideTrackingActive = false;
        Vector3 worldDelta = trackingTarget.position - slideStartWorld;
        localDelta = ToLocalDelta(worldDelta);
        distance = worldDelta.magnitude;

        return ClassifyDelta(worldDelta, minSlideTravelDistance);
    }

    void EvaluateMovement()
    {
        if (samples.Count < 2)
        {
            currentMovement = HandMovementType.Still;
            currentWorldDelta = Vector3.zero;
            currentLocalDelta = Vector3.zero;
            currentVelocity = 0f;
            return;
        }

        MovementSample oldest = samples.Peek();
        MovementSample newest = oldest;

        foreach (MovementSample sample in samples)
            newest = sample;

        float elapsed = Mathf.Max(0.0001f, newest.time - oldest.time);
        currentWorldDelta = newest.position - oldest.position;
        currentVelocity = currentWorldDelta.magnitude / elapsed;

        if (referenceSpace != null)
            currentLocalDelta = referenceSpace.InverseTransformDirection(currentWorldDelta);
        else
            currentLocalDelta = transform.InverseTransformDirection(currentWorldDelta);

        if (currentWorldDelta.magnitude < minGestureDistance || currentVelocity < minGestureVelocity)
        {
            currentMovement = HandMovementType.Still;
            return;
        }

        Vector3 normalizedDelta = currentLocalDelta.normalized;
        float absX = Mathf.Abs(normalizedDelta.x);
        float absY = Mathf.Abs(normalizedDelta.y);
        float absZ = Mathf.Abs(normalizedDelta.z);

        if (absX >= absY && absX >= absZ)
            currentMovement = normalizedDelta.x >= 0f ? HandMovementType.Right : HandMovementType.Left;
        else if (absY >= absX && absY >= absZ)
            currentMovement = normalizedDelta.y >= 0f ? HandMovementType.Up : HandMovementType.Down;
        else
            currentMovement = normalizedDelta.z >= 0f ? HandMovementType.Forward : HandMovementType.Back;
    }

    HandMovementType ClassifyDelta(Vector3 worldDelta, float minDistance)
    {
        if (worldDelta.magnitude < minDistance)
            return HandMovementType.Still;

        Vector3 localDelta = ToLocalDelta(worldDelta);
        Vector3 normalizedDelta = localDelta.normalized;
        float absX = Mathf.Abs(normalizedDelta.x);
        float absY = Mathf.Abs(normalizedDelta.y);
        float absZ = Mathf.Abs(normalizedDelta.z);

        if (absX >= absY && absX >= absZ)
            return normalizedDelta.x >= 0f ? HandMovementType.Right : HandMovementType.Left;

        if (absY >= absX && absY >= absZ)
            return normalizedDelta.y >= 0f ? HandMovementType.Up : HandMovementType.Down;

        return normalizedDelta.z >= 0f ? HandMovementType.Forward : HandMovementType.Back;
    }

    Vector3 ToLocalDelta(Vector3 worldDelta)
    {
        if (referenceSpace != null)
            return referenceSpace.InverseTransformDirection(worldDelta);

        return transform.InverseTransformDirection(worldDelta);
    }
}
