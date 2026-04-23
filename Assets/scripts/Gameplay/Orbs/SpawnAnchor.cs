using UnityEngine;

public class SpawnAnchor : MonoBehaviour
{
    public Transform player;
    public float distanceForward = 2.5f;
    public float heightOffset = 0f;

    void Update()
    {
        if (player == null) return;

        Vector3 forward = player.forward;
        forward.y = 0f;
        forward.Normalize();

        transform.position =
            player.position +
            forward * distanceForward +
            Vector3.up * heightOffset;

        transform.rotation = Quaternion.LookRotation(forward);
    }
}