using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum playerState
{
    idle,
    Interacting,
    waiting,


}


public class InPlayerHandItems
{

}
//[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    //int movementSpeed;
    Rigidbody rb;
    bool isplayermoving;
    InPlayerHandItems inHand;

    public bool isPlayerHandEmpty => inHand == null;
    private void Awake()
    {
        InputManager.OnMovementInput -= GoToPositions;
        InputManager.OnMovementInput += GoToPositions;
    }
    private void OnDestroy()
    {
        InputManager.OnMovementInput -= GoToPositions;
        
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();        
    }
    private void Update()
    {        
        isplayermoving=gameObject.GetComponent<NavMeshAgent>().velocity!=Vector3.zero;
    }
    void onInputRecived()
    {
        RaycastHit ray;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out ray, 1000f);
        //Debug.Log($"<color=cyan>{ray.point}</color>");
        GoToPositions(ray.point);
    }
    public void GoToPositions(Vector3 _pos)
    {
        //rb.MovePosition(_pos);
        Vector3Int newint = new Vector3Int((int)_pos.x, (int)_pos.y, (int)_pos.z);
        gameObject.GetComponent<NavMeshAgent>().SetDestination(newint);
    }
}
