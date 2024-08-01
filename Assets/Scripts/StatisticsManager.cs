using System.Collections;
using System.Collections.Generic;
using SmartifyOS.SaveSystem;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    private static float startTime;

    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        float runtimeInSeconds = Time.time - startTime;
        if (runtimeInSeconds >= 60)
        {
            SaveManager.Load().statistics.totalMinutes += 1;
            startTime = Time.time;
        }

        if (LiveDataController.speedKmh > SaveManager.Load().statistics.topSpeed)
        {
            SaveManager.Load().statistics.topSpeed = LiveDataController.speedKmh;
        }
    }

    public static void SetTopSpeed(float topSpeed)
    {
        SaveManager.Load().statistics.topSpeed = topSpeed;
    }

    public static void SetTotalMinutes(int totalMinutes)
    {
        SaveManager.Load().statistics.totalMinutes = totalMinutes;
    }

    public static void SetBest0To100(double best0To100)
    {
        SaveManager.Load().statistics.best0To100Ms = best0To100;
    }

    public static double GetBest0To100()
    {
        return SaveManager.Load().statistics.best0To100Ms;
    }

    public static float GetTopSpeed()
    {
        return SaveManager.Load().statistics.topSpeed;
    }

    public static int GetTotalMinutes()
    {
        return SaveManager.Load().statistics.totalMinutes;
    }
}
