using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Memory
{
    public int SIZE = 50;
    public int LEN = 2;

    private const float MATCHUP_WEIGHT = 1.0f;
    private const float ENVIRONMENT_WEIGHT = 0.3f;

    private UnitController controller = null;
    private List<Units.Type> memory = new();

    public Memory(UnitController _controller)
    {
        controller = _controller;
    }
    
    public void AddMove(Units.Type move)
    {
        memory.Add(move);
        if (memory.Count > SIZE)
            memory.RemoveAt(0);
    }

    public Dictionary<Units.Type, float> GetPredictedUnits()
    {
        if (memory.Count <= LEN)
            return null;
        
        List<Units.Type> previous = memory.Skip(memory.Count - LEN).Take(LEN).ToList();

        Dictionary<Units.Type, int> predictionCount = new();
        int patternCount = 0;

        for (int i = 0; i <= memory.Count - (LEN + 1); i++)
        {
            bool match = true;
            for (int j = 0; j < LEN; j++)
            {
                if (memory[i + j] != previous[j])
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                if (!predictionCount.ContainsKey(memory[i + LEN]))
                    predictionCount.Add(memory[i + LEN], 0);
                predictionCount[memory[i + LEN]]++;
                patternCount++;
            }
        }

        if (patternCount == 0)
            return null;
        
        Dictionary<Units.Type, float> predictions = new();
        foreach (var (unit, count) in predictionCount)
            predictions.Add(unit, count / (float)patternCount);

        return predictions;
    }

    private float GetEnvironmentScore(UnitData data, Modifiers.Type weather, Modifiers.Type terrain)
    {
        float score = 0f;
        if (weather != Modifiers.Type.NoWeather && data.buffs.Contains(weather))
            score += 0.25f;
        if (terrain != Modifiers.Type.NoTerrain && data.buffs.Contains(terrain))
            score += 0.25f;
        return score;
    }

    private float GetMatchupScore(UnitData data, Units.Type predicted)
    {
        float score = 0f;
        foreach (var strongAgainst in data.strengths)
            if (strongAgainst == predicted) score += 0.25f;
        foreach (var weakAgainst in data.weaknesses)
            if (weakAgainst == predicted) score -= 0.25f;
        return score;
    }

    public Units.Type GetBestMove()
    {
        if (controller.GetHeldUnits().Count <= 0)
            return Units.Type.None;

        Modifiers.Type weather = Modifiers.GetWeather();
        Modifiers.Type terrain = Modifiers.GetTerrain(controller.playerIndex);

        Dictionary<Units.Type, float> predictions = GetPredictedUnits();

        if (predictions == null)
        {
            Dictionary<Units.Type, float> bestUnits = new();

            foreach (var unit in controller.GetHeldUnits())
            {
                UnitData data = UnitRegistry.GetData(unit);
                float envScore = GetEnvironmentScore(data, weather, terrain) * ENVIRONMENT_WEIGHT;
                bestUnits.Add(unit, data.damage * (1.0f + envScore));
            }

            bestUnits = bestUnits
                .OrderByDescending(kvp => kvp.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Debug.Print("AI predictions (no history):");
            foreach (var (unit, damage) in bestUnits)
                Debug.Print("\t" + unit + ": " + damage);

            return bestUnits.Keys.First();
        }

        Dictionary<Units.Type, float> unitScores = new();

        foreach (var unit in controller.GetHeldUnits())
        {
            UnitData data = UnitRegistry.GetData(unit);
            float totalScore = 0f;

            foreach (var (predicted, probability) in predictions)
            {
                float matchupScore = GetMatchupScore(data, predicted) * MATCHUP_WEIGHT;
                float envScore = GetEnvironmentScore(data, weather, terrain) * ENVIRONMENT_WEIGHT;
                float roundScore = data.damage * (1.0f + matchupScore + envScore);
                totalScore += roundScore * probability;
            }

            unitScores.Add(unit, totalScore);
        }

        unitScores = unitScores
            .OrderByDescending(kvp => kvp.Value)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Debug.Print("AI predictions:");
        foreach (var (predicted, prob) in predictions)
            Debug.Print("\tOpponent likely to play: " + predicted + " (" + (prob * 100f).ToString("F1") + "%)");
        Debug.Print("\tAI chose: " + unitScores.Keys.First());

        return unitScores.Keys.First();
    }
    
    public Units.Type GetBestMoveRand()
    {
        if (controller.GetHeldUnits().Count <= 0)
            return Units.Type.None;

        Modifiers.Type weather = Modifiers.GetWeather();
        Modifiers.Type terrain = Modifiers.GetTerrain(controller.playerIndex);

        Dictionary<Units.Type, float> predictions = GetPredictedUnits();

        if (predictions == null)
        {
            Dictionary<Units.Type, float> bestUnits = new();

            foreach (var unit in controller.GetHeldUnits())
            {
                UnitData data = UnitRegistry.GetData(unit);
                float envScore = GetEnvironmentScore(data, weather, terrain) * ENVIRONMENT_WEIGHT;
                bestUnits.Add(unit, data.damage * (1.0f + envScore));
            }

            bestUnits = bestUnits
                .OrderByDescending(kvp => kvp.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Debug.Print("AI predictions (no history):");
            foreach (var (unit, damage) in bestUnits)
                Debug.Print("\t" + unit + ": " + damage);

            return bestUnits.Keys.First();
        }

        float randPredicted = (float)Random.Shared.NextDouble();
        float cumulative = 0f;
        Units.Type rolledPrediction = predictions.Keys.First();

        foreach (var (predicted, score) in predictions)
        {
            cumulative += score;
            if (randPredicted <= cumulative)
            {
                rolledPrediction = predicted;
                break;
            }
        }

        Dictionary<Units.Type, float> damageAgainstRolled = new();

        foreach (var unit in controller.GetHeldUnits())
        {
            UnitData data = UnitRegistry.GetData(unit);
            float matchupScore = GetMatchupScore(data, rolledPrediction) * MATCHUP_WEIGHT;
            float envScore = GetEnvironmentScore(data, weather, terrain) * ENVIRONMENT_WEIGHT;
            damageAgainstRolled.Add(unit, data.damage * (1.0f + matchupScore + envScore));
        }

        List<Units.Type> topUnits = damageAgainstRolled
            .OrderByDescending(kvp => kvp.Value)
            .Take(3)
            .Select(kvp => kvp.Key)
            .ToList();

        Units.Type bestUnit = topUnits[Random.Shared.Next(0, topUnits.Count)];

        Debug.Print("AI predictions:");
        Debug.Print("\tOpponent likely to play: " + rolledPrediction);
        Debug.Print("\tTop candidates: " + string.Join(", ", topUnits));
        Debug.Print("\tAI chose: " + bestUnit);

        return bestUnit;
    }

    public void PrintMove(Units.Type unit)
    {
        Debug.Print(controller.name + "'s Best move:");
        Debug.Print(unit.ToString());
    }

    public void PrintMemory()
    {
        Debug.Print("Memory:");
        List<Units.Type> rev = new List<Units.Type>(memory);
        rev.Reverse();
        int count = 1;
        foreach (Units.Type u in rev)
        {
            Debug.Print(count + ". " + u);
            count++;
        }
    }
}