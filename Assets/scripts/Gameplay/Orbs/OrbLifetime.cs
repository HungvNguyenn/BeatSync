using UnityEngine;

public class OrbLifetime : MonoBehaviour
{
    public float lifetime = 5f;
    public string missMessage = "Miss";

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

    public void SetRemainingLifetime(float seconds)
    {
        lifetime = Mathf.Max(0.01f, seconds);
        spawnTime = Time.time;
    }

    public void Hit()
    {
        if (!active) return;

        active = false;
        Destroy(gameObject);
    }

    public void ForceMiss()
    {
        Miss();
    }

    void Miss()
    {
        if (!active) return;

        active = false;
        Debug.Log(missMessage);
        Destroy(gameObject);
    }
}
