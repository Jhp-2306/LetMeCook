using System.Collections;
using UnityEngine;

public class PopoutAnimations : MonoBehaviour
{
    [SerializeField] private float duration = 0.25f;
    //[SerializeField] private AnimationCurve popupCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField]
    private AnimationCurve popupCurve = new AnimationCurve(
    new Keyframe(0f, 0f),
    new Keyframe(0.7f, 1.5f),
    new Keyframe(1f, 1f)
);
    private Vector3 targetScale;

    private void Awake()
    {
        targetScale = transform.localScale;
    }

    private void OnEnable()
    {
        StartPopup();
    }

    public void StartPopup()
    {
        StopAllCoroutines();
        StartCoroutine(Popup());
    }

    private System.Collections.IEnumerator Popup()
    {
        transform.localScale = Vector3.zero;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float value = popupCurve.Evaluate(t);

            transform.localScale = targetScale * value;

            yield return null;
        }

        transform.localScale = targetScale;
    }
}
