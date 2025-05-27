using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public string name;
    public int? waves; 
    public List<Spawn> spawns;
}
