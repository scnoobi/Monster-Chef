using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using UnityEditorInternal;

public class DBCharacter : EditorWindow
{
    MonoScript charScript;
    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    List<Character> database;
    JsonSerializer jsonSerializer;
    JsonReader jsonReader;
    JsonWriter jsonWriter;
    StreamReader textReader;
    StreamWriter textWriter;
    string itemFileName = "/StreamingAssets/Characters.json";
    Character tempChar;
    object tempStruct;
    List<int> charAbilities;
    ReorderableList reorderableList;
    int tasteToStats = 0;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/DBCharacter")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DBCharacter window = (DBCharacter)EditorWindow.GetWindow(typeof(DBCharacter));
        window.Show();
    }

    void Start()
    {
        jsonSerializer = new JsonSerializer();
        textReader = File.OpenText(Application.dataPath + itemFileName);
        jsonReader = new JsonTextReader(textReader);
        database = JsonConvert.DeserializeObject<List<Character>>(textReader.ReadToEnd(), new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        });
        if (database == null)
            database = new List<Character>();

        textReader.Close();
        textReader.Dispose();
        jsonReader.Close();

        charAbilities = new List<int>();
        tempChar = new Character();
    }

    void OnGUI()
    {
        Type targetType;
        charScript = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Character/Character.cs");
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
                info.SetValue(tempChar, EditorGUILayout.IntField(info.Name, (int)info.GetValue(tempChar)));
            }
            else if (fieldType.IsEnum)
            {
                if (info.GetValue(tempChar) == null)
                    info.SetValue(tempChar, Activator.CreateInstance(fieldType));
                info.SetValue(tempChar, EditorGUILayout.EnumPopup(info.Name, (Enum)info.GetValue(tempChar)));
            }
            else if (fieldType == typeof(string))
            {
                string name = (string)info.GetValue(tempChar);
                info.SetValue(tempChar, EditorGUILayout.TextField(info.Name, name));
            }
            else if (fieldType == typeof(float))
            {
                info.SetValue(tempChar, EditorGUILayout.FloatField(info.Name, (float)info.GetValue(tempChar)));
            }
            else if (fieldType.IsValueType && !fieldType.IsPrimitive) //struct
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                if (fieldType == typeof(Character.Stats))
                    tempStruct = new Character.Stats();

                foreach (FieldInfo infoInStruct in fieldType.GetFields(flags))
                {
                    Type fieldTypeInStruct = infoInStruct.FieldType;

                    if (fieldTypeInStruct == typeof(float))
                    {
                        infoInStruct.SetValue(tempStruct, EditorGUILayout.FloatField(infoInStruct.Name, (float)infoInStruct.GetValue(info.GetValue(tempChar))));
                    }
                }

                if (fieldType == typeof(Character.Stats))
                {
                    info.SetValue(tempChar, (Character.Stats)tempStruct);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            else if (fieldType == typeof(TasteToStats))//object
            {
                var dirPath = new DirectoryInfo("Assets/Scripts/Character/TasteTranslation");
                FileInfo[] fileInfo = dirPath.GetFiles();
                List<string> fileNames = new List<string>();
                for (int i = 0; i < fileInfo.Length; i++)
                {
                    if (!fileInfo[i].Name.Contains("meta") && !fileInfo[i].Name.Equals("TasteToStats.cs"))
                    {
                        fileNames.Add(fileInfo[i].Name);
                    }
                }
                tasteToStats = EditorGUILayout.Popup("Ability:", tasteToStats, fileNames.ToArray());
            }






        }


        EditorGUILayout.EndVertical();

        if (GUILayout.Button("submit"))
        {
            textWriter = new StreamWriter(Application.dataPath + itemFileName);
            jsonWriter = new JsonTextWriter(textWriter);
            database.Add(tempChar);
            String text = JsonConvert.SerializeObject(database, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            textWriter.Write(text);

            textWriter.Close();
            textWriter.Dispose();
            jsonWriter.Close();

            tempChar = null;

        }
    }
}
