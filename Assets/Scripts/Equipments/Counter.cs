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
        //if(myClient==null) 
        //    myClient=new NPC.NPC();

        myClient = client;
        Debug.Log("Setting client here"+ myClient.GetOrderList().Count+ this.gameObject.name);
        //trigger the and send the npc out
    }
    private void Update()
    {
        if(myClient != null)
        {
            Debug.Log(CustomLogs.CC_TagLog("Counter", $"{myClient.namedis.text}"));
        }
    }
    public void OnPlayerReached()
    {
        //if Player entered then check for the dish in hand
        //var player = GameDataDNDL.Instance.GetPlayer();
        if (player != null)
        {
            if (player.InHand.GetGameObject().GetComponent<Plate>() != null)
            {
                var plate = player.InHand.GetGameObject().GetComponent<Plate>();
                Debug.Log("PlayerReached the Counter");

                if (myClient!=null&& myClient.GetOrderList().Contains(plate.GetFinalDish()))
                {
                    Debug.Log("Setting client here" + myClient.GetOrderList().Count + this.gameObject.name);
                    if(myClient.OnOrderComplete(plate.GetFinalDish(), plate.GetFinalPrice(),plate.RateTheDish()))
                    {
                        myClient=null;
                    }
                    player.RemoveFromHand();
                }
            }
        }
    }
    public void CalculateQualityOfFood()
    {
        
    }
}
