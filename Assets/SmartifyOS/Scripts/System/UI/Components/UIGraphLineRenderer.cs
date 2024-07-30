using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGraphLineRenderer : Graphic
{
    public Vector2Int gridSize;
    public float thickness;

    public List<Vector2Int> points;

    float width;
    float height;
    float unitWidth;
    float unitHeight;

    private float resolution = 0.001f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {

        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        unitWidth = width / gridSize.x;
        unitHeight = height / gridSize.y;

        if (points.Count < 2) return;


        float angle = 0;
        for (int i = 0; i < points.Count - 1; i++)
        {

            Vector2Int point = points[i];
            Vector2Int point2 = points[i + 1];

            if (i < points.Count - 1)
            {
                angle = GetAngle(points[i], points[i + 1]) + 90f;
            }

            DrawVerticesForPoint(point, point2, angle, vh);
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            int index = i * 4;
            vh.AddTriangle(index + 0, index + 1, index + 2);
            vh.AddTriangle(index + 1, index + 2, index + 3);
        }
    }
    public float GetAngle(Vector2 me, Vector2 target)
    {
        //panel resolution go there in place of 9 and 16

        return (float)(Mathf.Atan2(9f * (target.y - me.y), 16f * (target.x - me.x)) * (180 / Mathf.PI));
    }
    void DrawVerticesForPoint(Vector2Int point, Vector2Int point2, float angle, VertexHelper vh)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x * resolution, unitHeight * point.y * resolution);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x * resolution, unitHeight * point.y * resolution);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point2.x * resolution, unitHeight * point2.y * resolution);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point2.x * resolution, unitHeight * point2.y * resolution);
        vh.AddVert(vertex);
    }

    public void ShiftPoints(Vector2Int point)
    {
        // Shift the points array
        for (int i = 1; i < points.Count; i++)
        {
            points[i - 1] = new Vector2Int(points[i].x - 100, points[i].y);
        }

        // Add new value
        points[points.Count - 1] = point;
        SetVerticesDirty();
    }

    public void AddPoint(Vector2Int point)
    {
        points.Add(point);
        SetVerticesDirty();
    }

    public void RemoveLastPoint()
    {
        points.RemoveAt(0);
        SetVerticesDirty();
    }

    public int GetPointCount()
    {
        return points.Count;
    }
}
