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

    public Dictionary<Units.Type, float> GetWeights()
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
        
        Units.Type predictedUnit = Units.Type.None;
        float  bestScore = -1;
        
        Dictionary<Units.Type, float> predictions  = GetWeights();
        if (predictions == null)
        {
            List<Units.Type> handUnits = controller.GetHeldUnits().ToList();
            int rand = Random.Shared.Next(0, handUnits.Count);
            Units.Type randUnit = handUnits[rand];
            return randUnit;
        }
        
        foreach (var (unit, score) in predictions)
        {
            if (score > bestScore)
            {
                predictedUnit = unit;
                bestScore = score;
            }
        }
        
        List<Units.Type> bestUnits = UnitRegistry.GetWeaknesses(predictedUnit);
        if (bestUnits == null || bestUnits.Count <= 0)
            return predictedUnit;
        
        bool unitFound = false;
        Units.Type bestUnit = Units.Type.None;
        foreach (Units.Type unit in bestUnits)
        {
            if (controller.GetHeldUnits().Contains(unit))
            {
                bestUnit = unit;
                unitFound = true;
                break;
            }
        }

        if (!unitFound)
        {
            List<Units.Type> handUnits = controller.GetHeldUnits().ToList();
            int rand = Random.Shared.Next(0, handUnits.Count);
            Units.Type randUnit = handUnits[rand];
            return randUnit;
        }
        
        return bestUnit;
    }

    public void PrintWeights()
    {
        Debug.Print("Weights:");
        foreach (var (unit, weight) in GetWeights())
        {
            Debug.Print(unit + ": " + weight);
        }
    }
    
    public void PrintMemory()
    {
        Debug.Print("Memory:");
        List<Units.Type> rev = memory;
        rev.Reverse();
        
        int count = 1;
        foreach (Units.Type unit in rev)
        {
            Debug.Print(count + ". " + unit);
            count++;
        }
    }
    
    public void PrintBestMove()
    {
        Debug.Print( controller.name + "'s Best move:");
        Debug.Print(GetBestMove().ToString());
    }
}