using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorGUITable;

[CustomEditor(typeof(CustomTerrain))]
[CanEditMultipleObjects]

public class CustomTerrainEditor : Editor
{
    //properties
    SerializedProperty heightMapScale;
    SerializedProperty heightMapImage;
    SerializedProperty randomHeightRange;
    SerializedProperty perlinXScale;
    SerializedProperty perlinYScale;
    SerializedProperty perlinOffsetY;
    SerializedProperty perlinOffsetX;
    SerializedProperty perlinOctaves;
    SerializedProperty perlinPersistance;
    SerializedProperty perlinHeightScale;
    SerializedProperty resetTerrain;
    SerializedProperty voronoifallOff;
    SerializedProperty voronoidropOff;
    SerializedProperty voronoimaxHeight;
    SerializedProperty voronoimaxWidth;
    SerializedProperty voronoipeakCount;
    SerializedProperty voronoiType;
    SerializedProperty midPointHeightMax;
    SerializedProperty midPointHeightMin;
    SerializedProperty midPointRoughness;
    SerializedProperty midPointDampener;
    SerializedProperty smoothCount;
    SerializedProperty splatOffset;
    SerializedProperty splatNoiseXscale;
    SerializedProperty splatNoiseYscale;
    SerializedProperty splatNoisescaler;

    float HeightestPiontInTerrian;
    float LowestPointInTerrain;

    Texture2D hmTexture;

    bool showVegetation = false;
    bool showHeights = false;
    bool showSplatMap = false;
    bool showSmooth = false;
    bool showRandom = false;
    bool showLoadHeights = false;
    bool showPerlinNoise = false;
    bool showMultiplePerlins = false;
    bool showVoronoi = false;
    bool midPointShow = false;

    float sHeightestPiontInTerrian;
    GUITableState splatMapTable;
    SerializedProperty splatHeights;

    GUITableState perlinParametersTable;
    SerializedProperty perlinParameters;

