﻿using System.Collections;
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

    public float voronoifallOff = 0.2f;
    public float voronoidropOff = 0.6f;
    public float voronoimaxHeight = 0.1f;
    public float voronoimaxWidth = .05f;
    public int voronoipeakCount = 5;
    public enum VoronoiType {Linear = 0, Power = 1, Combined = 2, SinPow = 3}
    public VoronoiType voronoiType = VoronoiType.Combined;

    //
    [System.Serializable]
    public class PerlinParameters
    {
      public float mperlinXScale = 0.01f;
      public float mperlinYScale = 0.01f;
      public int mperlinOctaves = 5;
      public float mperlinHeightScale = 0.09f;
      public float mperlinPersistance = 8;
      public int mperlinOffsetX = 0;
      public int mperlinOffsetY = 0;
      public bool remove = false;
    }
    public List<PerlinParameters> perlinParameters = new List<PerlinParameters>()
    {
      new PerlinParameters()
    };
    public bool resetTerrain = true;

    public Terrain terrain;
    public TerrainData terrainData;

    float [,] GetHeightMap()
    {
      if(!resetTerrain)
      {
        return terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
      }
      else
      {
        return new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
      }
    }
    public void Voronoi()
    {
      float[,] heightMap = GetHeightMap();
      for(int p = 0; p < voronoipeakCount; p++)
      {
        Vector3 peak = new Vector3(UnityEngine.Random.Range(0, terrainData.heightmapWidth),
                                  UnityEngine.Random.Range(voronoimaxHeight, voronoimaxWidth),
                                  UnityEngine.Random.Range(0, terrainData.heightmapHeight));

        if(heightMap[(int)peak.x, (int)peak.z] < peak.y)
        {
          heightMap[(int)peak.x, (int)peak.z] = peak.y;
        }
        else
          continue;

        Vector2 peakLocation = new Vector2(peak.x,peak.z);
        float maxDistance = Vector2.Distance(new Vector2(0,0), new Vector2(terrainData.heightmapWidth, terrainData.heightmapHeight));
        for(int y = 0; y < terrainData.heightmapHeight; y++)
        {
          for(int x = 0; x < terrainData.heightmapWidth; x++)
          {
            if(!(x == peak.x && y == peak.z))
            {
              float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y))/maxDistance;
              float h;
              if(voronoiType == VoronoiType.Combined)
              {
                 h = peak.y - distanceToPeak* voronoifallOff - Mathf.Pow(distanceToPeak, voronoidropOff);
              }
              else if(voronoiType == VoronoiType.Power)
              {
                h = peak.y - Mathf.Pow(distanceToPeak, voronoidropOff) * voronoidropOff;
              }
              else if(voronoiType == VoronoiType.SinPow)
              {
                h = peak.y - Mathf.Pow(distanceToPeak*3, voronoifallOff) - Mathf.Sin(distanceToPeak*2*Mathf.PI)/voronoidropOff;
              }
              else
              {
                h = peak.y - distanceToPeak * voronoidropOff;
              }
              if(heightMap[x, y] < h)
              {
                  heightMap[x, y] = h;
              }
            }
          }
        }
      }

      terrainData.SetHeights(0,0, heightMap);
    }

    public void Perlin()
    {
      float[,] heightMap = GetHeightMap();

      for (int x=0; x < terrainData.heightmapWidth; x++)
      {
        for(int y = 0; y < terrainData.heightmapHeight; y++)
        {
          heightMap[x, y] += Utils.fBM((x + perlinXScale) * perlinXScale,
                                      (y+perlinYScale) * perlinYScale,
                                      perlinOctaves,
                                      perlinPersistance) * perlinHeightScale;

        }
      }
      terrainData.SetHeights(0,0, heightMap);
    }
    public void MultiplePerlinTerrain()
    {
      float[,] heightMap = GetHeightMap();
      for(int y = 0; y < terrainData.heightmapHeight; y++)
      {
        for (int x=0; x < terrainData.heightmapWidth; x++)
        {
          foreach(PerlinParameters p in perlinParameters)
          {
            heightMap[x, y] += Utils.fBM((x + p.mperlinXScale) * p.mperlinXScale,
                                      (y+ p.mperlinYScale) * p.mperlinYScale,
                                      p.mperlinOctaves,
                                      p.mperlinPersistance) * p.mperlinHeightScale;
          }
        }
      }
      terrainData.SetHeights(0,0, heightMap);
    }
    public void AddNewPerlin()
    {
      perlinParameters.Add(new PerlinParameters());
    }
    public void RemovePerlin()
    {
      List<PerlinParameters> keptPerlinParameters = new List<PerlinParameters>();
      for (int i = 0; i < perlinParameters.Count; i++) {
        if (!perlinParameters[i].remove)
        {
          keptPerlinParameters.Add(perlinParameters[i]);
        }
      }
      if (keptPerlinParameters.Count == 0)
      {
        keptPerlinParameters.Add(perlinParameters[0]);
      }
      perlinParameters = keptPerlinParameters;
    }

    public void RandomTerrain()
    {
      float[,] heightMap = GetHeightMap();

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
      float[,] heightMap =  GetHeightMap();

      for (int x=0; x < terrainData.heightmapWidth; x++)
      {
        for(int z = 0; z < terrainData.heightmapHeight; z++)
        {
          heightMap[x, z] += heightMapImage.GetPixel((int)(x * heightMapScale.x),(int)(z * heightMapScale.z)).grayscale * heightMapScale.y;
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
