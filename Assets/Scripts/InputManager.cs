using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Util;
using static UnityEngine.Rendering.DebugUI;

public class InputManager : Singletonref<InputManager>
{

    public static Action<Vector3, bool, Action> OnMovementInput = delegate { };
    public static Action OnHoldingCancel = delegate { };
    Transform previousdata;
    public InteractionBTN Interactionbtn;
    public EventSystem Esystem;
    //Drag calculations
    bool isDrag;
    [SerializeField]
    Vector2 startPositions, currentPositions, updatedPositions;
    [SerializeField]
    [Range(-0.1f, 0.1f)]
    float touchsensitive = 0;
    public GameObject Selector;
    public Selector _Selector;
    public LayerMask FloorMask;
    //Events
    //public static Action<float> OnDrag = delegate { };
    public InputActionAsset _action;
    public CameraControle _cameraControle;
    public FreeCameraObject _freeCameraObject;
    public Vector2 ClickPosition { get => _action.FindActionMap("BasicMovement").FindAction("ClickPosition").ReadValue<Vector2>(); }
    Coroutine _dragcoroutine;
    Coroutine _Buildcoroutine;
    Coroutine _pressNholdingcoroutine;
    CameraTargetMode _currenttargetmode;
    GameObject _currenttarget { get => ShopManager.Instance.GetCurrentPurchaseObject; }
    GridMaker _grid { get => GameDataDNDL.Instance.GetGrid(); }
    private void Start()
    {
        //Basic Mode
        _action.FindActionMap("BasicMovement").Enable();
        _action.FindAction("Click").started += OnClick;
        _action.FindAction("Click").canceled += OnClickCancel;
        _action.FindAction("ClickPosition").performed += OnMouseMove;
        //Building Mode
        _action.FindActionMap("Building").FindAction("Click").started += OnBuildModeEnabled;
        _action.FindActionMap("Building").FindAction("Click").canceled += context => { Invoke("OnBuildModeDisable", Time.deltaTime * 5f); };
        //ChangeMode(CameraTargetMode.PlayerCam);
    }

    #region InputActions
    void OnClick(InputAction.CallbackContext context)
    {

        if (_currenttargetmode == CameraTargetMode.PlayerCam)
        {
            // On Clicked
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            RaycastHit ray;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(ClickPosition), out ray, 1000f))
            {
                //CustomLogs.CC_Log($"{ray.point}", "cyan");
                if (ray.transform.tag == "unlocked")
                {

                    if (GameDataDNDL.Instance.GetPlayer().isHandsfull)
                        GameDataDNDL.Instance.GetPlayer().InHand.AddEvent();
                    else
                        Interactionbtn.ResetButton("Interact");
                    var pos = GameDataDNDL.Instance.GetGrid().GetRoundAnchorPositionFromWorldPosiion(ray.point);
                    OnMovementInput(pos, false, null);
                }
                if (ray.transform.GetComponent<InteractiveBlock>() != null)
                {
                    ///CustomLogs.CC_Log($"{rayUE.transform.GetComponent<Table>().GetLookPos().position}", "cyan");
                    //if (ray.transform.GetComponent<InteractiveBlock>().IsInteractionSatisfied())
                    //    InteractionButtonClick(ray.transform.GetComponent<InteractiveBlock>());
                    ///TODO: Hold to Move Equipment
                    
                    OnMovementInput(ray.transform.GetComponent<InteractiveBlock>().GetLookPos().position, true, () =>
                    {
                        if (ray.transform.GetComponent<InteractiveBlock>().IsInteractionSatisfied())
                            InteractionButtonClick(ray.transform.GetComponent<InteractiveBlock>());
                    });
                    if (Vector3.Distance(new Vector3(GameDataDNDL.Instance.GetPlayer().transform.position.x, ray.transform.GetComponent<InteractiveBlock>().GetLookPos().position.y, GameDataDNDL.Instance.GetPlayer().transform.position.z), ray.transform.GetComponent<InteractiveBlock>().GetLookPos().position) < 0.5f)
                        if (ray.transform.GetComponent<InteractiveBlock>().IsInteractionSatisfied())
                            InteractionButtonClick(ray.transform.GetComponent<InteractiveBlock>());
                }
            }
        }
        if (_currenttargetmode == CameraTargetMode.FreeCam)
        {
            _dragcoroutine = StartCoroutine(OnDrag());
        }


    }
    void OnClickCancel(InputAction.CallbackContext context)
    {
        //if (_currenttargetmode == CameraTargetMode.FreeCam)
        if (_dragcoroutine != null) StopCoroutine(_dragcoroutine);
        //if (UIInHold) {
        //    Debug.Log("stop Holding call the callbacks");
        //    StopCoroutine(_pressNholdingcoroutine);
        //    OnHoldingCancel?.Invoke();
        //}

    }
    void OnMouseMove(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        RaycastHit rayUE;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(ClickPosition), out rayUE, 1000f))
        {
            Selector.transform.position = GameDataDNDL.Instance.GetGrid().GetRoundAnchorPositionFromWorldPosiion(new Vector3(rayUE.point.x, 0.015f, rayUE.point.z));
            if (rayUE.transform.GetComponent<InteractiveBlock>() != null)
            {
                if (previousdata != null && rayUE.transform != previousdata)
                {
                    // previousdata.GetComponent<InteractiveBlock>().OnMouseHoverExit();
                }
                previousdata = rayUE.transform;
                //rayUE.transform.GetComponent<InteractiveBlock>().OnMouseHoverEnter();
            }
            else
            {
                if (previousdata != null && rayUE.transform != previousdata)
                {
                    //previousdata.GetComponent<InteractiveBlock>().OnMouseHoverExit();
                    previousdata = null;
                }
            }
        }
