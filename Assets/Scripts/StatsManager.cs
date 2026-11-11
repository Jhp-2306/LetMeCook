using System.Collections.Generic;
using UnityEngine;
using Util;

[System.Serializable]
public class Stats
{
    public int id;
    public string StatName;
    public int StatPoints;
    public string StatValue;
    public string Descriptions;

    public Stats()
    {
    }

    public Stats(int id, string statName, string descriptions)
    {
        this.id = id;
        StatName = statName;
        //StatPoints = statPoints;
        Descriptions = descriptions;
    }
    public void AddValue(string value = null, int points = 0)
    {
        StatPoints = points;
        StatValue = value;
    }
    public string GetValue()
    {
        if(StatPoints==-1)return StatValue;
        else return StatPoints.ToString();
    }
}
public class StatsManager : Singletonref<StatsManager>
{
    public Dictionary<int,Stats> stats;

    public void Start()
    {
        //Read from Save

        if (stats == null)
        {
            stats = new Dictionary<int, Stats>();
            stats.Add(1,new Stats(001, "day_on_save", "Max Day On A Save"));
            stats.Add(2,new Stats(002, "best_dish", "Best Dish"));
            stats.Add(3,new Stats(003, "perfect_dish_total", "Total No Of Perfect Dish served"));
            stats.Add(4,new Stats(004, "money_collect_on_save", "Max Money Collect On A Save"));
            stats.Add(5,new Stats(005, "money_collect_total", "Total Money Collect"));
            stats.Add(6,new Stats(006, "money_spent_total", "Total Money Spent"));
            stats.Add(7,new Stats(007, "cust_served_total", "Total Customer Served"));
            stats.Add(8,new Stats(008, "cust_served_on_save", "Max Customer Served On A Save"));
            stats.Add(9,new Stats(009, "most_served_dish", "Most Served Dish"));
            stats.Add(10,new Stats(010, "most_bought_ingredient", "Most Bought Ingredient"));
            stats.Add(11,new Stats(011, "most_stars", "Max Star reached On A Save"));
            stats.Add(12,new Stats(012, "time_spend_on_save", "Max Time Spend On A Save"));
            stats.Add(13,new Stats(013, "time_spend_total", "Total Time Spend"));
            Debug.Log(CustomLogs.CC_TagLog("Stats managage", $"stats entered total count {stats.Count}"));
        }
        
    }
    public void SetStat(int id, string value=null,int points = -1)
    {
        stats[id].AddValue(value,points);
    }
    public Stats GetStats(int id) { return stats[id]; }
}


