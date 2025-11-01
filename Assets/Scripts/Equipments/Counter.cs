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
    public Transform AILookAtMe()
    {
        return AiLookAtMe;
    }
    //AI place the order 
    //Player Deliver the order here
}
