using UnityEngine;

public class SlideVisual : MonoBehaviour
{
    const float PathWidth = 0.08f;

    public LineRenderer line;
    public Transform startMarker;
    public Transform endMarker;

    private bool hasConfiguredPath = false;
    private Vector3 fixedStartPosition;
    private Vector3 fixedEndPosition;
    private bool detachedEndMarker = false;

    public void ConfigurePath(Vector3 start, Vector3 end)
    {
        fixedStartPosition = start;
        fixedEndPosition = end;
        hasConfiguredPath = true;

        if (line != null)
        {
            line.widthMultiplier = PathWidth;
            line.positionCount = 2;
            line.SetPosition(0, fixedStartPosition);
            line.SetPosition(1, fixedEndPosition);
        }

        if (startMarker != null)
            startMarker.gameObject.SetActive(false);

        if (endMarker != null)
        {
            if (!detachedEndMarker)
            {
                endMarker.SetParent(null, true);
                detachedEndMarker = true;
            }

            endMarker.gameObject.SetActive(true);
            endMarker.position = fixedEndPosition;
        }
    }

    void LateUpdate()
    {
        if (!hasConfiguredPath)
            return;

        if (line != null)
        {
            line.SetPosition(0, fixedStartPosition);
            line.SetPosition(1, fixedEndPosition);
        }

        if (endMarker != null)
            endMarker.position = fixedEndPosition;
    }

    void OnDestroy()
    {
        if (detachedEndMarker && endMarker != null)
            Destroy(endMarker.gameObject);
    }
}
