using UnityEngine;
using UnityEngine.UI;

public class UIReverseOverlay : MaskableGraphic
{
    public float steeringAngle = 0f; // Value between -1 and 1 to indicate how bend the lines should be
    [SerializeField] private float lineThickness = 10f; // How thick each line should be
    [SerializeField] private float lineHeight = 200f; // Height of the lines
    [SerializeField] private float bendStrength = 0.5f; // Multiplier for the bend strength
    [SerializeField] private float bendPosition = 0.5f; // Position along the height where the bend starts (0 to 1)
    [SerializeField] private float perspectiveAngle = 5f; // Angle at which lines converge towards each other
    [SerializeField] private int segments = 20; // Number of segments for the curve

    private float internalBendPosition;

    private float lastSteeringAngle;

    private void Update()
    {
        //steeringAngle = GpsController.wheelAngle / 36f;

        if (lastSteeringAngle != steeringAngle)
        {
            lastSteeringAngle = steeringAngle;
            SetAllDirty();
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();


        float leftLineX = -rectTransform.rect.width / 4;
        float rightLineX = rectTransform.rect.width / 4;
        float bottomY = -rectTransform.rect.height / 2;
        float topY = bottomY + lineHeight;

        internalBendPosition = bendPosition * Mathf.Abs(steeringAngle);
        internalBendPosition = Mathf.Clamp(internalBendPosition, 0, 0.68f);

        DrawBendingLine(vh, new Vector2(leftLineX, bottomY), lineHeight, steeringAngle, lineThickness, bendStrength, internalBendPosition, perspectiveAngle);
        DrawBendingLine(vh, new Vector2(rightLineX, bottomY), lineHeight, steeringAngle, lineThickness, bendStrength, internalBendPosition, perspectiveAngle);
    }

    private void DrawBendingLine(VertexHelper vh, Vector2 bottom, float height, float bend, float thickness, float bendStrength, float bendPosition, float perspectiveAngle)
    {
        // Ensure bendPosition is clamped between 0 and 1
        bendPosition = Mathf.Clamp01(bendPosition);

        // Calculate the top point offset for the bend
        float topOffset = bend * rectTransform.rect.width / 4;

        // Calculate the control point offset for the bend strength
        float controlOffset = topOffset * bendStrength;

        // Calculate the top and control points for the Bezier curve
        Vector2 top = new Vector2(bottom.x + topOffset, bottom.y + height);
        Vector2 controlPoint = new Vector2(bottom.x + controlOffset, bottom.y + height * bendPosition);

        // Adjust top point for perspective effect (horizontal adjustment only)
        float horizontalOffset = Mathf.Abs(bottom.x) * Mathf.Tan(Mathf.Deg2Rad * perspectiveAngle);
        top.x += Mathf.Sign(bottom.x) * horizontalOffset;

        // Calculate vertices along the Bezier curve
        Vector2[] points = new Vector2[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            points[i] = CalculateQuadraticBezierPoint(t, bottom, controlPoint, top);
        }

        // Draw the line segments
        for (int i = 0; i < segments; i++)
        {
            Vector2 p0 = points[i];
            Vector2 p1 = points[i + 1];

            AddQuad(vh, p0, p1, thickness);
        }
    }

    private Vector2 CalculateQuadraticBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return (uu * p0) + (2 * u * t * p1) + (tt * p2);
    }

    private void AddQuad(VertexHelper vh, Vector2 p0, Vector2 p1, float thickness)
    {
        Vector2 direction = (p1 - p0).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x) * thickness / 2;

        Vector2 v0 = p0 - normal;
        Vector2 v1 = p0 + normal;
        Vector2 v2 = p1 + normal;
        Vector2 v3 = p1 - normal;

        int index = vh.currentVertCount;

        vh.AddVert(v0, color, Vector2.zero);
        vh.AddVert(v1, color, Vector2.zero);
        vh.AddVert(v2, color, Vector2.zero);
        vh.AddVert(v3, color, Vector2.zero);

        vh.AddTriangle(index, index + 1, index + 2);
        vh.AddTriangle(index + 2, index + 3, index);
    }
}
