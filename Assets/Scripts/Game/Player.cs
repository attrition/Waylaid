using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColours
{
    public static readonly Dictionary<int, Color> Table = new Dictionary<int, Color>() {
        { -1, Color.gray },
        {  0, Color.red },
        {  1, Color.blue },
        {  2, Color.green },
        {  3, Color.yellow },
    };
}
 
public class WaylaidPlayer
{
    protected   string  name;
    public      int     Number { get; protected set; }
    protected   string  typeDesc;
    public      Color   Colour { get; protected set; }

    public HashSet<Province> Provinces { get; set; }
}

public class HumanPlayer : WaylaidPlayer
{
    public HumanPlayer(int number, string name)
    {
        this.name = name;
        this.Number = number;
        this.typeDesc = "Human";
        this.Colour = PlayerColours.Table[number];
        Provinces = new HashSet<Province>();
    }
}

public class AIPlayer : WaylaidPlayer
{
    public AIPlayer(int number, string name)
    {
        this.name = name;
        this.Number = number;
        this.typeDesc = "AI";
        this.Colour = PlayerColours.Table[number];
        Provinces = new HashSet<Province>();
    }
}

public class AITribe : WaylaidPlayer
{
    public AITribe(int number, string name)
    {
        this.name = name;
        this.Number = number;
        this.typeDesc = "Tribe";
        this.Colour = PlayerColours.Table[-1];  // all tribes share the same neutral toned colour
        Provinces = new HashSet<Province>();       // tribes can only hold one province at a time though
    }
}
