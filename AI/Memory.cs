using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Memory
{
    public int SIZE = 20;
    public int LEN = 2;

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
        {
            memory.RemoveAt(0);
        }
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
        {
            predictions.Add(unit, count / (float)patternCount);
        }

        return predictions;
    }

    public Units.Type GetBestMove()
    {
        if (controller.GetHeldUnits().Count <= 0)
            return Units.Type.None;
        
        Dictionary<Units.Type, float> predictions = GetPredictedUnits();
        
        Modifiers.Type dayNight = Modifiers.GetDayNight();
        Modifiers.Type weather = Modifiers.GetWeather();
        Modifiers.Type terrain = Modifiers.GetTerrain(controller.playerIndex);
        
        if (predictions == null)
        {
            Dictionary<Units.Type, float> bestUnits = new(); 
            
            foreach (var unit in controller.GetHeldUnits())
            {
                UnitData data = UnitRegistry.GetData(unit);
                float baseDamage = data.damage;

                float multiplier = 1.0f;
            
                if (data.buffnerfs.ContainsKey(dayNight))
                    multiplier += data.buffnerfs[dayNight] == Modifiers.Effects.Buff ? 0.25f : -0.25f;
                if (data.buffnerfs.ContainsKey(weather))
                    multiplier += data.buffnerfs[weather] == Modifiers.Effects.Buff ? 0.25f : -0.25f;
                if (data.buffnerfs.ContainsKey(terrain))
                    multiplier += data.buffnerfs[terrain] == Modifiers.Effects.Buff ? 0.25f : -0.25f;
        
                float damage = baseDamage * multiplier;
                bestUnits.Add(unit, damage);
            }
            
            bestUnits = bestUnits
                .OrderByDescending(kvp => kvp.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            Debug.Print("AI predictions:");
            foreach (var (unit, damage) in bestUnits)
            {
                Debug.Print("\t" + unit.ToString() + ": " + damage);
            }
            
            return bestUnits.Keys.First();
        }
        
        Dictionary<Units.Type, Dictionary<Units.Type, float>> bestUnitsAgainstOpponent = new(); 
        
        foreach (var (predicted, score) in predictions)
        {
            bestUnitsAgainstOpponent.Add(predicted, new Dictionary<Units.Type, float>());
            
            foreach (var unit in controller.GetHeldUnits())
            {
                UnitData data = UnitRegistry.GetData(unit);
                float baseDamage = data.damage;

                float multiplier = 1.0f;
                
                if (data.buffnerfs.ContainsKey(dayNight))
                    multiplier += data.buffnerfs[dayNight] == Modifiers.Effects.Buff ? 0.25f : -0.25f;
                if (data.buffnerfs.ContainsKey(weather))
                    multiplier += data.buffnerfs[weather] == Modifiers.Effects.Buff ? 0.25f : -0.25f;
                if (data.buffnerfs.ContainsKey(terrain))
                    multiplier += data.buffnerfs[terrain] == Modifiers.Effects.Buff ? 0.25f : -0.25f;

                foreach (var strongAgainst in data.strengths)
                {
                    if (strongAgainst == predicted)
                    {
                        multiplier += 0.25f;
                    }
                }
                
                foreach (var weakAgainst in data.weaknesses)
                {
                    if (weakAgainst == predicted)
                    {
                        multiplier += -0.25f;
                    }
                }
            
                float damage = baseDamage * (multiplier + (score*0.5f));
                bestUnitsAgainstOpponent[predicted].Add(unit, damage);
            }
        }

        foreach (var predicted in bestUnitsAgainstOpponent.Keys.ToList())
        {
            bestUnitsAgainstOpponent[predicted] = bestUnitsAgainstOpponent[predicted]
                .OrderByDescending(kvp => kvp.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        
        Debug.Print("AI predictions:");
        foreach (var (predicted, dict) in bestUnitsAgainstOpponent)
        {
            Debug.Print("\tFor Opponent Unit: " + predicted);
            foreach (var (unit, damage) in dict)
            {
                Debug.Print("\t\t" + unit.ToString() + ": " + damage);
            }
        }
        
        Units.Type bestUnit = bestUnitsAgainstOpponent.Values.First().Keys.First();
        
        return bestUnit;
    }
    
    public Units.Type GetBestMoveRand()
    {
        if (controller.GetHeldUnits().Count <= 0)
            return Units.Type.None;
        
        Dictionary<Units.Type, float> predictions = GetPredictedUnits();
        
        Modifiers.Type dayNight = Modifiers.GetDayNight();
        Modifiers.Type weather = Modifiers.GetWeather();
        Modifiers.Type terrain = Modifiers.GetTerrain(controller.playerIndex);
        
        if (predictions == null)
        {
            Dictionary<Units.Type, float> bestUnits = new(); 
            
            foreach (var unit in controller.GetHeldUnits())
            {
                UnitData data = UnitRegistry.GetData(unit);
                float baseDamage = data.damage;

                float multiplier = 1.0f;
            
                if (data.buffnerfs.ContainsKey(dayNight))
                    multiplier += data.buffnerfs[dayNight] == Modifiers.Effects.Buff ? 0.25f : -0.25f;
                if (data.buffnerfs.ContainsKey(weather))
                    multiplier += data.buffnerfs[weather] == Modifiers.Effects.Buff ? 0.25f : -0.25f;
                if (data.buffnerfs.ContainsKey(terrain))
                    multiplier += data.buffnerfs[terrain] == Modifiers.Effects.Buff ? 0.25f : -0.25f;
        
                float damage = baseDamage * multiplier;
                bestUnits.Add(unit, damage);
            }
            
            bestUnits = bestUnits
                .OrderByDescending(kvp => kvp.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            Debug.Print("AI predictions:");
            foreach (var (unit, damage) in bestUnits)
            {
                Debug.Print("\t" + unit.ToString() + ": " + damage);
            }
            
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
            float baseDamage = data.damage;

            float multiplier = 1.0f;

            if (data.buffnerfs.ContainsKey(dayNight))
                multiplier += data.buffnerfs[dayNight] == Modifiers.Effects.Buff ? 0.25f : -0.25f;
            if (data.buffnerfs.ContainsKey(weather))
                multiplier += data.buffnerfs[weather] == Modifiers.Effects.Buff ? 0.25f : -0.25f;
            if (data.buffnerfs.ContainsKey(terrain))
                multiplier += data.buffnerfs[terrain] == Modifiers.Effects.Buff ? 0.25f : -0.25f;

            foreach (var strongAgainst in data.strengths)
                if (strongAgainst == rolledPrediction) multiplier += 0.25f;

            foreach (var weakAgainst in data.weaknesses)
                if (weakAgainst == rolledPrediction) multiplier -= 0.25f;

            damageAgainstRolled.Add(unit, baseDamage * multiplier);
        }

        List<Units.Type> topUnits = damageAgainstRolled
            .OrderByDescending(kvp => kvp.Value)
            .Take(3)
            .Select(kvp => kvp.Key)
            .ToList();

        Units.Type bestUnit = topUnits[Random.Shared.Next(0, topUnits.Count)];
        
        Debug.Print("AI predictions:");
        Debug.Print("\tFor Opponent Unit: " + rolledPrediction);
        foreach (var topUnit in topUnits)
            Debug.Print("\t\t" + topUnit.ToString());
        
        return bestUnit;
    }

    public void PrintWeights()
    {
        Debug.Print("Weights:");
        foreach (var (unit, weight) in GetPredictedUnits())
        {
            Debug.Print(unit + ": " + weight);
        }
    }
    
    public void PrintMemory()
    {
        Debug.Print("Memory:");
        List<Units.Type> rev = new List<Units.Type>(memory);
        rev.Reverse();
        
        int count = 1;
        foreach (Units.Type unit in rev)
        {
            Debug.Print(count + ". " + unit);
            count++;
        }
    }
    
    public void PrintMove(Units.Type unit)
    {
        Debug.Print( controller.name + "'s Best move:");
        Debug.Print(unit.ToString());
    }
}