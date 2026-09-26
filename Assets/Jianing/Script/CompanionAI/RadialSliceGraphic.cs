using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
public class RadialSliceGraphic :
    MaskableGraphic,
    ICanvasRaycastFilter
{
    private float innerRadius = 65f;
    private float outerRadius = 230f;
    private float centerAngle = 90f;
    private float arcAngle = 86f;

    public void Configure(
        float inner,
        float outer,
        float center,
        float arc
    )
    {
        innerRadius = Mathf.Max(0f, inner);
        outerRadius = Mathf.Max(innerRadius + 1f, outer);
        centerAngle = center;
        arcAngle = Mathf.Clamp(arc, 1f, 360f);

        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper mesh)
    {
        mesh.Clear();

        int segments =
            Mathf.Max(2, Mathf.CeilToInt(arcAngle / 4f));

        for (int i = 0; i <= segments; i++)
        {
            float angle =
                (
                    centerAngle -
                    arcAngle * 0.5f +
                    arcAngle * i / segments
                ) * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            );

            mesh.AddVert(
                new Vector3(
                    direction.x * innerRadius,
                    direction.y * innerRadius,
                    0f
                ),
                color,
                Vector2.zero
            );

            mesh.AddVert(
                new Vector3(
                    direction.x * outerRadius,
                    direction.y * outerRadius,
                    0f
                ),
                color,
                Vector2.one
            );

            if (i > 0)
            {
                int index = i * 2;

                mesh.AddTriangle(
                    index - 2,
                    index - 1,
                    index + 1
                );

                mesh.AddTriangle(
                    index - 2,
                    index + 1,
                    index
                );
            }
        }
    }

    public bool IsRaycastLocationValid(
        Vector2 screenPoint,
        Camera eventCamera
    )
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                screenPoint,
                eventCamera,
                out Vector2 point))
        {
            return false;
        }

        float radius = point.magnitude;

        float angle =
            Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg;

        return radius >= innerRadius &&
               radius <= outerRadius &&
               Mathf.Abs(
                   Mathf.DeltaAngle(angle, centerAngle)
               ) <= arcAngle * 0.5f;
    }
}