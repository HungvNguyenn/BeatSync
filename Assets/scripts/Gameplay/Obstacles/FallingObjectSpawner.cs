using UnityEngine;

public class FallingObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab;
    public Transform playerCamera;
    public float spawnInterval = 2f;

    public float forwardDistance = 2f;
    public float horizontalRange = 1.5f;
    public float verticalOffset = 1f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnObject), 1f, spawnInterval);
    }

    void SpawnObject()
    {
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;

        Vector3 spawnPosition =
            playerCamera.position +
            forward * forwardDistance +
            right * Random.Range(-horizontalRange, horizontalRange) +
            Vector3.up * verticalOffset;

        Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
    }
}