using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NPC
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class NPC : MonoBehaviour
    {
        int id;
        string npcName;
        NavMeshAgent Agent;
        bool isNPCMoving;
        bool isRoaming;
        Coroutine myCor;

        public TMPro.TextMeshProUGUI namedis,iddis;

        public GameObject CurrentPlatform;

        public void Start()
        {
            Agent = GetComponent<NavMeshAgent>();
            //gameObject.SetActive(false);
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
            if(myCor != null)
            {
                StopCoroutine(myCor);
            }
            gameObject.SetActive(true);
            isRoaming = isroaming;
            CurrentPlatform = myplatform;
            myCor=StartCoroutine(GoToPositions(pos));
        }

        IEnumerator GoToPositions(Vector3 _pos)
        {
            Agent = GetComponent<NavMeshAgent>();
            Vector3Int newint = new Vector3Int((int)_pos.x, (int)_pos.y, (int)_pos.z);
            Agent.SetDestination(newint);
            while (isRoaming)
            {
                Agent.SetDestination(newint);
                yield return new WaitUntil(() => Agent.velocity == Vector3.zero);
                newint = NPCManager.Instance.GetNPCRandomCood();
            }
        }
    }
}
