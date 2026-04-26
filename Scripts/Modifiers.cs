using Godot;
using System;
using System.Collections.Generic;

public class Modifiers
{
    public enum Effects
    {
        Buff,
        Nerf
    }
    
    public enum Type
    {
        Day,
        Night,
        Raining,
        Snowing,
        NoWeather,
        HighGround,
        Swamp,
        NoTerrain
    }

    // Global modifiers (same for both players)
    private static Type activeDayNight = Type.Day;
    private static Type activeWeather = Type.NoWeather;

    // Per-player terrain
    private static Type player1Terrain = Type.NoTerrain;
    private static Type player2Terrain = Type.NoTerrain;

    // Day/Night — always alternates, no randomness needed
    public static void AdvanceDayNight()
    {
        activeDayNight = activeDayNight == Type.Day ? Type.Night : Type.Day;
    }

    public static Type GetDayNight() => activeDayNight;

    // Weather — Raining, Snowing, or None, but not the same as last turn
    private static Type lastWeather = Type.NoWeather;
    private static readonly Type[] weatherPool = { Type.NoWeather, Type.Raining, Type.Snowing };

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

    // Terrain — HighGround, Swamp, or None, but not the same as last turn per player
    private static readonly Type[] terrainPool = { Type.NoTerrain, Type.HighGround, Type.Swamp };
    private static Type lastPlayer1Terrain = Type.NoTerrain;
    private static Type lastPlayer2Terrain = Type.NoTerrain;

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