using UnityEngine;

public class StageBoundaryWarning : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayerCollision(other))
            return;

        Debug.Log("Boundary Warning");
        GameSceneManager.instance?.ShowBoundaryWarning();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayerCollision(other))
            return;

        Debug.Log("Boundary Cleared");
        GameSceneManager.instance?.HideBoundaryWarning();
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
