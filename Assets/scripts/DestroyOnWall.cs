using Unity.VisualScripting;
using UnityEngine;


public class HoldToDisappear : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            Debug.Log("Cub hit the wall");
            Destroy(gameObject);
        }
    }
}