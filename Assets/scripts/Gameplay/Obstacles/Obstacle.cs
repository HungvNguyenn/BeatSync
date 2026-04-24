using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 3f;
    public float maxTravelDistance = 10f;

    Vector3 moveDirection = Vector3.back;
    Vector3 spawnPosition;

    void Awake()
    {
        spawnPosition = transform.position;
    }

    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.back;
        spawnPosition = transform.position;
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        if (Vector3.Distance(spawnPosition, transform.position) >= maxTravelDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerCollision(other))
        {
            Debug.Log("Wall Hit");
            Destroy(gameObject);
        }
    }

    bool IsPlayerCollision(Collider other)
    {
        if (other == null)
            return false;

        if (other.CompareTag("Player"))
            return true;

        Transform current = other.transform;

        while (current != null)
        {
            if (current.CompareTag("Player"))
                return true;

            current = current.parent;
        }

        return false;
    }
}
