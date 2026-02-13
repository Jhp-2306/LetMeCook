using TMPro;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class TMP_CurveText : MonoBehaviour
{
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 0);

    [Tooltip("Overall vertical strength of the curve")]
    public float curveStrength = 10f;

    [Tooltip("Recalculate curve every frame (disable if animating manually)")]
    public bool liveUpdate = true;


    [Header("Rotation")]
    public float rotationStrength = 1f;
    public bool rotateAlongCurve = true;
    TMP_Text textComponent;
    Mesh mesh;
    Vector3[] vertices;

    void OnEnable()
    {
        textComponent = GetComponent<TMP_Text>();
        textComponent.ForceMeshUpdate();
    }

    void Update()
    {
        if (!liveUpdate && !Application.isPlaying)
            return;

        CurveText();
    }
    //private void OnDrawGizmos()
    //{
    //    textComponent = GetComponent<TMP_Text>();
    //    textComponent.ForceMeshUpdate();
    //    CurveText();
    //}

    //void CurveText()
    //{
    //    textComponent.ForceMeshUpdate();
    //    TMP_TextInfo textInfo = textComponent.textInfo;

    //    if (textInfo.characterCount == 0)
    //        return;

    //    float minX = float.MaxValue;
    //    float maxX = float.MinValue;

    //    // Find bounds
    //    for (int i = 0; i < textInfo.characterCount; i++)
    //    {
    //        var charInfo = textInfo.characterInfo[i];
    //        if (!charInfo.isVisible) continue;

    //        minX = Mathf.Min(minX, charInfo.bottomLeft.x);
    //        maxX = Mathf.Max(maxX, charInfo.topRight.x);
    //    }

    //    float width = maxX - minX;
    //    if (width <= 0) return;

    //    // Apply curve
    //    for (int i = 0; i < textInfo.characterCount; i++)
    //    {
    //        var charInfo = textInfo.characterInfo[i];
    //        if (!charInfo.isVisible) continue;

    //        int meshIndex = charInfo.materialReferenceIndex;
    //        int vertexIndex = charInfo.vertexIndex;

    //        vertices = textInfo.meshInfo[meshIndex].vertices;

    //        // Normalize X position (0–1)
    //        float midX = (vertices[vertexIndex].x + vertices[vertexIndex + 2].x) * 0.5f;
    //        float normalizedX = (midX - minX) / width;

    //        float yOffset = curve.Evaluate(normalizedX) * curveStrength;

    //        Vector3 offset = new Vector3(0, yOffset, 0);

    //        vertices[vertexIndex + 0] += offset;
    //        vertices[vertexIndex + 1] += offset;
    //        vertices[vertexIndex + 2] += offset;
    //        vertices[vertexIndex + 3] += offset;
    //    }

    //    // Push mesh back
    //    for (int i = 0; i < textInfo.meshInfo.Length; i++)
    //    {
    //        textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
    //        textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
    //    }
    //}
    void CurveText()
    {
        textComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = textComponent.textInfo;

        if (textInfo.characterCount == 0)
            return;

        float minX = float.MaxValue;
        float maxX = float.MinValue;

        // Get text bounds
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            minX = Mathf.Min(minX, textInfo.characterInfo[i].bottomLeft.x);
            maxX = Mathf.Max(maxX, textInfo.characterInfo[i].topRight.x);
        }

        float width = maxX - minX;
        if (width <= 0) return;

        const float epsilon = 0.001f;

        // Apply curve + rotation
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            vertices = textInfo.meshInfo[meshIndex].vertices;

            // Character center
            Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 2]) * 0.5f;

            float normalizedX = (center.x - minX) / width;

            // Vertical offset
            float yOffset = curve.Evaluate(normalizedX) * curveStrength;

            // Rotation from curve slope
            float angle = 0f;
            if (rotateAlongCurve)
            {
                float y1 = curve.Evaluate(Mathf.Clamp01(normalizedX));
                float y2 = curve.Evaluate(Mathf.Clamp01(normalizedX + epsilon));
                float slope = (y2 - y1) / epsilon;

                angle = Mathf.Atan(slope) * Mathf.Rad2Deg * rotationStrength;
            }

            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            // Apply transform
            for (int v = 0; v < 4; v++)
            {
                Vector3 pos = vertices[vertexIndex + v];
                pos -= center;
                pos = rotation * pos;
                pos += center + Vector3.up * yOffset;
                vertices[vertexIndex + v] = pos;
            }
        }

        // Push mesh back
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
