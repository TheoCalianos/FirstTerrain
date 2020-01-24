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

    bool showRandom = false;
    bool showLoadHeights = false;
    //fould outs -------
    void OnEnable()
    {
      randomHeightRange = serializedObject.FindProperty("randomHeightRange");
      heightMapScale= serializedObject.FindProperty("heightMapScale");
      heightMapImage= serializedObject.FindProperty("heightMapImage");
    }
    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      CustomTerrain terrain = (CustomTerrain) target;

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
