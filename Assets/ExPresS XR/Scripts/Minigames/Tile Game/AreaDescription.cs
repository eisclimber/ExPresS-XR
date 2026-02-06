using UnityEngine;

[System.Serializable]
public class AreaDescription
{
    public int Id;
    public string Name;
    public Color Color;
    public Material Material;

    public AreaDescription(int id, string name, Color color, Material material)
    {
        Id = id;
        Name = name;
        Color = color;
        Material = material;
    }
}