using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace NPC
{
    public class NPCManager : Singletonref<NPCManager>
    {
        public List<GameObject> DestPads;
        private Queue<Counter> DestPadsQueue;

        public List<NPC> npc;
        Queue<NPC> roaming_npc;

        public int coolDownTimeInSec = 3;
        public float insideTrafficTimer = 10f;
        public Vector3Int roam_AnchorMin, roam_AnchorMax;
        public List<Vector3> LeftSide, RightSide;
        public Transform DoorEntryPosition, DoorExitPosition;
        private void Start()
        {
            CreatIDs();
            roaming_npc = new Queue<NPC>();
            foreach (var t in npc)
            {
                roaming_npc.Enqueue(t);
            }
            DestPadsQueue = new Queue<Counter>();
            foreach (var t in DestPads)
            {
                DestPadsQueue.Enqueue(t.GetComponent<Counter>());
            }
            StartCoroutine(NPC_RoamingTimer());

        }
        void CreatIDs()
        {
            for (int i = 0; i < npc.Count; i++)
            {
                npc[i].SetNPC($"name{i}]", i);
            }
        }

        public void DisableNPC(NPC npc)
        {
            npc.gameObject.SetActive(false);
            roaming_npc.Enqueue(npc);
        }

        IEnumerator NPC_RoamingTimer()
        {
            float TrafficTimer=0f;
            while (roaming_npc.Count > 0)
            {
                yield return new WaitForSeconds(coolDownTimeInSec);
                TrafficTimer += coolDownTimeInSec;
                if (TimeManagementDNDL.Instance.isRoaminghrs)
                {
                    if (TimeManagementDNDL.Instance.CurrentDayPhase == TimeManagementDNDL.DayPhase.KitchenOpen && DestPadsQueue.Count != 0&&TrafficTimer>insideTrafficTimer&&
                        !GameDataDNDL.Instance.isFTUT)
                    {
                        TrafficTimer = 0;
                        //Send NPC inside the shop
                        NPCMovingInsideTheShop();
                    }
                    else
                    {
                        NpcRoaming();
                    }
                }
            }
        }
        public NPC RequestFTUTNPC()
        {
           return NPCMovingInsideTheShop();
        }
        public NPC NPCMovingInsideTheShop()
        {
            if (DestPadsQueue.Count > 0)
            {
                //var temp=roaming_npc.Dequeue();
                var finalLocationGO = DestPadsQueue.Dequeue();
                //return finalLocationGO;
                var temp = roaming_npc.Dequeue();
                Debug.Log(CustomLogs.CC_TagLog("NPC-Manager", $"removing count{DestPadsQueue.Count},{temp.npcName},{finalLocationGO == null}"));
                temp.transform.position = GetNPCRandomCood();
                temp.SetNPC(DoorEntryPosition.position, false, finalLocationGO);
                return temp;
            }
            return null;
        }
        
        public void NPCMovingOutsideTheShop(NPC npc, Counter location)
        {
            DestPadsQueue.Enqueue(location);
            Debug.Log(CustomLogs.CC_TagLog("NPC-Manager", $"adding back count{DestPadsQueue.Count},{location == null}"));
            //npc.SetNPC(GetNPCRandomCood(), true);
        }
        #region IGC Functions
        public void IGC_NPC_Back_Roaming(int id)
        {
            //DestPadsQueue.Enqueue(npc[id].CurrentPlatform);
            npc[id].SetNPC(GetRandomCoods(), true);
            roaming_npc.Enqueue(npc[id]);

        }
        public void IGC_Get_NPC_Customer()
        {
            NPCMovingInsideTheShop();
        }
        public void IGC_NPC_ROAMING()
        {
            NpcRoaming();
        }
        #endregion
        #region Roaming
        public void NpcRoaming()
        {
            //Set a Spawn Side and move him from one connor to another
            bool isLeftToRight = UnityEngine.Random.value <= 0.5f;
            if (isLeftToRight)
            {
                //Spawn from Left and end in Right
                var temp = roaming_npc.Dequeue();
                temp.gameObject.transform.position = LeftSide[UnityEngine.Random.RandomRange(0, LeftSide.Count)];
                temp.SetNPC(RightSide[UnityEngine.Random.RandomRange(0, RightSide.Count)], true);
            }
            else
            {
                //Spawn from Right and end in Left
                var temp = roaming_npc.Dequeue();
                temp.gameObject.transform.position = RightSide[UnityEngine.Random.RandomRange(0, RightSide.Count)];
                temp.SetNPC(LeftSide[UnityEngine.Random.RandomRange(0, LeftSide.Count)], true);
            }
        }
        public Vector3 GetNPCRandomCood()
        {
            return GetRandomCoods();
        }
        Vector3 GetRandomCoods()
        {
            return UnityEngine.Random.value < 0.5 ? LeftSide[UnityEngine.Random.RandomRange(0, LeftSide.Count)] : RightSide[UnityEngine.Random.RandomRange(0, RightSide.Count)];
        }
        #endregion
    }
}
