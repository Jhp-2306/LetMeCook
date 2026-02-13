using ASPathFinding;
using NPC;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class InGameConsole : MonoBehaviour
{

    public static class IGCCommands
    {
        public const string help = "help";
        public const string Fsave = "f_save";
        public const string NewGame = "NG";
        public const string GetCustomer = "get_cust";
        public const string KickCustomer = "kick_cust";
        public const string UpdateASPFGrid = "u_aspf_grid";
        public const string CreateASPFGrid = "c_aspf_grid";
        public const string Npc_Roaming = "npc_roaming";

        public static readonly List<string> CommandsList = new List<string> { "help", "f_save", "NG" , "get_cust", "kick_cust" , "u_aspf_grid", "c_aspf_grid", "npc_roaming" };

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
    public List<string> t =new List<string>{ "help"};
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

    public void ProcessCommand()
    {
        if (input_Field.text != null)
        {
            char[] spliter = { ' ', };
            string[] Processedtext=input_Field.text.Split(spliter);
            string commandKey = Processedtext[0];
            switch (commandKey)
            {
                case IGCCommands.Fsave:
                    ExecuteCommand("Force Save", () => { GameSaveDNDL.Instance.ForceSave(); });
                    break;
                case IGCCommands.NewGame:
                    ExecuteCommand( "New Game", () => { GameSaveDNDL.Instance.NewGame();
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    });
                    break;
                case IGCCommands.GetCustomer:
                    ExecuteCommand(IGCCommands.GetCustomer, () => { NPCManager.Instance.IGC_Get_NPC_Customer(); });
                    break;
                case IGCCommands.KickCustomer:
                    ExecuteCommand(IGCCommands.KickCustomer, () => { NPCManager.Instance.IGC_NPC_Back_Roaming(int.Parse(Processedtext[1])); });
                    break;
                case IGCCommands.Npc_Roaming:
                    ExecuteCommand(IGCCommands.Npc_Roaming, () => { NPCManager.Instance.IGC_NPC_ROAMING(); });
                    break;
                case IGCCommands.UpdateASPFGrid:
                    ExecuteCommand(IGCCommands.UpdateASPFGrid, () => { ASPF.updateGrid(); });
                    break;
                case IGCCommands.CreateASPFGrid:
                    ExecuteCommand(IGCCommands.CreateASPFGrid, () => { ASPF.CreateGrid(); });
                    break;
                case IGCCommands.help:
                    ExecuteCommand(IGCCommands.help, () => { foreach (var t in IGCCommands.CommandsList) SendMsg(t); });
                    break;
                default:
                    SendMsg($"Failed to execute: Wrong command {input_Field.text}");
                    break;
            }
            
        }
    }

    void ExecuteCommand(string commandname,Action method)
    {
        SendMsg($"Executing {commandname}");
        try
        {
            method();
            SendMsg($"Executed {commandname}");
        }
        catch
        {
            SendMsg($"Failed to execute {commandname}");
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

