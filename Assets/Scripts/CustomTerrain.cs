using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CustomTerrain : MonoBehaviour
{
    public Vector2 randomHeightRange = new Vector2(0,0.1f);
    public Texture2D heightMapImage;
    public Vector3 heightMapScale = new Vector3(1, 1, 1);

    public float perlinXScale = 0.01f;
    public float perlinYScale = 0.01f;
    public int perlinOffsetX = 0;
    public int perlinOffsetY = 0;
    public int perlinOctaves = 3;
    public float perlinHeightScale = 0.09f;
    public float perlinPersistance = 8;

    public Terrain terrain;
    public TerrainData terrainData;

    public void Perlin()
    {
      float[,] heightMap = terrainData.GetHeights(0,0,terrainData.heightmapWidth, terrainData.heightmapHeight);

      for (int x=0; x < terrainData.heightmapWidth; x++)
      {
        for(int y = 0; y < terrainData.heightmapHeight; y++)
        {
          heightMap[x, y] = Utils.fBM((x + perlinXScale) * perlinXScale,
                                      (y+perlinYScale) * perlinYScale,
                                      perlinOctaves,
                                      perlinPersistance) * perlinHeightScale;

        }
      }
      terrainData.SetHeights(0,0, heightMap);
    }

    public void RandomTerrain()
    {
      float[,] heightMap = terrainData.GetHeights(0,0,terrainData.heightmapWidth, terrainData.heightmapHeight);

      for (int x=0; x < terrainData.heightmapWidth; x++)
      {
        for(int y = 0; y < terrainData.heightmapHeight; y++)
        {
          heightMap[x, y] += UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
        }
      }
      terrainData.SetHeights(0,0, heightMap);
    }
    public void LoadTexture()
    {
      float[,] heightMap;
      heightMap = new float [terrainData.heightmapWidth, terrainData.heightmapHeight];

      for (int x=0; x < terrainData.heightmapWidth; x++)
      {
        for(int z = 0; z < terrainData.heightmapHeight; z++)
        {
          heightMap[x, z] = heightMapImage.GetPixel((int)(x * heightMapScale.x),(int)(z * heightMapScale.z)).grayscale * heightMapScale.y;
        }
      }
      terrainData.SetHeights(0,0, heightMap);
    }
    void OnEnable()
    {
      Debug.Log("initialising Terrain Data");
      terrain = this.GetComponent<Terrain>();
      terrainData = Terrain.activeTerrain.terrainData;
    }
    void Awake()
    {
      SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
      SerializedProperty tagsProp = tagManager.FindProperty("tags");

      AddTag(tagsProp, "Terrain");
      AddTag(tagsProp, "Cloud");
      AddTag(tagsProp, "Shore");

      tagManager.ApplyModifiedProperties();

      this.gameObject.tag = "Terrain";
    }
    public void ResetTerrain()
    {
      float[,] heightMap;
      heightMap = new float [terrainData.heightmapWidth, terrainData.heightmapHeight];

      for (int x=0; x < terrainData.heightmapWidth; x++)
      {
        for(int y = 0; y < terrainData.heightmapHeight; y++)
        {
          heightMap[x, y] = 0;
        }
      }
      terrainData.SetHeights(0,0, heightMap);
    }
    void AddTag(SerializedProperty tagsProp,string newTag)
    {
      bool found = false;

      for (int i = 0; i < tagsProp.arraySize; i++)
      {
        SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
        if(t.stringValue.Equals(newTag))
        {
            found = true;
            break;
        }
      }
      if(!found)
      {
        tagsProp.InsertArrayElementAtIndex(0);
        SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
        newTagProp.stringValue = newTag;
      }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