#endif
    }
    private void PrintStatment(InputAction.CallbackContext context)
    {
        Debug.Log("checking" + _action.FindActionMap("Building").FindAction("ClickPosition").ReadValue<Vector2>());
        //throw new NotImplementedException();
        // Debug.Log("Check Input" + _action.FindActionMap("BasicMovement").FindAction("ClickPosition").ReadValue<Vector2>()+" "+ Input.mousePosition);
    }

    IEnumerator OnDrag()
    {
        while (true)
        {
            yield return null;
            var tempos = _action.FindActionMap("BasicMovement").FindAction("ClickDelta").ReadValue<Vector2>();
            _freeCameraObject.onMove(new Vector3(_freeCameraObject.transform.position.x + (tempos.x * touchsensitive), 0, _freeCameraObject.transform.position.z + (tempos.y * touchsensitive)));
        }
    }


    GameObject SelectedObject;
    void OnBuildModeEnabled(InputAction.CallbackContext context)
    {
        SelectedObject = null;
        RaycastHit ray;
        var tempos = _action.FindActionMap("Building").FindAction("ClickDelta").ReadValue<Vector2>();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(tempos), out ray, 1000f))
        {
            if (SelectedObject == null)
            {
                if (ray.transform.GetComponent<InteractiveBlock>() == _currenttarget.GetComponent<InteractiveBlock>())
                {
                    SelectedObject = _currenttarget;
                    Debug.Log("setting the Target");
                }
            }
        }
        if (SelectedObject != null)
        {
            _Buildcoroutine = StartCoroutine(OnBuildObjectMove());
        }

    }


    Vector3 FinalPosition;
    IEnumerator OnBuildObjectMove()
    {
        //GameObject SelectedObject = null;
        FinalPosition = Vector3.zero;
        while (true)
        {
            yield return null;
            var tempos = _action.FindActionMap("Building").FindAction("ClickDelta").ReadValue<Vector2>();
            //RaycastHit ray;
            RaycastHit rayFloor;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(tempos), out rayFloor, 1000f))
                //Debug.Log("is occp"+_grid.processCoords(_grid.GetRoundAnchorPositionFromWorldPosiion(new Vector3(rayFloor.point.x, 1.69f, rayFloor.point.z))));
                if (_grid.processCoords(_grid.GetRoundAnchorPositionFromWorldPosiion(new Vector3(rayFloor.point.x, 1.69f, rayFloor.point.z))))
                {
                    Debug.Log("Check update here");
                    var pos = _grid.GetRoundAnchorPositionFromWorldPosiion(new Vector3(rayFloor.point.x, 1.69f, rayFloor.point.z));
                    FinalPosition = pos;
                    //SelectedObject.transform.position = pos;
                    SelectedObject.transform.position = Vector3.Lerp(SelectedObject.transform.position, pos, Time.deltaTime * 10f);
                }
        }
    }
    void OnBuildModeDisable()
    {
        if (SelectedObject != null)
        {
            SelectedObject.transform.position = new Vector3(FinalPosition.x, 0, FinalPosition.z);
            SelectedObject = null;
        }
        if (_Buildcoroutine != null) { StopCoroutine(_Buildcoroutine); }
        ;
    }

    #endregion
    //bool UIInHold;
    //public void OnHoldUI(float _holdThreshold, Action<float> _update, Action _callback)
    //{
    //    UIInHold= true;
    //    _pressNholdingcoroutine = StartCoroutine(OnHold(_holdThreshold,_update,
    //        ()=> { 
    //            _callback?.Invoke();
    //            UIInHold = false;
    //        }));
    //}
    //IEnumerator OnHold(float _holdThreshold, Action<float> _update, Action _callback)
    //{
    //    float timer = 0f;
    //    while (timer<_holdThreshold) { 
    //        yield return null;
    //        timer += Time.deltaTime;
    //        _update?.Invoke(timer);
    //    }
    //    Debug.Log("stop Holding call the callbacks");
    //    _callback?.Invoke();
    //}
    public void UpdateGridPositions(Vector3 pos)
    {
        _grid.UpdateCellOccupied(pos, true);
        var t = GridMaker.GetIndexFromAnchorPosition(new Vector2(pos.x, pos.z));
        GameDataDNDL.Instance.UpdateNavMesh((int)t.x, (int)t.y, false);
    }
    public void InteractionButtonClick(InteractiveBlock table)
    {
        Interactionbtn.AddEvent(table,
                        table.GetInteractableName(), true,
                        table.isTableEmpty());
        //OnMovementInput(table.GetLookPos().position, true);
    }
    public void InteractionButtonAddEvent(string btnName, Action onClick)
    {
        Interactionbtn.ResetButton("Interact");
        Interactionbtn.AddEvent(btnName, onClick);
    }
    public void ChangeMode(CameraTargetMode _mode, GameObject _target)
    {
        _currenttargetmode = _mode;
        _grid.GridVisual.SetActive(false);
        switch (_mode)
        {
            case CameraTargetMode.PlayerCam:

                _action.FindActionMap("BasicMovement").Enable();
                _action.FindActionMap("Building").Disable();
                //_cameraControle.SetTarget(/*GameDataDNDL.Instance.GetPlayer().gameObject*/_target);
                break;
            case CameraTargetMode.FreeCam:

                _action.FindActionMap("Building").Disable();
                //_action.FindActionMap("BasicMovement").Disable();
                //_cameraControle.SetTarget(/*GameDataDNDL.Instance.GetFreeCamRig()*/_target);
                break;
            case CameraTargetMode.BuildingMode:
                //_currenttarget=_target;
                _action.FindActionMap("Building").Enable();
                _action.FindActionMap("BasicMovement").Disable();
                _grid.GridVisual.SetActive(true);
                break;
            default: break;
        }
        _cameraControle.SetTarget(/*GameDataDNDL.Instance.GetFreeCamRig()*/_target);
    }

    public void EnterBuildingMode()
    {
        _cameraControle.CameraSmoothValue = 1.2f;
    }
    public void ExitBuildingMode()
    {
        _cameraControle.CameraSmoothValue = 5f;
    }
}
