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

    bool showRandom = false;
    bool showLoadHeights = false;
    bool showPerlinNoise = false;
    bool showMultiplePerlins = false;

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

      perlinParametersTable = new GUITableState("perlinParametersTable");
      perlinParameters = serializedObject.FindProperty("perlinParameters");

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
