using System;

public class NGramsAI
{
    public const int RUNS = 100;

    private int wins = 0;
    private int losses = 0;
    private int draws = 0;

    private Main.Units[] list =
    {
        Main.Units.Swordsmen,
        Main.Units.Cavalries,
        Main.Units.Spearsmen,
        Main.Units.Archers,
        Main.Units.Mages
    };

    public void Run()
    {
        Memory memory = new Memory();
        Player player = new Player();

        for (int i = 0; i < RUNS; i++)
        {
            Console.WriteLine($"Game {i}");
            Play(memory, player);
        }

        Console.WriteLine($"Wins: {wins}");
        Console.WriteLine($"Losses: {losses}");
        Console.WriteLine($"Draws: {draws}");
    }

    private void Play(Memory memory, Player player)
    {
        Main.Units ai = GetPlay(memory);
        Main.Units p = player.GetNext();

        memory.AddMove(p);

        Console.WriteLine($"Player: {p} \tAI: {ai}");

        int result = GetWinner(ai, p);

        if (result == 1)
        {
            Console.WriteLine("AI wins!");
            wins++;
        }
        else if (result == -1)
        {
            Console.WriteLine("Player wins!");
            losses++;
        }
        else
        {
            Console.WriteLine("Draw!");
            draws++;
        }
    }

    private Main.Units GetPlay(Memory memory)
    {
        Random rand = new Random();

        int best = -1;
        Main.Units predictedPlayerMove = list[0];

        foreach (var move in list)
        {
            int count = memory.CountPattern(move);
            Console.WriteLine($"Matching for {move}: {count}");

            if (count > best)
            {
                best = count;
                predictedPlayerMove = move;
            }
            else if (count == best)
            {
                if (rand.Next(2) == 0)
                {
                    predictedPlayerMove = move;
                }
            }
        }

        Console.WriteLine("Memory: " + memory.GetMemory());

        return GetBestCounter(predictedPlayerMove);
    }

    private Main.Units GetBestCounter(Main.Units playerMove)
    {
        Main.Units best = Main.Units.Swordsmen;
        float bestModifier = float.MinValue;

        foreach (var candidate in list)
        {
            float mod = ArchetypeSystem.GetModifier(candidate, playerMove);

            if (mod > bestModifier)
            {
                bestModifier = mod;
                best = candidate;
            }
        }

        return best;
    }

    private int GetWinner(Main.Units ai, Main.Units player)
    {
        float aiMod = ArchetypeSystem.GetModifier(ai, player);
        float playerMod = ArchetypeSystem.GetModifier(player, ai);

        if (aiMod > playerMod) return 1;
        if (aiMod < playerMod) return -1;
        return 0;
    }
}