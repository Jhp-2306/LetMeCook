using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : InteractiveBlock
{
    /// <summary>
    /// This Script Validates the Ai Order and Purchase 
    /// Main Link Between the player and NPC scriptes
    /// </summary>
    [SerializeField] Transform AiLookAtMe;

    [SerializeField] NPC.NPC myClient;
    public Transform AILookAtMe()
    {
        return AiLookAtMe;
    }
    //AI place the order 
    public void SetClient(NPC.NPC client)
    {
        myClient = client;
        //trigger the and send the npc out
    }
    private void Update()
    {
        //if(myClient != null)
        //{
        //    Debug.Log(CustomLogs.CC_TagLog("Counter", $"{myClient.namedis.text}"));
        //}
    }
    public void OnPlayerReached()
    {
        //if Player entered then check for the dish in hand
        var player = GameDataDNDL.Instance.GetPlayer();
        if (player != null) {
            var Plate = player.InHand.GetGameObject().GetComponent<Plate>();
            if (Plate!=null &&myClient.GetOrderList().Contains(Plate.GetFinalDish())){
                myClient.OnOrderComplete(Plate.GetFinalDish(),Plate.GetFinalPrice());
                player.RemoveFromHand();
            }
        }
    }
    //Player Deliver the order here
}
