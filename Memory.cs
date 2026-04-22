using System;
using System.Collections.Generic;
using System.Linq;

public class Memory
{
    public int SIZE = 10;
    public int LEN = 2;

    private List<Main.Units> memory = new List<Main.Units>();

    public void AddMove(Main.Units move)
    {
        memory.Add(move);

        if (memory.Count > SIZE)
        {
            memory.RemoveAt(0);
        }
    }

    public int CountPattern(Main.Units next)
    {
        if (memory.Count < LEN) return 0;

        var previous = memory.Skip(memory.Count - LEN).Take(LEN).ToList();

        int count = 0;

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

            if (match && memory[i + LEN] == next)
            {
                count++;
            }
        }

        return count;
    }

    public string GetMemory()
    {
        return string.Join(", ", memory);
    }
}