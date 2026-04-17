using UnityEngine;

public class SlideVisual : MonoBehaviour
{
    public LineRenderer line;
    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;

        Vector3 start = transform.position;
        Vector3 end = start + direction * 2.0f;

        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}