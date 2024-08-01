using System;
using System.Collections.Generic;
using SmartifyOS.SaveSystem;
using SmartifyOS.UI;
using UnityEngine;

public class StatisticsUIWindow : BaseUIWindow
{
    [SerializeField] private StatisticsEntry entryPrefab;
    [SerializeField] private Transform entriesParent;

    private List<StatisticsEntry> entriesList = new List<StatisticsEntry>();

    [SerializeField] private Sprite speedIcon;
    [SerializeField] private Sprite timeIcon;
    [SerializeField] private int time;

    private void Start()
    {
        Init();
    }

    protected override void OnShow()
    {
        ClearEntries();

        AddEntry("Top Speed: ", StatisticsManager.GetTopSpeed().ToString("0") + " km/h", speedIcon);
        AddEntry("Total Time: ", ConvertMinutesToReadableTime(StatisticsManager.GetTotalMinutes()), timeIcon);

        AddEntry("Best 0-100: ", FormatMs(StatisticsManager.GetBest0To100()) + " sec", speedIcon);
    }

    private void AddEntry(string name, string info, Sprite icon)
    {
        var entry = Instantiate(entryPrefab, entriesParent);
        entry.gameObject.SetActive(true);
        entry.Init(name, icon, info);
        entriesList.Add(entry);
    }

    private void ClearEntries()
    {
        foreach (var entry in entriesList)
        {
            Destroy(entry.gameObject);
        }

        entriesList.Clear();
    }

    private string FormatMs(double ms)
    {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(ms);
        return string.Format("{0:D2}.{1:D3}", timeSpan.Seconds, timeSpan.Milliseconds);
    }

    private string ConvertMinutesToReadableTime(int totalMinutes)
    {
        if (totalMinutes < 1)
        {
            return "Less than a minute";
        }

        TimeSpan timeSpan = TimeSpan.FromMinutes(totalMinutes);

        int years = timeSpan.Days / 365;
        int months = (timeSpan.Days % 365) / 30;
        int days = (timeSpan.Days % 365) % 30;
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;

        string readableTime = "";

        if (years > 0)
        {
            readableTime += $"{years} year{(years > 1 ? "s" : "")} ";
        }
        if (months > 0)
        {
            readableTime += $"{months} month{(months > 1 ? "s" : "")} ";
        }
        if (days > 0)
        {
            readableTime += $"{days} day{(days > 1 ? "s" : "")} ";
        }
        if (hours > 0)
        {
            readableTime += $"{hours} hour{(hours > 1 ? "s" : "")} ";
        }
        if (minutes > 0)
        {
            readableTime += $"{minutes} minute{(minutes > 1 ? "s" : "")}";
        }

        return readableTime.Trim();
    }

    protected override void HandleWindowOpened(BaseUIWindow window)
    {
        //Add all windows that should hide this window when they open
        if (window.IsWindowOfType(typeof(AppListUIWindow)))
        {
            Hide(true);
        }
    }
}
