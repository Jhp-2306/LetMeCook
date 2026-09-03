using ASPathFinding;
using Constants;
using NPC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.Rendering.DebugUI;

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
        public const string Give = "Give";
        public const string AddCoins = "Add_Coins";
        public const string TimeScale = "IG_TimeScale";

        //public static readonly List<string> CommandsList = new List<string> { "help", "f_save", "NG" , "get_cust", "kick_cust" , "u_aspf_grid", "c_aspf_grid", "npc_roaming", "ADD <item> <count>" };

    }
    public class ConsoleCommand
    {
        public string Name;
        public string Description;
        public Func<string[], string> AutoComplete;
        List<string> Items = new List<string>();
        public Action<string[]> Execute;

        public ConsoleCommand(string name, string description, Action<string[]> execute, List<string> item = null)
        {
            Name = name;
            Description = description;
            Execute = execute;
            Items = item ?? new List<string>();
            AutoComplete = args =>
            {
                // User is still typing the command
                if (args.Length == 1)
                    return name;

                // User is typing the item
                if (args.Length == 2)
                {
                    string search = args[1];
                    //if(Items==null)
                    foreach (string item in Items)
                    {
                        if (item.StartsWith(search, StringComparison.OrdinalIgnoreCase))
                            return $"{name} {item}";
                    }

                    return "";
                }

                // User finished item, typing count
                if (args.Length == 3)
                {
                    return $"{name} {args[1]}";
                }

                return "";
            };
        }
    }
    private Dictionary<string, ConsoleCommand> commands;

    private void Awake()
    {
        commands = new Dictionary<string, ConsoleCommand>(StringComparer.OrdinalIgnoreCase)
    {
        {
            IGCCommands.help,
            new ConsoleCommand(IGCCommands.help, "Shows all commands", args =>
            {
                foreach (var cmd in commands.Values)
                    SendMsg($"{cmd.Name} - {cmd.Description}");
            })
        },

        {
            IGCCommands.Fsave,
            new ConsoleCommand(IGCCommands.Fsave, "Force save the game", args =>
            {
                GameSaveDNDL.Instance.ForceSave();
            })
        },

        {
            IGCCommands.NewGame,
            new ConsoleCommand(IGCCommands.NewGame, "Start new game", args =>
            {
                GameSaveDNDL.Instance.NewGame();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            })
        },

        {
            IGCCommands.Npc_Roaming,
            new ConsoleCommand( IGCCommands.Npc_Roaming, "NPC Roaming Patten", args =>
            {
                NPCManager.Instance.IGC_NPC_ROAMING();
            })
        },
        {
            IGCCommands.UpdateASPFGrid,
            new ConsoleCommand(IGCCommands.UpdateASPFGrid, "update the NaveMesh A*path Data", args =>
            {
                ASPF.updateGrid();
            })
        },
        {
            IGCCommands.CreateASPFGrid,
            new ConsoleCommand(IGCCommands.CreateASPFGrid, "Create a NaveMesh A*path Data", args =>
            {
                ASPF.CreateGrid();
            })
        },
        {
            IGCCommands.KickCustomer,
            new ConsoleCommand(IGCCommands.KickCustomer, "kick_cust <id>", args =>
            {
                if (args.Length < 2)
                {
                    SendMsg("Usage: kick_cust <id>");
                    return;
                }

                NPCManager.Instance.IGC_NPC_Back_Roaming(int.Parse(args[1]));
            })
        },

        {
            IGCCommands.Give,
            new ConsoleCommand(IGCCommands.Give, "Give <item> to the player hands", args =>
            {
                if (args.Length < 2)
                {
                    SendMsg($"Give {args[1]} to player hand");
                    return;
                }

                string item = args[1];
                if (Enum.TryParse<IngredientType>(args[1], true, out IngredientType ingredient))
                {
                    GameDataDNDL.Instance.TesterToolCommandGiveIngredientToPlayer(ingredient);
                }
                else
                {
                    SendMsg($"Unknown ingredient: {args[1]}");
                }
            },
            Enum.GetNames(typeof(IngredientType)).ToList()
            )
        },
        {
            IGCCommands.AddCoins,
            new ConsoleCommand(IGCCommands.AddCoins, "Add Coins", args =>
            {
                if (args.Length < 2)
                {
                    SendMsg("Failed Adding Coins need int");
                    return;
                }

                GameDataDNDL.Instance.AddCurrency(int.Parse(args[1]));
            })
        },
        {
            IGCCommands.TimeScale,
            new ConsoleCommand(IGCCommands.TimeScale, "in-Game Timer for a Day <float Minutes>", args =>
            {
                if (args.Length < 2)
                {
                    SendMsg("Failed Change ig-time Scale");
                    return;
                }
                TimeManagementDNDL.Instance.TesterTool_SetIRLMinsForInGameDay(int.Parse(args[1]));
            })
        },
    };
    }
    public virtual void ProcessCommand()
    {
        if (string.IsNullOrWhiteSpace(input_Field.text))
            return;

        string[] args = input_Field.text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (commands.TryGetValue(args[0], out var command))
        {
            try
            {
                SendMsg($"Executing {command.Name}");
                command.Execute(args);
                SendMsg($"Executed {command.Name}");
            }
            catch (Exception e)
            {
                SendMsg(e.Message);
            }
        }
        else
        {
            SendMsg($"Unknown command: {args[0]}");
        }

        input_Field.text = "";
    }

    public GameObject SliderPrefab;
    public TMPro.TMP_InputField input_Field;

    public GameObject ParentforText;

    public GameObject Content, Space;
    public TextMeshProUGUI symbol;
    public TextMeshProUGUI hintText;
    public Color Red, Green;
    public Image openCloseBTn;
    bool isopen = false;
    Stack<string> Logs = new Stack<string>();
    public List<string> t = new List<string> { "help" };
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
        input_Field.onValueChanged.AddListener(OnInputChanged);
        //StartCoroutine(InitMsg());
    }

    //IEnumerator InitMsg()
    //{
    //    SendMsg("Setting up...");
    //    yield return new WaitForSeconds(10);
    //    SendMsg("Welcome world");
    //}

    //public void ProcessCommand()
    //{
    //    if (input_Field.text != null)
    //    {
    //        char[] spliter = { ' ', };
    //        string[] Processedtext=input_Field.text.Split(spliter);
    //        string commandKey = Processedtext[0];
    //        switch (commandKey)
    //        {
    //            case IGCCommands.Fsave:
    //                ExecuteCommand("Force Save", () => { GameSaveDNDL.Instance.ForceSave(); });
    //                break;
    //            case IGCCommands.NewGame:
    //                ExecuteCommand( "New Game", () => { GameSaveDNDL.Instance.NewGame();
    //                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //                });
    //                break;
    //            case IGCCommands.GetCustomer:
    //                ExecuteCommand(IGCCommands.GetCustomer, () => { NPCManager.Instance.IGC_Get_NPC_Customer(); });
    //                break;
    //            case IGCCommands.KickCustomer:
    //                ExecuteCommand(IGCCommands.KickCustomer, () => { NPCManager.Instance.IGC_NPC_Back_Roaming(int.Parse(Processedtext[1])); });
    //                break;
    //            case IGCCommands.Npc_Roaming:
    //                ExecuteCommand(IGCCommands.Npc_Roaming, () => { NPCManager.Instance.IGC_NPC_ROAMING(); });
    //                break;
    //            case IGCCommands.UpdateASPFGrid:
    //                ExecuteCommand(IGCCommands.UpdateASPFGrid, () => { ASPF.updateGrid(); });
    //                break;
    //            case IGCCommands.CreateASPFGrid:
    //                ExecuteCommand(IGCCommands.CreateASPFGrid, () => { ASPF.CreateGrid(); });
    //                break;
    //            case IGCCommands.help:
    //                ExecuteCommand(IGCCommands.help, () => { foreach (var t in IGCCommands.CommandsList) SendMsg(t); });
    //                break;
    //            default:
    //                SendMsg($"Failed to execute: Wrong command {input_Field.text}");
    //                break;
    //        }

    //    }
    //}

    //void ExecuteCommand(string commandname,Action method)
    //{
    //    SendMsg($"Executing {commandname}");
    //    try
    //    {
    //        method();
    //        SendMsg($"Executed {commandname}");
    //    }
    //    catch
    //    {
    //        SendMsg($"Failed to execute {commandname}");
    //    }
    //}

    public void ConsoleOpenCloseControleClicked()
    {
        isopen = !isopen;
        ConsoleOpenCloseControle(isopen);
    }

    void ConsoleOpenCloseControle(bool state)
    {
        if (state)
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

    public void OnInputChanged(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            hintText.text = "";
            return;
        }
        string suggestion = GetSuggestion(input);

        if (string.IsNullOrEmpty(suggestion))
        {
            hintText.text = "";
            return;
        }

        if (suggestion.Length <= input.Length)
        {
            hintText.text = "";
            return;
        }

        string remaining = suggestion.Substring(input.Length);

        hintText.text =
            input +
            $"<color=#888888>{remaining}</color>";
    }
    //private string GetSuggestion(string input)
    //{
    //    input = input.ToLower();

    //    foreach (string command in IGCCommands.CommandsList)
    //    {
    //        if (command.ToLower().StartsWith(input))
    //            return command;
    //    }

    //    return "";
    //}
    private string GetSuggestion(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";

        string[] args = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Still typing command
        if (args.Length == 1 && !text.EndsWith(" "))
        {
            foreach (var _cmd in commands.Values)
            {
                if (_cmd.Name.StartsWith(args[0], StringComparison.OrdinalIgnoreCase))
                    return _cmd.Name;
            }

            return "";
        }

        // Command found
        if (commands.TryGetValue(args[0], out var cmd))
        {
            return cmd.AutoComplete?.Invoke(args);
        }

        return "";
    }
}

