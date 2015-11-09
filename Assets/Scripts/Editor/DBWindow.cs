using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;

public class DBWindow : EditorWindow {

    MonoScript food;
    BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    List<Item> database;
    JsonSerializer jsonSerializer;
    JsonReader jsonReader;
    JsonWriter jsonWriter;
    StreamReader textReader;
    StreamWriter textWriter;
    string filename = "/StreamingAssets/Items.json";
    Food tempItem;
    object tempTaste;


	// Add menu named "My Window" to the Window menu
	[MenuItem ("Window/My Window")]
	static void Init () {
		// Get existing open window or if none, make a new one:
        DBWindow window = (DBWindow)EditorWindow.GetWindow(typeof(DBWindow));
		window.Show();
	}

    void Start() {
        jsonSerializer = new JsonSerializer();
        textReader = File.OpenText(Application.dataPath + filename);
        jsonReader = new JsonTextReader(textReader);
        //database = jsonSerializer.Deserialize<List<Item>>(jsonReader);
        database = JsonConvert.DeserializeObject<List<Item>>(textReader.ReadToEnd(), new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        });
        if (database == null)
            database = new List<Item>();
        textReader.Close();
        textReader.Dispose();
        jsonReader.Close();
    }

	void OnGUI () {
        if (jsonSerializer == null) {
            Start();
        }
        food = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Items/Food/Food.cs");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        Type targetType = food.GetClass();

        if (tempItem == null)
        {
            tempItem = new MainIngredient();
            tempTaste = new Food.taste();
        }

        foreach (FieldInfo info in targetType.GetFields(flags))
        {
            Type fieldType = info.FieldType;
            if (fieldType.IsEnum) {
                //EditorGUILayout.EnumPopup(info.Name, (Enum)weirdThing);
                if (info.GetValue(tempItem) == null)
                    info.SetValue(tempItem, Activator.CreateInstance(fieldType));
                info.SetValue(tempItem, EditorGUILayout.EnumPopup(info.Name, (Enum)info.GetValue(tempItem)));
            }
            else if (fieldType == typeof(string))
            {
                string name = (String)info.GetValue(tempItem);
                info.SetValue(tempItem, EditorGUILayout.TextField(info.Name, name));
            }
            else if (fieldType == typeof(float))
            {
                info.SetValue(tempItem, EditorGUILayout.FloatField(info.Name, (float)info.GetValue(tempItem)));
            }
            else if (fieldType.IsValueType && !fieldType.IsPrimitive) //struct
            {
                EditorGUILayout.Space();
                foreach (FieldInfo infoInStruct in fieldType.GetFields(flags))
                {
                    Type fieldTypeInStruct = infoInStruct.FieldType;
                    infoInStruct.SetValue(tempTaste, EditorGUILayout.IntField(infoInStruct.Name, (int)infoInStruct.GetValue(tempTaste)));
                }
                info.SetValue(tempItem, (Food.taste)tempTaste);
                
                EditorGUILayout.Space();
            }
            else { //class
                Debug.Log(fieldType.Name + "  " + info.Name);
            }
        }

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("submit"))
        {
            textWriter = new StreamWriter(Application.dataPath + filename);
            jsonWriter = new JsonTextWriter(textWriter);
            database.Add(tempItem);
            String text = JsonConvert.SerializeObject(database, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            textWriter.Write(text);
            textWriter.Close();
            textWriter.Dispose();
            jsonWriter.Close();
            tempItem = null;
        }
	}
}