using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Util;

public class InputManager : Singletonref<InputManager>
{
    public static Action<Vector3> OnMovementInput = delegate { };
    Transform previousdata;
    public InteractionBTN Interactionbtn;
    public EventSystem Esystem;
    //Drag calculations
    bool isDrag;
    [SerializeField]
    Vector2 startPositions, currentPositions, updatedPositions;
    [SerializeField][Range(-10f,10f)]
    float touchsensitive=0;
    //Events
    public static Action<float> OnDrag = delegate { };

    private void Update()
    {
        if (isDrag)
        {
            onDragEnable();
        }
        if (Esystem.IsPointerOverGameObject())
        {
            //on InteractionBButton
            if (Input.GetMouseButtonDown(0))
            {
                CustomLogs.CC_Log("drag is true", "red");
                isDrag = true;
                resetDragPositions();
                setDragPosition(Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(1000), 
                    Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(1000), 
                    Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(1000));
            }
            if (Input.GetMouseButtonUp(0))
            {
                CustomLogs.CC_Log("drag is false", "red");
                isDrag = false;
                resetDragPositions();

            }
            return;
        }
#if UNITY_EDITOR
        RaycastHit rayUE;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayUE, 1000f))
        {
            if (rayUE.transform.GetComponent<Table>() != null)
            {
                if (previousdata != null && rayUE.transform != previousdata)
                {
                    previousdata.GetComponent<Table>().OnMouseHoverExit();
                }
                previousdata = rayUE.transform;
                rayUE.transform.GetComponent<Table>().OnMouseHoverEnter();
            }
            else
            {
                if (previousdata != null && rayUE.transform != previousdata)
                {
                    previousdata.GetComponent<Table>().OnMouseHoverExit();
                    previousdata = null;
                }
            }
        }
#endif

        if (Input.GetMouseButtonDown(0))
        {
            
            RaycastHit ray;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out ray, 1000f))
            {
                //CustomLogs.CC_Log($"{ray.point}", "cyan");
                if (ray.transform.tag == "unlocked")
                {
                    Interactionbtn.ResetButton("Interact");
                    OnMovementInput(Conversions.Converions.Vector3DFloatToInt(ray.point));
                }
                if(rayUE.transform.GetComponent<Table>() != null)
                {
                    ///CustomLogs.CC_Log($"{rayUE.transform.GetComponent<Table>().GetLookPos().position}", "cyan");
                    Interactionbtn.AddEvent(rayUE.transform.GetComponent<Table>().OnClick, 
                        rayUE.transform.GetComponent<Table>().GetInteractableName(), true,
                        rayUE.transform.GetComponent<Table>().isTableEmpty());
                    OnMovementInput(rayUE.transform.GetComponent<Table>().GetLookPos().position);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) /*Input.touches[0].phase==TouchPhase.Ended*/)
        {
            if (isDrag)
            {
            isDrag = false;
            resetDragPositions();
            }
        }
    }
    void setDragPosition(Vector2 _startpos,Vector2 _currentpos,Vector2 _updatepos)
    {
        startPositions = _startpos;
        updatedPositions = _updatepos;
        currentPositions = _currentpos;
    }
    void resetDragPositions()
    {
        startPositions = Vector2.zero;
        currentPositions = Vector2.zero;
        updatedPositions = Vector2.zero;
        touchsensitive = 0f;
    }
    
    void onDragEnable()
    {

        setDragPosition(startPositions, Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(1000), currentPositions);
        if (updatedPositions == currentPositions) startPositions = currentPositions;
        var initialvelocity = (currentPositions.x - updatedPositions.x) / Time.deltaTime;
        //Debug.Log(initialvelocity);
        touchsensitive = initialvelocity / 1000;
        touchsensitive = Mathf.Clamp(touchsensitive, -10f, 10f);
        ////moving right
        //if (updatedPositions.x < currentPositions.x)
        //{
        //    CustomLogs.CC_Log("Moving Right", "red");

        //}else
        ////moving left
        //if(updatedPositions.x > currentPositions.x)
        //{
        //    CustomLogs.CC_Log("Moving Left", "red");
        //}
        //else
        ////no movement just holding 
        //{
        //    CustomLogs.CC_Log("no Movement", "red");
        //    touchsensitive = 0f;
        //}
        OnDrag(touchsensitive);
    }

}
