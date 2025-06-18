using ASPathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;

    public float speed = 20f;
    public float turnSpeed = 5f;
    public float turnDst = 2f;
    public float stoppingDst = 3f;
    bool isInputActive;
    Path path;
    Vector3 target;
    Coroutine followPathCoroutine;

    const float pathUpdateMoveThreshold = 0.5f;
    const float minPathUpdateTime = 0.2f;

    private void Awake()
    {
        InputManager.OnMovementInput -= GoToPosition;
        InputManager.OnMovementInput += GoToPosition;
    }

    private void OnDestroy()
    {
        InputManager.OnMovementInput -= GoToPosition;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //target = new Vector3(15, 1.5f, 19);
        StartCoroutine(UpdatePath());
    }

    public void GoToPosition(Vector3 pos,bool istable)
    {
        isInputActive = true;
        if (target == pos) return;
        Vector3 snappedPos = new Vector3(pos.x, transform.position.y, pos.z);
        target = snappedPos;
        Debug.Log($"<color=cyan>Target set to: {snappedPos}</color>");
    }

    #region A* Pathfinding

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful && waypoints.Length > 0)
        {
            path = new Path(waypoints, transform.position, turnDst, stoppingDst);

            if (followPathCoroutine != null)
            {
                StopCoroutine(followPathCoroutine);
            }

            followPathCoroutine = StartCoroutine(FollowPath());
        }
    }

    bool IsInputActive() => isInputActive;
    IEnumerator UpdatePath()
    {
        yield return new WaitUntil(IsInputActive);
        yield return new WaitForSeconds(0.3f);

        PathManager.RequestPath(transform.position, target, OnPathFound);

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 lastTargetPos = target;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);

            if ((target - lastTargetPos).sqrMagnitude > sqrMoveThreshold)
            {
                PathManager.RequestPath(transform.position, target, OnPathFound);
                lastTargetPos = target;
            }
        }
    }

    IEnumerator FollowPath()
    {
        if (path == null || path.lookPoints == null || path.lookPoints.Length == 0)
        {
            yield break;
        }

        int pathIndex = 0;
        float speedPercent = 1f;
        bool followingPath = true;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

            
            while (path != null && path.turnBoundaries != null &&
                pathIndex < path.turnBoundaries.Length &&
                   path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    transform.position = GetHeightAdjusted(path.lookPoints[pathIndex]);
                    pathIndex++;
                }
                if (pathIndex >= path.turnBoundaries.Length)
                {
                    Debug.LogWarning("pathIndex exceeded turnBoundaries length. Breaking out.");
                    yield break;
                }
            }
           // Debug.Log($"path{path.lookPoints[pathIndex]}, distance {Vector2.Distance(new Vector2(transform.position.x,transform.position.z), new Vector2(path.lookPoints[pathIndex].x, path.lookPoints[pathIndex].z))}");
            // Fallback: if we're near the next point but haven't crossed, force progress
            if (pathIndex < path.lookPoints.Length &&
                /*Vector3.Distance(transform.position, path.lookPoints[pathIndex])*/
                Vector2.Distance(new Vector2(transform.position.x, transform.position.z), 
                new Vector2(path.lookPoints[pathIndex].x, path.lookPoints[pathIndex].z)) < 0.2f)
            {
                transform.position = GetHeightAdjusted( path.lookPoints[pathIndex]);
                pathIndex++;
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
            }
            if (followingPath)
            {
                if (pathIndex <= path.finishLineIndex)
                {

                Vector3 targetPoint = GetHeightAdjusted(path.lookPoints[pathIndex]);
                Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);

                if (pathIndex >= path.slowDownIndex && stoppingDst > 0 && path.turnBoundaries.Length > 1)
                {
                    float distanceToEnd = path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D);
                    speedPercent = Mathf.Clamp01(distanceToEnd / stoppingDst);

                    if (speedPercent < 0.05f)
                    {
                        followingPath = false;
                        transform.position = GetHeightAdjusted(path.lookPoints[pathIndex]);
                        break;
                    }
                }
                }


            }


            yield return null;
        }

        // Snap to final point if very close
        if (Vector3.Distance(transform.position, path.lookPoints[^1]) < 0.3f)
        {
            Debug.Log($"pathccheck{path.lookPoints[^1]}");
            transform.position = path.lookPoints[^1];
        }
    }

    Vector3 GetHeightAdjusted(Vector3 point)
    {
        return new Vector3(point.x, transform.position.y, point.z);
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
public enum playerState
{
    idle,
    Interacting,
    waiting,


}
[System.Serializable]
public enum typeofhandheld
{
    veg, meal, box
}


