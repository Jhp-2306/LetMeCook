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
        private Queue<GameObject> DestPadsQueue;
        //private Queue<GameObject> DestOnCooldown;

        public List<NPC> npc;
        Queue<NPC> roaming_npc;

        public int coolDownTimeInSec = 3;

        public Vector3Int roam_AnchorMin, roam_AnchorMax;
        private void Start()
        {
            CreatIDs();
            NpcRoaming();
            DestPadsQueue = new Queue<GameObject>();
            foreach(var t in DestPads)
            {
                DestPadsQueue.Enqueue(t);
            }
        }
        void CreatIDs()
        {
            for (int i = 0;i<npc.Count;i++)
            {
                npc[i].SetNPC($"name{i}]", i);
            }
        }

        public void NPC_Back_Roaming(int id)
        {
            DestPadsQueue.Enqueue(npc[id].CurrentPlatform);
            npc[id].SetNPC(GetRandomCoods(), true);
            roaming_npc.Enqueue(npc[id]);

        }
        public void Get_NPC_Customer()
        {
           var npcCustomer= roaming_npc.Dequeue();
            if(DestPadsQueue.Count > 0)
            {
                var t = DestPadsQueue.Dequeue();
                npcCustomer.SetNPC(t.transform.position, false, t);
            }
        }
       
        #region Roaming
        public void NpcRoaming()
        {
            roaming_npc = new Queue<NPC>();
            foreach(var t in npc)
            {
                t.SetNPC(GetRandomCoods(),true);
                roaming_npc.Enqueue(t);
            }
        }
        public Vector3Int GetNPCRandomCood()
        {
            return GetRandomCoods();
        }
        Vector3Int GetRandomCoods()
        {
            int x = (int)UnityEngine.Random.Range(roam_AnchorMin.x, roam_AnchorMax.x);
            int z = (int)UnityEngine.Random.Range(roam_AnchorMin.z, roam_AnchorMax.z);
            return new Vector3Int(x,0, z);
        }
        #endregion
    }
}