    GUITableState vegetationTable;
    SerializedProperty vegetation;
    SerializedProperty maxTrees;
    SerializedProperty treeSpacing;
    //fould outs -------
    void OnEnable()
    {
      randomHeightRange = serializedObject.FindProperty("randomHeightRange");
      heightMapScale= serializedObject.FindProperty("heightMapScale");
      heightMapImage= serializedObject.FindProperty("heightMapImage");
      perlinXScale =  serializedObject.FindProperty("perlinXScale");
      perlinYScale =  serializedObject.FindProperty("perlinYScale");
      perlinOffsetY =  serializedObject.FindProperty("perlinOffsetY");
      perlinOffsetX =  serializedObject.FindProperty("perlinOffsetX");
      perlinOctaves = serializedObject.FindProperty("perlinOctaves");
      perlinPersistance = serializedObject.FindProperty("perlinPersistance");
      perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
      resetTerrain = serializedObject.FindProperty("resetTerrain");
      voronoifallOff = serializedObject.FindProperty("voronoifallOff");
      voronoidropOff = serializedObject.FindProperty("voronoidropOff");
      voronoimaxHeight = serializedObject.FindProperty("voronoimaxHeight");
      voronoimaxWidth = serializedObject.FindProperty("voronoimaxWidth");
      voronoipeakCount = serializedObject.FindProperty("voronoipeakCount");
      voronoiType = serializedObject.FindProperty("voronoiType");
      perlinParametersTable = new GUITableState("perlinParametersTable");
      perlinParameters = serializedObject.FindProperty("perlinParameters");
      midPointHeightMax = serializedObject.FindProperty("midPointHeightMax");
      midPointHeightMin = serializedObject.FindProperty("midPointHeightMin");
      midPointRoughness = serializedObject.FindProperty("midPointRoughness");
      midPointDampener = serializedObject.FindProperty("midPointDampener");
      smoothCount = serializedObject.FindProperty("smoothCount");
      splatMapTable = new GUITableState("splatMapTable");
      splatHeights = serializedObject.FindProperty("splatHeights");
      splatOffset = serializedObject.FindProperty("splatOffset");
      splatNoiseXscale = serializedObject.FindProperty("splatNoiseXscale");
      splatNoiseYscale = serializedObject.FindProperty("splatNoiseYscale");
      splatNoisescaler = serializedObject.FindProperty("splatNoisescaler");
      vegetationTable = new GUITableState("vegetationTable");
      vegetation = serializedObject.FindProperty("vegetation");
      maxTrees = serializedObject.FindProperty("maxTrees");
      treeSpacing = serializedObject.FindProperty("treeSpacing");

      hmTexture = new Texture2D(513, 513, TextureFormat.ARGB32, false);


    }
    Vector2 scrollPos;
    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      CustomTerrain terrain = (CustomTerrain) target;
      Rect r = EditorGUILayout.BeginVertical();
      scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(r.width), GUILayout.Height(r.height));
      EditorGUI.indentLevel++;
      EditorGUILayout.PropertyField(resetTerrain);

      showRandom = EditorGUILayout.Foldout(showRandom, "Random");
      if(showRandom)
      {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Set Heights Between Random Values", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(randomHeightRange);
        if(GUILayout.Button("RandomHeights"))
        {
          terrain.RandomTerrain();
        }
      }
      showLoadHeights = EditorGUILayout.Foldout(showLoadHeights, "Load Heights");
      if(showLoadHeights)
      {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Load Heights From Texture", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(heightMapImage);
        EditorGUILayout.PropertyField(heightMapScale);
        if(GUILayout.Button("Load Texture"))
        {
          terrain.LoadTexture();
        }
      }
      showPerlinNoise = EditorGUILayout.Foldout(showPerlinNoise , "Perlin Noise");
      if(showPerlinNoise)
      {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Perlin Noise", EditorStyles.boldLabel);
        EditorGUILayout.Slider(perlinXScale, 0, 1, new GUIContent("perlinXScale"));
        EditorGUILayout.Slider(perlinYScale, 0, 1, new GUIContent("perlinYScale"));
        EditorGUILayout.IntSlider(perlinOffsetX, 0, 10000, new GUIContent("perlinOffsetY"));
        EditorGUILayout.IntSlider(perlinOffsetY, 0, 10000, new GUIContent("perlinOffsetY"));
        EditorGUILayout.IntSlider(perlinOctaves, 0, 10, new GUIContent("Octaves"));
        EditorGUILayout.Slider(perlinPersistance, 0.1f, 10, new GUIContent("Persistance"));
        EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("Height Scale"));
        if(GUILayout.Button("Perlin"))
        {
          terrain.Perlin();
        }
      }
      showMultiplePerlins = EditorGUILayout.Foldout(showMultiplePerlins , "Mulitple Perlin Noise");
      if(showMultiplePerlins)
      {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Mulitple Perlin Niose", EditorStyles.boldLabel);
        perlinParametersTable = GUITableLayout.DrawTable(perlinParametersTable, perlinParameters);
        GUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("+"))
        {
          terrain.AddNewPerlin();
        }
        if(GUILayout.Button("-"))
        {
          terrain.RemovePerlin();
        }
        EditorGUILayout.EndHorizontal();
        if(GUILayout.Button("Apply Mutiple Perlin"))
        {
          terrain.MultiplePerlinTerrain();
        }
      }
      showVoronoi = EditorGUILayout.Foldout(showVoronoi, "Voronoi");
      if(showVoronoi)
      {
        EditorGUILayout.IntSlider(voronoipeakCount, 1, 10, new GUIContent("Peak Count"));
        EditorGUILayout.Slider(voronoifallOff , 0, 10, new GUIContent("fall Off"));
        EditorGUILayout.Slider(voronoidropOff , 0, 10, new GUIContent("drop Off"));
        EditorGUILayout.Slider(voronoimaxHeight ,0, 1, new GUIContent("max Height"));
        EditorGUILayout.Slider(voronoimaxWidth ,0, 1, new GUIContent("max Width"));
        EditorGUILayout.PropertyField(voronoiType);
        if(GUILayout.Button("Voronoi"))
        {
          terrain.Voronoi();
        }
      }
      midPointShow = EditorGUILayout.Foldout(midPointShow, "Midpoint Displacemnet");
      if(midPointShow)
      {
        EditorGUILayout.PropertyField(midPointHeightMin);
        EditorGUILayout.PropertyField(midPointHeightMax);
        EditorGUILayout.PropertyField(midPointRoughness);
        EditorGUILayout.PropertyField(midPointDampener);
        if(GUILayout.Button("MPD"))
        {
          terrain.MidPointDisplacement();
        }
      }
      showSmooth = EditorGUILayout.Foldout(showSmooth, "Smooth");
      if(showSmooth)
      {
        EditorGUILayout.IntSlider(smoothCount, 0, 10, new GUIContent("Smooth Count"));
        if(GUILayout.Button("Smooth"))
        {
          terrain.Smooth();
        }
      }
      showSplatMap = EditorGUILayout.Foldout(showSplatMap, "Splat Map");
      if(showSplatMap)
      {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Splat Map", EditorStyles.boldLabel);
        GUILayout.Label("Max Height Of Terrain", EditorStyles.boldLabel);
        EditorGUILayout.FloatField(HeightestPiontInTerrian);
        EditorGUILayout.FloatField(LowestPointInTerrain);
        EditorGUILayout.Slider(splatOffset , 0, .1f, new GUIContent("Offset"));
        EditorGUILayout.Slider(splatNoiseXscale , 0.001f, 1, new GUIContent("Noise X scale"));
        EditorGUILayout.Slider(splatNoiseYscale , 0.001f, 1, new GUIContent("Noise Y scale"));
        EditorGUILayout.Slider(splatNoisescaler ,0, 1, new GUIContent("Noise scaler"));
        splatMapTable = GUITableLayout.DrawTable(splatMapTable, serializedObject.FindProperty("splatHeights"));
        //splatMapTable = GUITableLayout.DrawTable(splatMapTable, splatHeights);
        GUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("+"))
        {
          terrain.AddNewSplatHeights();
        }
        if(GUILayout.Button("-"))
        {
          terrain.RemoveSplatHeight();
        }
        EditorGUILayout.EndHorizontal();
        if(GUILayout.Button("Apply Splatmaps"))
        {
          terrain.SplatMaps();
        }
      }
      showVegetation = EditorGUILayout.Foldout(showVegetation, "Vegetation");
      if(showVegetation)
      {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Vegetation", EditorStyles.boldLabel);
        EditorGUILayout.IntSlider(maxTrees,0, 10000, new GUIContent("maximum Trees"));
        EditorGUILayout.IntSlider(treeSpacing, 2, 20, new GUIContent("Trees Spacing"));
        vegetationTable = GUITableLayout.DrawTable(vegetationTable, serializedObject.FindProperty("vegetation"));
        //splatMapTable = GUITableLayout.DrawTable(splatMapTable, splatHeights);
        GUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("+"))
        {
          terrain.AddNewVegetation();
        }
        if(GUILayout.Button("-"))
        {
          terrain.RemoveVegetations();
        }
        EditorGUILayout.EndHorizontal();
        if(GUILayout.Button("Apply Vegetations"))
        {
          terrain.Vegetation();
        }
      }
      showHeights = EditorGUILayout.Foldout(showHeights, "Height Map");
      if(showHeights)
      {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        int hmtSize = (int)(EditorGUIUtility.currentViewWidth - 100);
        GUILayout.Label(hmTexture, GUILayout.Width(hmtSize), GUILayout.Height(hmtSize));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("Refresh", GUILayout.Width(hmtSize)))
        {
          float[,] heightMap = terrain.terrainData.GetHeights(0,0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight);
          for(int y = 0; y < terrain.terrainData.heightmapHeight; y++)
          {
            for(int x = 0; x < terrain.terrainData.heightmapWidth; x++)
            {
              hmTexture.SetPixel(x, y, new Color(heightMap[x, y],
                                                  heightMap[x,y],
                                                  heightMap[x,y], 1));
              if(heightMap[x,y] > HeightestPiontInTerrian)
              {
                HeightestPiontInTerrian = heightMap[x,y];
              }
              if(heightMap[x,y] < LowestPointInTerrain)
              {
                LowestPointInTerrain = heightMap[x,y];
              }
            }
          }
          hmTexture.Apply();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
      }
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      if(GUILayout.Button("Reset"))
      {
        terrain.ResetTerrain();
      }
      EditorGUILayout.EndScrollView();
      EditorGUILayout.EndVertical();
      serializedObject.ApplyModifiedProperties();
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
