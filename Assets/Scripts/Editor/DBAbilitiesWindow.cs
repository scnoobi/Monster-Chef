using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using UnityEditorInternal;

public class DBAbilitiesWindow : EditorWindow {

    MonoScript abilityScript;
    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    List<Ability> database;
    JsonSerializer jsonSerializer;
    JsonReader jsonReader;
    JsonWriter jsonWriter;
    StreamReader textReader;
    StreamWriter textWriter;
    string itemFileName = "/StreamingAssets/Abilities.json";
    Ability tempAbility;
    int chosenScript = 0;

    // Add menu named "My Window" to the Window menu
    [MenuItem ("Window/AbilitiesWindow")]
	static void Init () {
        // Get existing open window or if none, make a new one:
        DBAbilitiesWindow window = (DBAbilitiesWindow)EditorWindow.GetWindow(typeof(DBAbilitiesWindow));
		window.Show();
	}

    void Start() {
        jsonSerializer = new JsonSerializer();
        textReader = File.OpenText(Application.dataPath + itemFileName);
        jsonReader = new JsonTextReader(textReader);
        database = JsonConvert.DeserializeObject<List<Ability>>(textReader.ReadToEnd(), new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        });
        if (database == null)
        {
            database = new List<Ability>();
        }

        textReader.Close();
        textReader.Dispose();
        jsonReader.Close();
    }

	void OnGUI () {
        Type targetType;

        if (jsonSerializer == null)
        {
            Start();
        }

        var dirPath = new DirectoryInfo("Assets/Scripts/Abilities/");
        FileInfo[] fileInfo = dirPath.GetFiles();
        List<string> fileNames = new List<string>();
        for(int i = 0; i < fileInfo.Length; i++)
        {
            if (!fileInfo[i].Name.Contains("meta") && !fileInfo[i].Name.Equals("Ability.cs"))
            {
                fileNames.Add(fileInfo[i].Name);
            }
        }
        chosenScript = EditorGUILayout.Popup("Ability:",chosenScript, fileNames.ToArray());
        abilityScript = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Abilities/"+ fileNames[chosenScript]);
        targetType = abilityScript.GetClass();
        if (tempAbility == null)
        {
            tempAbility = (Ability)Activator.CreateInstance(targetType);
        }
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        foreach (FieldInfo info in targetType.GetFields(flags))
        {
            Type fieldType = info.FieldType;
            if (fieldType == typeof(int))
            {
                if (info.Name.Equals("id")) {
                    info.SetValue(tempAbility, database.Count);
                    EditorGUILayout.LabelField(info.Name, database.Count.ToString());
                }
                else
                {
                    info.SetValue(tempAbility, EditorGUILayout.IntField(info.Name, (int)info.GetValue(tempAbility)));
                }
            }else if(fieldType == typeof(string))
            {
                info.SetValue(tempAbility, EditorGUILayout.TextField(info.Name, (string)info.GetValue(tempAbility)));
            }
        }

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("submit"))
        {
            textWriter = new StreamWriter(Application.dataPath + itemFileName);
            jsonWriter = new JsonTextWriter(textWriter);
            database.Add(tempAbility);
            String text = JsonConvert.SerializeObject(database, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            textWriter.Write(text);

            textWriter.Close();
            textWriter.Dispose();
            jsonWriter.Close();

            tempAbility = null;
        }
    }
}