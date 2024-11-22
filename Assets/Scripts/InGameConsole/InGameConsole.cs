using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.WSA;
using NPC;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using System;

public class InGameConsole : MonoBehaviour
{

    public static class IGCCommands
    {
        public const string Fsave = "Fsave";
        public const string NewGame = "NG";
        public const string GetCustomer = "Gcust";
        public const string KickCustomer = "Kcust";
    }


    public GameObject SliderPrefab;
    public TMPro.TMP_InputField input_Field;

    public GameObject ParentforText;

    public GameObject Content, Space;
    public TextMeshProUGUI symbol;
    public Color Red, Green;
    public Image openCloseBTn;
    bool isopen = false;
    Stack<string> Logs = new Stack<string>();
    public void SendMsg(string msg)
    {
        Logs.Push(msg);
        var obj = Instantiate(SliderPrefab, ParentforText.transform);
        obj.GetComponent<InGameConsoleSlider>().SetText(msg);
    }
    private void Start()
    {
        SendMsg("Starting...");
        isopen = false;
        ConsoleOpenCloseControle(isopen);
        //StartCoroutine(InitMsg());
    }

    IEnumerator InitMsg()
    {
        SendMsg("Setting up...");
        yield return new WaitForSeconds(10);
        SendMsg("Welcome world");
    }

    public void ExecuteCommand()
    {
        if (input_Field.text != null)
        {
            char[] spliter = { ' ', };
            string[] Processedtext=input_Field.text.Split(spliter);
            string commandKey = Processedtext[0];
            switch (commandKey)
            {
                case IGCCommands.Fsave:
                    SendMsg($"Executing Force Save");
                    try
                    {
                        GameSaveDNDL.Instance.ForceSave();
                        SendMsg($"Executed Force Save");
                    }
                    catch
                    {
                        SendMsg("Failed to execute Force Save");
                    }

                    break;
                case IGCCommands.NewGame:
                    SendMsg($"Executing New Game");
                    try
                    {
                        GameSaveDNDL.Instance.NewGame();
                        SendMsg($"Executed New Game");
                    }
                    catch
                    {
                        SendMsg("Failed to execute New Game");
                    }

                    break;
                case IGCCommands.GetCustomer:
                    SendMsg($"Executing {IGCCommands.GetCustomer}");
                    try
                    {
                        NPCManager.Instance.Get_NPC_Customer();
                        SendMsg($"Executed {IGCCommands.GetCustomer}");
                    }
                    catch
                    {
                        SendMsg($"Failed to execute {IGCCommands.GetCustomer}");
                    }

                    break;
                case IGCCommands.KickCustomer:
                    SendMsg($"Executing {IGCCommands.KickCustomer}");
                    try
                    {
                        NPCManager.Instance.NPC_Back_Roaming(int.Parse(Processedtext[1]));
                        SendMsg($"Executed {IGCCommands.KickCustomer}");
                    }
                    catch
                    {
                        SendMsg($"Failed to execute {IGCCommands.KickCustomer}");
                    }

                    break;
                default:
                    SendMsg($"Failed to execute: Wrong command {input_Field.text}");
                    break;
            }
        }
    }
    
    public void ConsoleOpenCloseControleClicked()
    {
        isopen = !isopen;
        ConsoleOpenCloseControle(isopen);
    }

    void ConsoleOpenCloseControle(bool state)
    {
        if(state)
        {
            Content.SetActive(true);
            Space.SetActive(true);
            symbol.text = "X";
            openCloseBTn.color = Red;
        }
        else
        {
            Content.SetActive(false);
            Space.SetActive(false);
            symbol.text = ">>";
            openCloseBTn.color = Green;
        }
    }

}

