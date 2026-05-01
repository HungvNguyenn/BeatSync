using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public GameObject tapOrbPrefab;
    public GameObject slideOrbPrefab;
    public Transform spawnAnchor;

    const float LeftRightLaneDistance = 0.7f;
    const float UpLaneHeight = 1.45f;
    const float CenterLaneHeight = 0.85f;
    const float DownLaneHeight = 0.25f;
    const float SlideSpawnHeight = 1.2f;
    const float MinSlidePathLength = 0.9f;
    const float MaxSlidePathLength = 1.8f;
    const float SlideLengthPerSecond = 0.75f;
    const float SlideCompletionGraceSeconds = 0.2f;

    public void SpawnOrb(float energy, string target)
    {
        Vector3 forward = spawnAnchor.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, forward);
        Vector2 offset = GetTargetOffset(target);

        Vector3 spawnPos =
            spawnAnchor.position +
            right * offset.x +
            Vector3.up * offset.y;

        GameObject orb = Instantiate(tapOrbPrefab, spawnPos, Quaternion.identity);
        ApplyTargetVisuals(orb, target, energy);
    }

    public void SpawnSlide(float startTime, float endTime)
    {
        float holdDuration = Mathf.Max(0.1f, endTime - startTime);
        float slidePathLength = Mathf.Clamp(
            holdDuration * SlideLengthPerSecond,
            MinSlidePathLength,
            MaxSlidePathLength);

        Vector3 forward = spawnAnchor.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        bool useVerticalPath = Random.value < 0.5f;
        float axisSign = Random.value < 0.5f ? -1f : 1f;

        Vector3 centerPos;
        Vector3 axisDirection;
        float halfPathLength;

        if (useVerticalPath)
        {
            float laneSpan = UpLaneHeight - DownLaneHeight;
            halfPathLength = Mathf.Min(slidePathLength * 0.5f, laneSpan * 0.5f);
            centerPos = spawnAnchor.position + Vector3.up * ((UpLaneHeight + DownLaneHeight) * 0.5f);
            axisDirection = Vector3.up * axisSign;
        }
        else
        {
            halfPathLength = slidePathLength * 0.5f;
            centerPos = spawnAnchor.position + Vector3.up * SlideSpawnHeight;
            axisDirection = right.normalized * axisSign;
        }

        Vector3 startPos = centerPos - axisDirection * halfPathLength;
        Vector3 endPos = centerPos + axisDirection * halfPathLength;

        GameObject slide = Instantiate(slideOrbPrefab, startPos, Quaternion.identity);

        SlideOrb slideOrb = slide.GetComponent<SlideOrb>();
        if (slideOrb != null)
            slideOrb.ConfigurePath(startPos, endPos, holdDuration, SlideCompletionGraceSeconds);

        SlideVisual visual = slide.GetComponent<SlideVisual>();
        if (visual != null)
            visual.ConfigurePath(startPos, endPos);
    }

    Vector2 GetTargetOffset(string target)
    {
        switch ((target ?? "Center").Trim().ToLowerInvariant())
        {
            case "left":
                return new Vector2(-LeftRightLaneDistance, CenterLaneHeight);
            case "right":
                return new Vector2(LeftRightLaneDistance, CenterLaneHeight);
            case "up":
                return new Vector2(0f, UpLaneHeight);
            case "down":
                return new Vector2(0f, DownLaneHeight);
            default:
                return new Vector2(0f, CenterLaneHeight);
        }
    }

    void ApplyTargetVisuals(GameObject orb, string target, float energy)
    {
        if (orb == null)
            return;

        if (orb.TryGetComponent(out OrbData orbData))
            orbData.Set(InteractionType.Tap, energy, GetExpectedMovement(target));
    }

    HandMovementType GetExpectedMovement(string target)
    {
        switch ((target ?? "Center").Trim().ToLowerInvariant())
        {
            case "left":
                return HandMovementType.Left;
            case "right":
                return HandMovementType.Right;
            case "up":
                return HandMovementType.Up;
            case "down":
                return HandMovementType.Down;
            default:
                return HandMovementType.Unknown;
        }
    }
}
