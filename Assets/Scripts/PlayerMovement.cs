using ASPathFinding;
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

    public float speed = 20;
    public float turnSpeed;
    public float turnDst ;
    public float stoppingDst ;
    public ASPathFinding.Path path;
    int crnt_targetIndex;

    Vector3 target;
    const float pathUpdateMoveThreshold = 0.5f;
    const float minPathUpdateTime = 0.2f;
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
        StartCoroutine(UpdatePath());
    }
    private void Update()
    {
        isplayermoving = gameObject.GetComponent<NavMeshAgent>().velocity != Vector3.zero;
    }
    void onInputRecived()
    {
        RaycastHit ray;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out ray, 1000f);
        Debug.Log($"<color=cyan>{ray.point}</color>");
        GoToPositions(ray.point);
    }
    public void GoToPositions(Vector3 _pos)
    {
        Debug.Log($"<color=cyan>{_pos}</color>");
        //rb.MovePosition(_pos);
        Vector3 newint = new Vector3((int)_pos.x, transform.position.y, (int)_pos.z);
        //Debug.Log($"<color=cyan>{newint}</color>");
        //gameObject.GetComponent<NavMeshAgent>().SetDestination(newint);
        target = newint;
    }

    #region A*Code
    Coroutine followpath;
    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new ASPathFinding.Path(waypoints, transform.position, turnDst,stoppingDst);
            if (followpath != null)
            {
                StopCoroutine(followpath);
                followpath = null;
            }
            if (followpath == null)
                followpath = StartCoroutine(FollowPath());
        }
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        PathManager.RequestPath(transform.position, target, OnPathFound);
        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target;
        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathManager.RequestPath(transform.position, target, OnPathFound);
                targetPosOld = target;
            }
        }
    }
    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        //transform.LookAt(path.lookPoints[0]);

        float speedPercent = 1;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {

                //if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                //{
                //    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                //    if (speedPercent < 0.01f)
                //    {
                //        followingPath = false;
                //    }
                //}

                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed  /*speedPercent*/, Space.Self);
            }

            yield return null;

        }
    }

    #endregion
    private void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
