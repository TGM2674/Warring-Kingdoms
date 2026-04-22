using Godot;
using System;
using System.Collections.Generic;

public class Player : Node
{
	public float health = 10000;
    private int current = 0;
    private int count = 0;
    private bool forward = true;

    private List<Main.Units> list = new List<Main.Units>
    {
        Main.Units.Swordsmen,
        Main.Units.Cavalries,
        Main.Units.Spearsmen,
        Main.Units.Archers,
        Main.Units.Mages
    };

    public Main.Units GetNext()
    {
        return Cycle();
        //return Switcher();
        //return RandomChoice();
    }

    public Main.Units Cycle()
    {
        current++;
        if (current >= list.Count) current = 0;
        return list[current];
    }

    public Main.Units RandomChoice()
    {
        Random rand = new Random();
        int r = rand.Next(list.Count);
        return list[r];
    }

    public Main.Units Switcher()
    {
        count++;
        if (count % 10 == 0)
        {
            forward = !forward;
        }

        if (forward)
        {
            return Cycle();
        }
        else
        {
            Cycle();
            return Cycle();
        }
    }
}