using SmartifyOS.Editor.Theming;
using UnityEngine;

public class TerrainThemer : BaseThemeable<Color>
{
    public Terrain terrain;

    public Color GetTint()
    {
        return terrain.terrainData.terrainLayers[0].diffuseRemapMax;
    }

    public override void UpdateValue(Color value)
    {
        TerrainLayer layer = terrain.terrainData.terrainLayers[0];

        // Adjust the tint by modifying the diffuseRemap values
        layer.diffuseRemapMin = Color.black; // Minimum color
        layer.diffuseRemapMax = value;   // Maximum color tint

        // Apply the modified layer
        terrain.terrainData.terrainLayers[0] = layer;
    }
}
