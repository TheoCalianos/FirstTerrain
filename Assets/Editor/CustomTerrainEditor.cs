﻿using System.Collections;
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

    bool showSmooth = false;
    bool showRandom = false;
    bool showLoadHeights = false;
    bool showPerlinNoise = false;
    bool showMultiplePerlins = false;
    bool showVoronoi = false;
    bool midPointShow = false;

    GUITableState perlinParametersTable;
    SerializedProperty perlinParameters;
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

    }
    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      CustomTerrain terrain = (CustomTerrain) target;

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
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      if(GUILayout.Button("Reset"))
      {
        terrain.ResetTerrain();
      }

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
