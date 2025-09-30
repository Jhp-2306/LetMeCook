using ASPathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

namespace NPC
{

    //[RequireComponent(typeof(NavMeshAgent))]
    public class NPC : MonoBehaviour
    {
        int id;
        string npcName;
        //NavMeshAgent Agent;
        bool isNPCMoving;
        bool isRoaming;
        Coroutine myCor;

        public TMPro.TextMeshProUGUI namedis,iddis;

        public GameObject CurrentPlatform;

        //A* Path Finding variables
        public float speed = 20f;
        public float turnSpeed = 5f;
        public float turnDst = 2f;
        public float stoppingDst = 0.5f;
        Path path;
        Vector3 target;
        Coroutine followPathCoroutine;
        bool isPlayerMoving;
        const float pathUpdateMoveThreshold = 0.5f;
        const float minPathUpdateTime = 0.2f;
        public void Start()
        {
            //Agent = GetComponent<NavMeshAgent>();
            //gameObject.SetActive(false);
            StartCoroutine(UpdatePath());
        }
        public void SetNPC(string _npcname,int _id)
        {
            npcName=_npcname;
            id = _id;
            namedis.text = npcName;
            iddis.text= id.ToString();
        }
        public void SetNPC(Vector3 pos, bool isroaming,GameObject myplatform=null)
        {
            this.gameObject.SetActive(true);
            if(myCor != null)
            {
                StopCoroutine(myCor);
            }
            gameObject.SetActive(true);
            isRoaming = isroaming;
            CurrentPlatform = myplatform;
            Vector3 snappedPos = new Vector3(pos.x, transform.position.y, pos.z);
            target = snappedPos;
            myCor=StartCoroutine(UpdatePath());
        }

        //IEnumerator GoToPositions(Vector3 _pos)
        //{
        //    //Agent = GetComponent<NavMeshAgent>();
        //    //Vector3Int newint = new Vector3Int((int)_pos.x, (int)_pos.y, (int)_pos.z);
        //    //Agent.SetDestination(newint);
        //    //while (isRoaming)
        //    //{
        //    //    Agent.SetDestination(newint);
        //    //yield return new WaitUntil(() => Agent.velocity == Vector3.zero);
        //    yield return null;
        //    //    newint = NPCManager.Instance.GetNPCRandomCood();
        //    //}
        //}
        #region A* Pathfinding

        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (followPathCoroutine != null)
            {
                StopCoroutine(followPathCoroutine);
            }
            if (pathSuccessful && waypoints.Length > 0)
            {
                Debug.Log(CustomLogs.CC_TagLog("NPC", "Path Found"));
                path = new Path(waypoints, transform.position, turnDst, stoppingDst);


                followPathCoroutine = StartCoroutine(FollowPath());
            }
        }

        //bool IsInputActive() => isInputActive;
        IEnumerator UpdatePath()
        {
            //yield return new WaitUntil(IsInputActive);
            yield return new WaitForSeconds(0.3f);

            PathManager.NPCRequestPath(transform.position, target, OnPathFound);

            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 lastTargetPos = target;

            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);
                //Debug.Log($"target move conditions{Vector3.SqrMagnitude(target - lastTargetPos) > sqrMoveThreshold},{target},{lastTargetPos}{Vector3.SqrMagnitude(target - lastTargetPos)}");
                if (Vector3.SqrMagnitude(target - lastTargetPos) > sqrMoveThreshold ||
                    (this.gameObject.transform.position != target && target == lastTargetPos && !isPlayerMoving/*&& Vector3.SqrMagnitude(target - lastTargetPos) < sqrMoveThreshold*/))
                {
                    Debug.Log(CustomLogs.CC_TagLog("NPC", "trying to Get the Path"));
                    PathManager.NPCRequestPath(transform.position, target, OnPathFound);
                    isPlayerMoving = true;
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
                       //Debug.Log($"Player Comments We Are At Stop");
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

                while (path != null && path.turnBoundaries != null &&
                    pathIndex < path.turnBoundaries.Length &&
                       path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex)
                    {
                        followingPath = false;
                        isPlayerMoving = false;
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
                
                if (pathIndex < path.lookPoints.Length &&
                    
                    Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                    new Vector2(path.lookPoints[pathIndex].x, path.lookPoints[pathIndex].z)) < 0.2f)
                {
                    transform.position = GetHeightAdjusted(path.lookPoints[pathIndex]);
                    pathIndex++;
                    if (pathIndex == path.finishLineIndex)
                    {
                        followingPath = false;
                        //Debug.Log($"Player Comments We Are At Stop");
                        isPlayerMoving = false;
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
                                //Debug.Log($"Player Comments We Are At Stop");
                                isPlayerMoving = false;
                                transform.position = GetHeightAdjusted(path.lookPoints[pathIndex]);
                                break;
                            }
                        }
                    }

                }
                if (transform.position == target)//PlayerStoped in Location
                {
                    Debug.Log($"AI Reached At the Stop");
                    followingPath = false;
                    isPlayerMoving = false;
                    //OnReachCallback?.Invoke();
                    //if it not roaming then call the disable function
                    if (isRoaming)
                    {
                        DisableMe();
                    }
                    //else call the order function 
                }

                yield return null;
            }

        }

        Vector3 GetHeightAdjusted(Vector3 point)
        {
            return new Vector3(point.x, transform.position.y, point.z);
        }

        #endregion
        void DisableMe()
        {
            if (myCor != null)
            {
                StopCoroutine(myCor);
            }
            NPCManager.Instance.DisableNPC(this);
        }
    }
}
