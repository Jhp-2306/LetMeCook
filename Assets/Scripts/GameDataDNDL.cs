using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
using Util;

public class GameDataDNDL : Singletonref<GameDataDNDL>
{
    public Material Selected;
    public Material normal;
    public DateTime StartTime;
    public DateTime PreviousGameCloseTime;

    
    private KitchenState m_KitchenState;
    private GameState m_GameState;

    private void Start()
    {
        m_KitchenState = 0;
        m_GameState = GameState.PlayGame;
    }
}
