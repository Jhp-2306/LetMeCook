using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHUD : MonoBehaviour
{
    //public GameObject Parent;
    //public Image MainBg;
    [SerializeField] private Image MaskImg;
    [SerializeField] private Image MaskRayHit;
    [SerializeField] private GameObject DialogParent;
    [SerializeField] private TextMeshProUGUI DialogText;

    private Action OnClickCallBack;
    //[SerializeField]private Button onClickDialog;
    //[SerializeField]private Button onClickMask;
    private Coroutine lerpCoroutine;
    private bool isLerpActive;
    [SerializeField] private AnimationCurve Curve;
    [SerializeField] private float bouncetimeinSec;

    private GameObject PreviousParent;
    [SerializeField] private GameObject SubParent;
    public void SetTutorialHUD(string _dialog, Action _callback, bool isclose = false)
    {
        this.gameObject.SetActive(true);
        MaskImg.gameObject.SetActive(false);
        DialogText.text = _dialog;
        DialogParent.SetActive(true);
        isLerpActive = false;
        //onClickMask.onClick = new Button.ButtonClickedEvent();
        //onClickDialog.onClick.AddListener(delegate
        OnClickCallBack = () =>
        {
            isLerpActive = false;
            _callback();
            if (isclose)
                CloseTutorial();
        };
    }
    public void SetTutorialHUD(Transform position, Action _callback, bool isclose = false, bool isUI = false)
    {
        this.gameObject.SetActive(true);
        MaskImg.gameObject.SetActive(true);
        DialogParent.SetActive(false);
        isLerpActive = true;
        LeapMask();
        if (isUI)
        {
            MaskImg.rectTransform.position = position.position;
            MaskRayHit.rectTransform.position = position.position;
        }
        else
        {
            MaskImg.rectTransform.position = Camera.main.WorldToScreenPoint(position.position);
            MaskRayHit.rectTransform.position = Camera.main.WorldToScreenPoint(position.position);
        }
        OnClickCallBack = () =>
        {
            isLerpActive = false;
            _callback();
            if (isclose)
                CloseTutorial();
        };
    }
    public void OnClick()
    {
        OnClickCallBack();
    }
    public void CloseTutorial()
    {
        this.gameObject.SetActive(false);
        isLerpActive = false;
    }
    public void LeapMask()
    {
        if(lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        lerpCoroutine = StartCoroutine(scaleLerp());
    }
    IEnumerator scaleLerp()
    {
        yield return null;
        MaskImg.gameObject.transform.localScale = Vector3.one * Curve.Evaluate(0);
        while (isLerpActive) {
            float time = 0;
            while (time/bouncetimeinSec < 1)
            {
                time += Time.deltaTime;
                var timeidx=Mathf.Clamp01(time);
                MaskImg.gameObject.transform.localScale=Vector3.one*Curve.Evaluate(timeidx);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
