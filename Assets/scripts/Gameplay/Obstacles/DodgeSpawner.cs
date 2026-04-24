using UnityEngine;

public class DodgeSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public Transform spawnAnchor;
    public float spawnDistance = 5f;
    public float dodgeHeight = 1.2f;

    public void SpawnFrontDodge()
    {
        SpawnObstacle();
    }

    void SpawnObstacle()
    {
        if (obstaclePrefab == null || spawnAnchor == null)
            return;

        Vector3 forward = spawnAnchor.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            forward = Vector3.forward;
        else
            forward.Normalize();

        Vector3 spawnPos =
            spawnAnchor.position +
            forward * spawnDistance +
            Vector3.up * dodgeHeight;

        GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.LookRotation(forward));

        if (obstacle.TryGetComponent(out Obstacle obstacleComponent))
            obstacleComponent.Initialize(-forward);
    }
}
