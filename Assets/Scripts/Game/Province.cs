using UnityEngine;
using System.Collections.Generic;

public class Province
{
    public int Number { get; private set; }
    public WaylaidPlayer Owner { get; set; }
    public List<Province> Connections { get; private set; }

    public Province(int num)
    {
        Number = num;
        Connections = new List<Province>();
    }
}