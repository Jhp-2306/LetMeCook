using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASPathFinding
{
    [RequireComponent(typeof(ASPF))]
public class PathManager : MonoBehaviour
{
        Queue<PathRequest> requestQueue = new Queue<PathRequest>();
        PathRequest currentRequest;
        static PathManager instance;

        Queue<PathRequest> AIrequestQueue = new Queue<PathRequest>();
        PathRequest AIcurrentRequest;
        bool AIisProcessingPath;
        ASPF aspf;
        bool isProcessingPath;
        private void Awake()
        {
            instance = this;
            aspf = GetComponent<ASPF>();
        }
        public static void RequestPath(Vector3 _pathStart, Vector3 _pathEnd, Action<Vector3[], bool> _callback)
        {
            PathRequest newRequest = new PathRequest(_pathStart,_pathEnd,_callback);
            instance.requestQueue.Enqueue(newRequest);
            instance.tryProcessNext();
        }
        void tryProcessNext()
        {
            if(!isProcessingPath&&requestQueue.Count > 0)
            {
                currentRequest = requestQueue.Dequeue();
                isProcessingPath = true;
                aspf.StartFindPath(currentRequest.pathStart, currentRequest.pathEnd);
            }
        }

        public void FinishProcessingPath(Vector3[] path,bool success)
        {
            currentRequest.callback(path, success);
            isProcessingPath=false;
            tryProcessNext();
        }
        public static void NPCRequestPath(Vector3 _pathStart, Vector3 _pathEnd, Action<Vector3[], bool> _callback)
        {
            PathRequest newRequest = new PathRequest(_pathStart, _pathEnd, _callback);
            instance.AIrequestQueue.Enqueue(newRequest);
            instance.NPCtryProcessNext();
        }
        void NPCtryProcessNext()
        {
            if (!isProcessingPath && AIrequestQueue.Count > 0)
            {
                AIcurrentRequest = AIrequestQueue.Dequeue();
                AIisProcessingPath = true;
                aspf.StartForAIFindPath(AIcurrentRequest.pathStart, AIcurrentRequest.pathEnd);
            }
        }

        public void NPCFinishProcessingPath(Vector3[] path, bool success)
        {
            AIcurrentRequest.callback(path, success);
            AIisProcessingPath = false;
            NPCtryProcessNext();
        }

        struct PathRequest
        {
            public Vector3 pathStart;
            public Vector3 pathEnd;
            public Action<Vector3[],bool> callback;

            public PathRequest(Vector3 _pathStart, Vector3 _pathEnd, Action<Vector3[], bool> _callback)
            {
                this.pathStart = _pathStart;
                this.pathEnd = _pathEnd;
                this.callback = _callback;
            }
        }
}
}
