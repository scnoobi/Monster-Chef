using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using UnityEditorInternal;

public class DBEnemies : EditorWindow
{
    MonoScript charScript;
    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    List<Enemies> database;
    JsonSerializer jsonSerializer;
    JsonReader jsonReader;
    JsonWriter jsonWriter;
    StreamReader textReader;
    StreamWriter textWriter;
    string itemFileName = "/StreamingAssets/Enemies.json";
    Enemies tempEnemy;
    object tempStruct;
    List<int> charAbilities;
    ReorderableList reorderableList;
    int tasteToStats = 0;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/DBEnemies")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DBEnemies window = (DBEnemies)EditorWindow.GetWindow(typeof(DBEnemies));
        window.Show();
    }

    void Start()
    {
        jsonSerializer = new JsonSerializer();
        textReader = File.OpenText(Application.dataPath + itemFileName);
        jsonReader = new JsonTextReader(textReader);
        database = JsonConvert.DeserializeObject<List<Enemies>>(textReader.ReadToEnd(), new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        });
        if (database == null)
            database = new List<Enemies>();

        textReader.Close();
        textReader.Dispose();
        jsonReader.Close();

        charAbilities = new List<int>();
        tempEnemy = new Enemies();
    }

    void OnGUI()
    {
        Type targetType;
        charScript = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Enemies/Enemies.cs");
        if (jsonSerializer == null)
        {
            Start();
        }
        targetType = charScript.GetClass();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("id: ", database.Count.ToString());

        if (reorderableList == null)
        {
            reorderableList = new ReorderableList(charAbilities, typeof(int),
                false, true, true, true);

            reorderableList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) => {
                    var element = reorderableList.list[index];
                    rect.y += 2;
                    reorderableList.list[index] = EditorGUI.IntField(
                        new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                        (int)element);
                };
        }
        reorderableList.DoLayoutList();
        
        foreach (FieldInfo info in targetType.GetFields(flags))
        {
            Type fieldType = info.FieldType;
            if (fieldType == typeof(int))
            {
                info.SetValue(tempEnemy, EditorGUILayout.IntField(info.Name, (int)info.GetValue(tempEnemy)));
            }
            else if (fieldType == typeof(string))
            {
                info.SetValue(tempEnemy, EditorGUILayout.TextField(info.Name, (string)info.GetValue(tempEnemy)));
            }
            else if (fieldType == typeof(float))
            {
                info.SetValue(tempEnemy, EditorGUILayout.FloatField(info.Name, (float)info.GetValue(tempEnemy)));
            }
            else if (fieldType == typeof(Enemies.EnemyStats)) //struct
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                if (tempStruct == null)
                {
                    tempStruct = new Enemies.EnemyStats();
                    info.SetValue(tempEnemy, (Enemies.EnemyStats)tempStruct);
                }

                foreach (FieldInfo infoInStruct in fieldType.GetFields(flags))
                {
                    Type fieldTypeInStruct = infoInStruct.FieldType;

                    if (fieldTypeInStruct == typeof(float))
                    {
                        infoInStruct.SetValue(tempStruct, EditorGUILayout.FloatField(infoInStruct.Name, (float)infoInStruct.GetValue(info.GetValue(tempEnemy)) ));
                    }
                }


                info.SetValue(tempEnemy, (Enemies.EnemyStats)tempStruct);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            else if (fieldType == typeof(List<int>))
            {
                info.SetValue(tempEnemy, charAbilities);
            }

        }
        

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("submit"))
        {
            textWriter = new StreamWriter(Application.dataPath + itemFileName);
            jsonWriter = new JsonTextWriter(textWriter);
            database.Add(tempEnemy);
            String text = JsonConvert.SerializeObject(database, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            textWriter.Write(text);

            textWriter.Close();
            textWriter.Dispose();
            jsonWriter.Close();

            tempEnemy = new Enemies();
            tempStruct = null;
            reorderableList = null;
            charAbilities = new List<int>();
        }
    }
}
