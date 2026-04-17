using UnityEngine;

public class OrbLifetime : MonoBehaviour
{
    public float lifetime = 5f;

    private float spawnTime;
    private bool active = true;

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        if (!active) return;

        if (Time.time - spawnTime >= lifetime)
        {
            Miss();
        }
    }

    public void Hit()
    {
        if (!active) return;

        active = false;
        Destroy(gameObject);
    }

    void Miss()
    {
        if (!active) return;

        active = false;
        Debug.Log("Miss");
        Destroy(gameObject);
    }
}