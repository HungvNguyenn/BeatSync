using UnityEngine;

public class OrbData : MonoBehaviour
{
    public InteractionType interactionType;
    public float energy;
    public HandMovementType expectedMovement = HandMovementType.Unknown;

    public void Set(InteractionType type, float e, HandMovementType movement)
    {
        interactionType = type;
        energy = e;
        expectedMovement = movement;
    }
}
