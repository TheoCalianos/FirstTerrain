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

    public float voronoifallOff = 0.2f;
    public float voronoidropOff = 0.6f;
    public float voronoimaxHeight = 0.1f;
    public float voronoimaxWidth = .05f;
    public int voronoipeakCount = 5;
    public enum VoronoiType {Linear = 0, Power = 1, SinPow = 2, LogPow=3, Combined = 4}
    public VoronoiType voronoiType = VoronoiType.Combined;

    public float midPointHeightMax = -2f;
    public float midPointHeightMin = 2f;
    public float midPointDampener = 2.0f;
    public float midPointRoughness = 2.0f;

    public float splatOffset = 0.1f;
    public float splatNoiseXscale = 0.01f;
    public float splatNoiseYscale = 0.01f;
    public float splatNoisescaler = 0.2f;

    [System.Serializable]
    public class SplatHeights
    {
      public Texture2D texture = null;
      public float minHeight = 0.1f;
      public float maxHeight = 0.2f;
      public float minSlope = 0;
      public float maxSlope = 90f;
      public Vector2 tileOffset = new Vector2(0,0);
      public Vector2 tileSize = new Vector2(0,0);
      public float tsplatOffset = 0.01f;
      public float tsplatNoiseXscale =  0.01f;
      public float tsplatNoiseYscale =  0.01f;
      public float tsplatNoisescaler = 0.01f;
      public bool remove = false;
    }
    [System.Serializable]
    public class VegetationParams
    {
      public GameObject mesh = null;
      public float minHeight = .01f;
      public float maxHeight = .2f;
      public float minSlope = 0f;
      public  float maxSlope = 1f;
      public bool remove = false;
    }
    public List<VegetationParams> vegetation = new List<VegetationParams>()
    {
      new VegetationParams()
    };
    public int maxTrees = 5000;
    public int treeSpacing = 5;
    public List<SplatHeights> splatHeights = new List<SplatHeights>()
    {
      new SplatHeights()
    };

    public int smoothCount = 1;

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
    List<Vector2> Getneighbours(Vector2 pos, int width, int height)
    {
      List<Vector2> neighbours = new List<Vector2>();
      for(int y = -1; y < 2; y++)
      {
        for(int x = -1; x < 2; x++)
        {
          if(!(x == 0 && y == 0))
          {
            Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width-1),Mathf.Clamp(pos.y + y, 0,height-1));
            if(!neighbours.Contains(nPos))
            {
              neighbours.Add(nPos);
            }
          }
        }
      }
      return neighbours;
    }
    float GetSteepness(float[,] heightmap, int x, int y, int width, int height)
    {
      float h = heightmap[x,y];
      int nx = x +1;
      int ny = y +1;

      if(nx > width -1)nx = x-1;
      if(ny > height -1)nx = y-1;

      float dx = heightmap[nx, y] - h;
      float dy = heightmap[x,  ny] - h;

      Vector2 gradient = new Vector2(dx, dy);

      float steep = gradient.magnitude;

      return steep;
    }
    public void AddNewVegetation()
    {
      vegetation.Add(new VegetationParams());
    }
    public void RemoveVegetations()
    {
      List<VegetationParams> keptVegetationParams = new List<VegetationParams>();
      for (int i = 0; i < splatHeights.Count; i++) {
        if (!vegetation[i].remove)
        {
          keptVegetationParams.Add(vegetation[i]);
        }
      }
      if (keptVegetationParams.Count == 0)
      {
        keptVegetationParams.Add(vegetation[0]);
      }
      vegetation = keptVegetationParams;
    }
    public void Vegetation()
    {
      TreePrototype[] newTreePrototypes;
      newTreePrototypes = new TreePrototype[vegetation.Count];
      int tindex = 0;
      foreach(VegetationParams t in vegetation)
      {
        newTreePrototypes[tindex] = new TreePrototype();
        newTreePrototypes[tindex].prefab = t.mesh;
        tindex++;
      }
      terrainData.treePrototypes = newTreePrototypes;
      List<TreeInstance> allVegetation = new List<TreeInstance>();
      for(int z =0; z < terrainData.size.z; z += treeSpacing)
      {
        for(int x = 0; x < terrainData.size.x; x += treeSpacing)
        {
          for(int tp = 0; tp < terrainData.treePrototypes.Length; tp++)
          {
            float thisHeight = terrainData.GetHeight(x, z)/ terrainData.size.y;
            float thisHeightStart = vegetation[tp].minHeight;
            float thisHeightEnd = vegetation[tp].maxHeight;
            if(thisHeight >= thisHeightStart && thisHeight <= thisHeightEnd)
            {
              TreeInstance instance = new TreeInstance();
              instance.position = new Vector3((x+ UnityEngine.Random.Range(-5.0f,5.0f))/ terrainData.size.x, terrainData.GetHeight(x,z)/terrainData.size.y, (z+UnityEngine.Random.Range(-5.0f,5.0f)) / terrainData.size.z);
              instance.rotation = UnityEngine.Random.Range(0,360);
              instance.prototypeIndex = tp;
              instance.color = Color.white;
              instance.lightmapColor = Color.white;
              instance.heightScale = .95f;
              instance.widthScale = .95f;

              allVegetation.Add(instance);
              if (allVegetation.Count >= maxTrees) goto TREESDONE;
            }
          }
        }
      }
      TREESDONE:
        terrainData.treeInstances = allVegetation.ToArray();
    }
    public void AddNewSplatHeights()
    {
      SplatHeights newSplatHeight = new SplatHeights();
      newSplatHeight.tsplatOffset = splatOffset;
      newSplatHeight.tsplatNoiseXscale = splatNoiseXscale;
      newSplatHeight.tsplatNoiseYscale = splatNoiseYscale;
      newSplatHeight.tsplatNoisescaler = splatNoisescaler;
      splatHeights.Add(newSplatHeight);
    }
    public void RemoveSplatHeight()
    {
      List<SplatHeights> keptsplatHeights = new List<SplatHeights>();
      for (int i = 0; i < splatHeights.Count; i++) {
        if (!splatHeights[i].remove)
        {
          keptsplatHeights.Add(splatHeights[i]);
        }
      }
      if (keptsplatHeights.Count == 0)
      {
        keptsplatHeights.Add(splatHeights[0]);
      }
      splatHeights = keptsplatHeights;
    }
    public void SplatMaps()
    {
      TerrainLayer[] newSplatProtoType;
      newSplatProtoType = new TerrainLayer[splatHeights.Count];
      int spindex = 0;
      foreach (SplatHeights sh in splatHeights)
      {
        newSplatProtoType[spindex] = new TerrainLayer();
           newSplatProtoType[spindex].diffuseTexture = sh.texture;
           newSplatProtoType[spindex].tileOffset = sh.tileOffset;
           newSplatProtoType[spindex].tileSize = sh.tileSize;
           newSplatProtoType[spindex].diffuseTexture.Apply(true);
           string path = "Assets/New Terrain Layer " + spindex + ".terrainLayer";
           AssetDatabase.CreateAsset(newSplatProtoType[spindex], path);
           spindex++;
           Selection.activeObject = this.gameObject;
      }
      terrainData.terrainLayers = newSplatProtoType;
      float[,] heightMap = terrainData.GetHeights(0,0, terrainData.heightmapWidth,
                                                        terrainData.heightmapHeight);
      float[,,] splatmapData = new float[terrainData.alphamapWidth,terrainData.alphamapHeight,terrainData.alphamapLayers];
      for(int y = 0; y < terrainData.alphamapHeight; y++)
      {
        for(int x = 0; x < terrainData.alphamapWidth; x++)
        {
          float[] splat = new float[terrainData.alphamapLayers];
          for(int i = 0; i < splatHeights.Count; i++)
          {
            float noise = Mathf.PerlinNoise(x*splatHeights[i].tsplatNoiseXscale,y*splatHeights[i].tsplatNoiseYscale) * splatHeights[i].tsplatNoisescaler;
            float offset = splatHeights[i].tsplatOffset + noise;
            float thisHeightStart = splatHeights[i].minHeight - offset;
            float thisHeightStop = splatHeights[i].maxHeight + offset;
            //float steepness = GetSteepness(heightMap, x,y,terrainData.heightmapWidth,terrainData.heightmapHeight);
            float steepness = terrainData.GetSteepness(y/ (float)terrainData.alphamapHeight, x/(float)terrainData.alphamapWidth);
            if((heightMap[x,y] >= thisHeightStart && heightMap[x,y] <= thisHeightStop) && (steepness >= splatHeights[i].minSlope
                                && steepness <= splatHeights[i].maxSlope))
            {
              splat[i] = 1;
            }
          }
          NormalizeVector(splat);
          {
            for(int j = 0; j < splatHeights.Count; j++)
            {
              splatmapData[x, y, j]=splat[j];
            }
          }
        }
        terrainData.SetAlphamaps(0,0, splatmapData);
      }
      void NormalizeVector(float[] v)
      {
        float total =0;
        for(int i = 0; i < v.Length; i++)
        {
          total += v[i];

        }
        for(int i = 0; i < v.Length; i++)
        {
          v[i] /= total;
        }
      }

       heightMap = terrainData.GetHeights(0,0, terrainData.heightmapWidth, terrainData.heightmapHeight);

       splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
    }
    public void Smooth()
    {
      float[,] heightMap =  terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
      float smoothProgress = 0;
      EditorUtility.DisplayProgressBar("smoothing Terrain", "Progress", smoothProgress);
      for(int c =0; c < smoothCount; c++)
      {
        for (int y=0; y < terrainData.heightmapWidth; y++)
        {
          for(int x = 0; x < terrainData.heightmapHeight; x++)
          {
            float avgHeight = heightMap[x,y];
            List<Vector2> neighbours = Getneighbours(new Vector2(x,y), terrainData.heightmapWidth, terrainData.heightmapHeight);
            foreach(Vector2 n in neighbours)
            {
              avgHeight += heightMap[(int)n.x,(int)n.y];
            }
            heightMap[x, y] = avgHeight/ ((float)neighbours.Count + 1);
          }
        }
        smoothProgress++;
        EditorUtility.DisplayProgressBar("smoothing Terrain", "Progress", smoothProgress/smoothCount);
      }
      terrainData.SetHeights(0,0, heightMap);
      EditorUtility.ClearProgressBar();
    }
    public void MidPointDisplacement ()
    {
      float[,] heightMap = GetHeightMap();
      int width = terrainData.heightmapWidth - 1;//like power of 2.
      int squareSize = width;
      float heightMax = midPointHeightMax;
      float heightMin = midPointHeightMin;
      float heightDampener = (float)Mathf.Pow(midPointDampener, -1 * midPointRoughness);

      int cornerX;
      int cornerY;
      int midX;
      int midY;
      int pmidXL,pmidXR, pmidYU, pmidYD;

      /*heightMap[0,0] = UnityEngine.Random.Range(0f, 0.2f);
      heightMap[0, terrainData.heightmapHeight - 2] = UnityEngine.Random.Range(0f, 0.2f);
      heightMap[terrainData.heightmapWidth-2, 0] = UnityEngine.Random.Range(0f, 0.2f);
      heightMap[terrainData.heightmapWidth-2,  terrainData.heightmapHeight-2] = UnityEngine.Random.Range(0f, 0.2f);*/
      while(squareSize > 0)
      {
        for(int x = 0; x < width; x += squareSize)
        {
          for(int y = 0; y < width; y += squareSize)
          {
            cornerX = (x + squareSize);
            cornerY = (y + squareSize);
            midX = (int)(x  + squareSize / 2.0f);
            midY = (int)(y + squareSize / 2.0f);
            heightMap[midX,midY] = (float)((heightMap[x,y] +
                                            heightMap[cornerX , y] +
                                            heightMap[x, cornerY] +
                                            heightMap[cornerX,cornerY]) / 4.0f +
                                            UnityEngine.Random.Range(heightMin, heightMax));
            }
          }
          for(int x = 0; x < width; x += squareSize)
          {
            for(int y = 0; y < width; y += squareSize)
            {
              cornerX = (x + squareSize);
              cornerY = (y + squareSize);

              midX = (int)(x + squareSize / 2.0f);
              midY = (int)(y + squareSize / 2.0f);

              pmidXR = (int)(midX + squareSize);
              pmidYU = (int)(midY + squareSize);
              pmidXL = (int)(midX - squareSize);
              pmidYD = (int)(midY - squareSize);

              if(pmidXR > squareSize)
              {
                pmidXR = (pmidXR/2)-(squareSize*2/3);
              }
              if(pmidYU > squareSize)
              {
                pmidYU = (pmidYU/2)-(squareSize*2/3);
              }
              if(pmidXL <= 0)
              {
                pmidXL = (pmidXL/2)+(squareSize*2/3);
              }
              if(pmidYD <= 0)
              {
                pmidYD = (pmidYD/2)+(squareSize*2/3);
              }
              //if(pmidXL <= 0 || pmidYD <= 0 || pmidXR >= width - 1 || pmidYU >= width - 1 ) continue;
              //Debug.Log("pmidXR " + pmidXR + " pmidYU " + pmidYU  + " pmidXL " + pmidXL + " pmidYD " + pmidYD );
              //calc the square value for the bottom side
              heightMap[midX, y] =  (float)((heightMap[midX,midY] +
                                              heightMap[x , y] +
                                              heightMap[midX, pmidYD] +
                                              heightMap[cornerX, y]) / 4.0f +
                                              UnityEngine.Random.Range(heightMin, heightMax));
               //calc the square value for the top side
               heightMap[midX, cornerY] =  (float)((heightMap[midX,midY] +
                                             heightMap[x , cornerY] +
                                             heightMap[midX, pmidYU] +
                                             heightMap[cornerX, cornerY]) / 4.0f +
                                             UnityEngine.Random.Range(heightMin, heightMax));
              //calc the square value for the left side
              heightMap[x, midY] =  (float)((heightMap[midX,midY] +
                                            heightMap[x , y] +
                                            heightMap[pmidXL, midY] +
                                            heightMap[x, cornerY]) / 4.0f +
                                            UnityEngine.Random.Range(heightMin, heightMax));
              //calc the square value for the right side
              heightMap[cornerX, midY] =  (float)((heightMap[midX,midY] +
                                            heightMap[cornerX , y] +
                                            heightMap[pmidXR, midY] +
                                            heightMap[cornerX, cornerY]) / 4.0f +
                                            UnityEngine.Random.Range(heightMin, heightMax));
            }
          }
          squareSize = (int) (squareSize/2.0f);
          heightMin *= heightDampener;
          heightMax *= heightDampener;
      }
      terrainData.SetHeights(0, 0, heightMap);
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
                h = peak.y - Mathf.Pow(distanceToPeak*3, voronoifallOff) -
                            Mathf.Sin(distanceToPeak*2*Mathf.PI)/voronoidropOff;
              }
              else if(voronoiType == VoronoiType.LogPow)
              {
                h = peak.y - Mathf.Pow(distanceToPeak*3, voronoifallOff) -  distanceToPeak* Mathf.Log(Mathf.Exp(1), voronoidropOff);
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
