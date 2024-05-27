using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Props : MonoBehaviour
{
    public GameObject[] propPrefabs;
    public GameObject[] grassPrefabs;
    public GameObject[] oneTilePlantsPrefabs;
    public GameObject[] twoTilePlantsPrefabs;
    public GameObject[] treePrefabs;
    
    
    public GameObject[] oneTilePropPrefabs;
    public GameObject[] oneTilePropWithGrassPrefabs;
    public GameObject[] twoTilePropPrefabs;
    public GameObject[] twoTilePropWithGrassPrefabs;

    public GameObject[] sixTilePropPrefabs;

    public GameObject[] chairPropPrefabs;
    
    public GameObject[] layerProps;
    public GameObject[] grassLayers;


    /// Instantiate an object at a specified location
    public void GenerayePropAt(int objNum, float x, float y)
    {
        GameObject instance = Instantiate(propPrefabs[0], layerProps[0].transform, true);
        instance.transform.position = new Vector3(x, y, 0);
        instance.layer = layerProps[0].layer;
    }

    /// Grass is generated at the specified location
    public void GenerateGrassAt(float x, float y, int layer, Random random)
    {
        if (grassPrefabs.Length == 0 || layer > grassLayers.Length)
        {
            return;
        }
        GameObject instance = Instantiate(grassPrefabs[random.Next(0, grassPrefabs.Length)], grassLayers[layer - 1].transform, true);
        instance.transform.position = new Vector3((float)(x + random.NextDouble()), (float)(y + random.NextDouble()), 0);
        instance.layer = grassLayers[layer - 1].layer;
        instance.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(grassLayers[layer - 1].layer);
    }

    /// A single lattice plant is generated at a specified location
    public void GenerateOneTilePlantAt(float x, float y, int layer, Random random)
    {
        if (oneTilePlantsPrefabs.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(oneTilePlantsPrefabs[random.Next(0, oneTilePlantsPrefabs.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3((float)(x + random.NextDouble()), (float)(y + random.NextDouble()), 0);
        instance.layer = layerProps[layer - 1].layer;
        instance.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(layerProps[layer - 1].layer);
    }

    /// Generates 1x1 items at the specified location
    public void Generate1X1TilePropAt(float x, float y, int layer, Random random, bool withGrass = false)
    {
        GameObject[] objects = withGrass ? oneTilePropWithGrassPrefabs : oneTilePropPrefabs;
        if (objects.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(objects[random.Next(0, objects.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3((float)(x + random.NextDouble()), (float)(y + random.NextDouble()), 0);
        instance.layer = layerProps[layer - 1].layer;
        instance.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(layerProps[layer - 1].layer);

        if (withGrass && grassPrefabs.Length != 0)
        {
            GameObject grass = Instantiate(grassPrefabs[random.Next(0, grassPrefabs.Length)], grassLayers[layer - 1].transform, true);
            float xPos = (float)(1 - random.NextDouble() / 10) * ((random.Next() & 2) - 1);
            float yPos = (float)(1 - random.NextDouble() / 10) * ((random.Next() & 2) - 1);
            grass.transform.position = new Vector3(instance.transform.position.x + xPos, instance.transform.position.y + yPos, 0);
            grass.layer = grassLayers[layer - 1].layer;
            grass.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(grassLayers[layer - 1].layer);
        }
    }

    /// Generates 2x2 grid items at the specified location
    public void Generate2X2TilePropAt(float x, float y, int layer, Random random, bool withGrass = false)
    {
        GameObject[] objects = withGrass ? twoTilePropWithGrassPrefabs : twoTilePropPrefabs;
        if (objects.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(objects[random.Next(0, objects.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3((float)(x + random.NextDouble()), (float)(y + random.NextDouble()), 0);
        instance.layer = layerProps[layer - 1].layer;
        instance.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(layerProps[layer - 1].layer);

        if (withGrass && grassPrefabs.Length != 0)
        {
            GameObject grass = Instantiate(grassPrefabs[random.Next(0, grassPrefabs.Length)], grassLayers[layer - 1].transform, true);
            float xPos = (float)(1 - random.NextDouble() / 10) * ((random.Next() & 2) - 1);
            float yPos = (float)(1 - random.NextDouble() / 10) * ((random.Next() & 2) - 1);
            grass.transform.position = new Vector3(instance.transform.position.x + xPos, instance.transform.position.y + yPos, 0);
            grass.layer = grassLayers[layer - 1].layer;
            grass.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(grassLayers[layer - 1].layer);
        }
    }

    /// Generates a 6x6 grid item at the specified location, fixed position
    public void Generate6X6TilePropAt(float x, float y, int layer, Random random)
    {
        GameObject[] objects = sixTilePropPrefabs;
        if (objects.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(objects[random.Next(0, objects.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3(x, y, 0);
        instance.layer = layerProps[layer - 1].layer;
        var sprites = instance.GetComponentsInChildren<SpriteRenderer>();
        foreach (var render in sprites)
        {
            render.sortingLayerName = LayerMask.LayerToName(layerProps[layer - 1].layer);
        }
    }

    /// Creates a chair at the specified position
    public void GenerateChairAt(float x, float y, int layer, Random random)
    {
        GameObject[] objects = chairPropPrefabs;
        if (objects.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(objects[random.Next(0, objects.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3(x, y, 0);
        instance.layer = layerProps[layer - 1].layer;
        instance.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(layerProps[layer - 1].layer);
    }

    /// Two grid plants are generated at the specified location
    public void GenerateTwoTilePlantAt(float x, float y, int layer, Random random)
    {
        if (twoTilePlantsPrefabs.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(twoTilePlantsPrefabs[random.Next(0, twoTilePlantsPrefabs.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3((float)(x + random.NextDouble()), (float)(y + random.NextDouble()), 0);
        instance.layer = layerProps[layer - 1].layer;
        instance.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(layerProps[layer - 1].layer);
    }

    /// Generates the tree at the specified location
    public void GenerateTreeAt(float x, float y, int layer, Random random)
    {
        if (treePrefabs.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(treePrefabs[random.Next(0, treePrefabs.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3((float)(x + random.NextDouble()), (float)(y + random.NextDouble()), 0);
        instance.layer = layerProps[layer - 1].layer;
        instance.transform.Find("Upper").GetComponent<SpriteRenderer>().sortingLayerName = "Layer " + (layer + 1);
        instance.transform.Find("Lower").GetComponent<SpriteRenderer>().sortingLayerName =  LayerMask.LayerToName(layerProps[layer - 1].layer);
        instance.transform.Find("Shadow").GetComponent<SpriteRenderer>().sortingLayerName =  LayerMask.LayerToName(layerProps[layer - 1].layer);
    }
}
