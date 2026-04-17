using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public GameObject tapOrbPrefab;
    public GameObject slideOrbPrefab;
    public Transform spawnAnchor;

    // TAP ORB
    public void SpawnOrb(float energy)
    {
        Vector3 forward = spawnAnchor.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, forward);

        float sideOffset = Random.Range(-0.8f, 0.8f);
        float heightOffset = Mathf.Lerp(-0.5f, 1.5f, energy);

        Vector3 spawnPos =
            spawnAnchor.position +
            right * sideOffset +
            Vector3.up * heightOffset;

        Instantiate(tapOrbPrefab, spawnPos, Quaternion.identity);
    }

    // SLIDE ORB
    public void SpawnSlide(float startTime, float endTime)
    {
        Vector3 forward = spawnAnchor.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, forward);

        // small randomness so slides don't feel identical
        float sideOffset = Random.Range(-0.4f, 0.4f);

        Vector3 spawnPos =
            spawnAnchor.position +
            right * sideOffset +
            Vector3.up * 1.2f;

        GameObject slide = Instantiate(slideOrbPrefab, spawnPos, Quaternion.identity);

        // IMPORTANT: assign direction
        SlideOrb slideOrb = slide.GetComponent<SlideOrb>();
        if (slideOrb != null)
        {
            slideOrb.SetDirection(forward);
        }

        // OPTIONAL: also update visual if you use LineRenderer
        SlideVisual visual = slide.GetComponent<SlideVisual>();
        if (visual != null)
        {
            visual.SetDirection(forward);
        }
    }
}