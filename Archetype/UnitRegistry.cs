using Godot;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class UnitRegistry : Node
{
    [Export] private Godot.Collections.Array<UnitData> units = new();
    private static Dictionary<Units.Type, UnitData> unitsInternal = new();

    public override void _Ready()
    {
        unitsInternal.Clear();
        foreach (UnitData unit in units)
        {
            unitsInternal.Add(unit.unitName, unit);
        }
        units.Clear();
    }

    public static UnitData GetData(Units.Type type)
    {
        if (!unitsInternal.ContainsKey(type))
            return null;
        
        return unitsInternal[type];
    }
    
    public static List<Units.Type> GetWeaknesses(Units.Type type)
    {
        UnitData data = GetData(type);
        if (data == null)
            return null;
        
        return data.weaknesses.ToList();
    }
    
    public static bool IsStrongAgainst(Units.Type type, Units.Type other)
    {
        UnitData data = GetData(type);
        if (data == null)
            return false;
        return data.strengths.Contains(other);
    }
    
    public static bool IsWeakAgainst(Units.Type type, Units.Type other)
    {
        UnitData data = GetData(type);
        if (data == null)
            return false;
        return data.weaknesses.Contains(other);
    }
}
