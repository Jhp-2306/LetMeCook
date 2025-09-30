using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillUpMiniGame : MonoBehaviour
{

    bool isMiniStart;
    float fillUpProgress;
    float fillUpProgressMin;
    float fillUpProgressWitdh;
    public float FillRate;
    public Action<int> OnMiniGameOver = delegate { };
    //Visual Data
    public GameObject HUDParent;
    public RectTransform ParentSafeArea;
    public RectTransform SafeArea;
    public Slider ProgressVis;

    public bool isMiniGameRunning { get => isMiniStart; }
    public void FillupMiniGameStart(float minRange01, float Width)
    {
        fillUpProgress = 0f;
        fillUpProgressMin = minRange01;
        fillUpProgressWitdh = Width;
        isMiniStart = true;
        HUDParent.SetActive(true);
        var width = ParentSafeArea.rect.width * fillUpProgressWitdh;
        var pos = (ParentSafeArea.rect.width * fillUpProgressMin) - width / 2;
        CustomLogs.CC_Log($"Setting Mini Game{width},pos{pos},parent width{ParentSafeArea.rect.width},{SafeArea.localPosition}", "cyan");
        SafeArea.sizeDelta = new Vector2(width, SafeArea.rect.height);
        SafeArea.anchoredPosition = new Vector3(pos, SafeArea.localPosition.y, SafeArea.localPosition.z);
        //Start Game
        OnHoldUI(1.5f, UpdateFillUpMiniGame, OnEnd);
        //isMiniStart=true;
    }
    public void UpdateFillUpMiniGame(float fillpercent)
    {
        fillUpProgress = Mathf.Clamp01(fillpercent / 1.5f);
        ProgressVis.value = fillUpProgress;
    }
    public void onClick()
    {
        StopCoroutine(_pressNholdingcoroutine);
        OnEnd();
    }
    public void OnEnd()
    {
        Debug.Log(CustomLogs.CC_TagLog("Mini Game", $"Calling the OverCallBack{fillUpProgress},{fillUpProgressMin},{fillUpProgressMin + fillUpProgressWitdh}{fillUpProgress > fillUpProgressMin && fillUpProgress < fillUpProgressMin + fillUpProgressWitdh}"));
        HUDParent.SetActive(false);
        var val = 0;
        if (fillUpProgress > fillUpProgressMin - fillUpProgressWitdh && fillUpProgress < fillUpProgressMin + fillUpProgressWitdh) val = 1;
        OnMiniGameOver?.Invoke(val);
        isMiniStart = false;
        //InputManager.OnHoldingCancel -= OnEnd;
    }

    #region NOTE:- Can Move this Block into DNDL{

    private Coroutine _pressNholdingcoroutine;
    public void OnHoldUI(float _holdThreshold, Action<float> _update, Action _callback)
    {
        //UIInHold = true;
        _pressNholdingcoroutine = StartCoroutine(OnHold(_holdThreshold, _update,
            () => {
                _callback?.Invoke();
                //UIInHold = false;
            }));
    }
    IEnumerator OnHold(float _holdThreshold, Action<float> _update, Action _callback)
    {
        float timer = 0f;
        while (timer < _holdThreshold)
        {
            yield return null;
            timer += Time.deltaTime;
            _update?.Invoke(timer);
        }
        Debug.Log("stop Holding call the callbacks");
        _callback?.Invoke();
    }

    #endregion
}

