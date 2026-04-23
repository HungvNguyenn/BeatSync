using UnityEngine;

public class OrbData : MonoBehaviour
{
    public InteractionType interactionType;
    public float energy;

    public Vector3 direction;

    public void Set(InteractionType type, float e)
    {
        interactionType = type;
        energy = e;
    }
}
