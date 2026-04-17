using UnityEngine;

public class SlideOrb : MonoBehaviour
{
    private OrbLifetime lifetime;

    public Vector3 direction;   // <- set by spawner (IMPORTANT)

    private bool started = false;
    private bool completed = false;

    private float progress = 0f;

    void Start()
    {
        lifetime = GetComponent<OrbLifetime>();
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand")) return;

        started = true;
        progress = 0f;
    }

    void OnTriggerStay(Collider other)
    {
        if (!started || completed) return;
        if (!other.CompareTag("PlayerHand")) return;

        Vector3 handDir =
            (other.transform.position - transform.position).normalized;

        float alignment = Vector3.Dot(direction, handDir);

        if (alignment > 0.6f)
        {
            progress += Time.deltaTime;
        }

        if (progress >= 0.3f)
        {
            completed = true;

            lifetime?.Hit();
            Destroy(gameObject);
        }
    }
}