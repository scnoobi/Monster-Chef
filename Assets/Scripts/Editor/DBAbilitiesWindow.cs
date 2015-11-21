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
        chosenScript = EditorGUI.Popup(new Rect(10,10,200,50), chosenScript, fileNames.ToArray());
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
                info.SetValue(tempAbility, database.Count);
                EditorGUILayout.LabelField("ID of item", database.Count.ToString());
            }
        }
        EditorGUILayout.EndVertical();
    }
}