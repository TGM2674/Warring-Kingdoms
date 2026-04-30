using Godot;
using System;
using System.Collections.Generic;

public class Modifiers
{
    public enum Type
    {
        Raining,
        Snowing,
        NoWeather,
        HighGround,
        Swamp,
        NoTerrain
    }

    private static bool isDay = true;
    private static Type activeWeather = Type.NoWeather;
    private static Type player1Terrain = Type.NoTerrain;
    private static Type player2Terrain = Type.NoTerrain;
    private static Type lastWeather = Type.NoWeather;
    private static Type lastPlayer1Terrain = Type.NoTerrain;
    private static Type lastPlayer2Terrain = Type.NoTerrain;

    private static readonly Type[] weatherPool = { Type.NoWeather, Type.Raining, Type.Snowing };
    private static readonly Type[] terrainPool = { Type.NoTerrain, Type.HighGround, Type.Swamp };

    public static void Reset()
    {
        isDay = true;
        activeWeather = Type.NoWeather;
        lastWeather = Type.NoWeather;
        player1Terrain = Type.NoTerrain;
        player2Terrain = Type.NoTerrain;
        lastPlayer1Terrain = Type.NoTerrain;
        lastPlayer2Terrain = Type.NoTerrain;
    }

    public static void AdvanceDayNight() => isDay = !isDay;
    public static bool IsDay() => isDay;

    public static void RollWeather()
    {
        List<Type> options = new List<Type>();
        foreach (Type t in weatherPool)
            if (t != lastWeather) options.Add(t);

        int index = new Random().Next(options.Count);
        lastWeather = activeWeather;
        activeWeather = options[index];
    }

    public static Type GetWeather() => activeWeather;

    public static void RollTerrain()
    {
        player1Terrain = RollFromPool(terrainPool, lastPlayer1Terrain);
        player2Terrain = RollFromPool(terrainPool, lastPlayer2Terrain);

        lastPlayer1Terrain = player1Terrain;
        lastPlayer2Terrain = player2Terrain;
    }

    public static Type GetTerrain(int playerIndex)
    {
        return playerIndex == 1 ? player1Terrain : player2Terrain;
    }

    private static Type RollFromPool(Type[] pool, Type exclude)
    {
        List<Type> options = new List<Type>();
        foreach (Type t in pool)
            if (t != exclude) options.Add(t);

        return options[new Random().Next(options.Count)];
    }
}