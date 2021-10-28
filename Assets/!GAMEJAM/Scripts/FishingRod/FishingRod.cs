
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FishingRod : UdonSharpBehaviour
{
    [Range(0, 1f)]
    public float bend = 0f;

    [SerializeField] Bobber bobber;

    [SerializeField] int lineQuality;
    [SerializeField] LineRenderer lineRenderer;

    [SerializeField] Transform startPoint;
    Vector3 midPoint;
    [SerializeField] Transform endPoint;

    void Start()
    {
        lineRenderer.positionCount = lineQuality;
    }

    private void Update()
    {
        DrawRope();
    }

    private void DrawRope()
    {
        lineRenderer.SetPosition(0, startPoint.position);
        midPoint = new Vector3(startPoint.position.x,endPoint.position.y/2,startPoint.position.z);

        int i = 0;
        for(float ratio = 0; ratio <= 1f; ratio += 1.0f / lineQuality, i++)
        {
            var tangentLineVertex1 = Vector3.Lerp(startPoint.position, midPoint, ratio);
            var tangentLineVertex2 = Vector3.Lerp(midPoint, endPoint.position, ratio);
            var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            
            if(i<lineQuality)
                lineRenderer.SetPosition(i, bezierPoint);
        }

        lineRenderer.SetPosition(lineQuality - 1, endPoint.position);
    }
}
