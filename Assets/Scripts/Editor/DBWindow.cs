using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;

public class DBWindow : EditorWindow {

    MonoScript food;
    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    List<Item> database;
    List<ComposedFood.recipe> recipes;
    JsonSerializer jsonSerializer;
    JsonReader jsonReader;
    JsonWriter jsonWriter;
    StreamReader textReader;
    StreamWriter textWriter;
    string itemFileName = "/StreamingAssets/Items.json";
    string recipesFileName = "/StreamingAssets/Recipes.json";
    object tempStruct;
    Food tempItem;
    bool simpleFood = true;
    bool mainIngredient = true;


    // Add menu named "My Window" to the Window menu
    [MenuItem ("Window/ItemDB")]
	static void Init () {
		// Get existing open window or if none, make a new one:
        DBWindow window = (DBWindow)EditorWindow.GetWindow(typeof(DBWindow));
		window.Show();
	}

    void Start() {
        jsonSerializer = new JsonSerializer();
        textReader = File.OpenText(Application.dataPath + itemFileName);
        jsonReader = new JsonTextReader(textReader);
        database = JsonConvert.DeserializeObject<List<Item>>(textReader.ReadToEnd(), new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        });
        if (database == null)
            database = new List<Item>();

        textReader = File.OpenText(Application.dataPath + recipesFileName);
        jsonReader = new JsonTextReader(textReader);
        recipes = JsonConvert.DeserializeObject<List<ComposedFood.recipe>>(textReader.ReadToEnd(), new JsonSerializerSettings
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
        Type targetType;
        simpleFood = EditorGUILayout.Toggle("Is Simple Food", simpleFood);
        if (simpleFood)
        {
            mainIngredient = EditorGUILayout.Toggle("Is Main Ingredient", mainIngredient);
            if (mainIngredient)
            {
                food = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Items/Food/MainIngredient.cs");
                targetType = food.GetClass();
                if (tempItem == null || tempItem.GetType() != targetType)
                {
                    tempItem = new MainIngredient();
                }
            }
            else
            {
                food = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Items/Food/Accompaniment.cs");
                targetType = food.GetClass();
                if (tempItem == null || tempItem.GetType() != targetType)
                {
                    tempItem = new Accompaniment();
                }
            }
        }
        else
        {
            food = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Items/Food/ComposedFood.cs");
            targetType = food.GetClass();
            if (tempItem == null || tempItem.GetType() != targetType)
            {
                tempItem = new ComposedFood();
            }
        }

        if (jsonSerializer == null) {
            Start();
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        foreach (FieldInfo info in targetType.GetFields(flags))
        {
            Type fieldType = info.FieldType;
            if (fieldType == typeof(int))
            {
                info.SetValue(tempItem, database.Count);
                EditorGUILayout.LabelField("ID of item", database.Count.ToString());
            }
            else if (fieldType.IsEnum) {
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

                
                if (fieldType == typeof(Food.taste))
                    tempStruct = new Food.taste();
                else
                if (fieldType == typeof(ComposedFood.recipe))
                    tempStruct = new ComposedFood.recipe();
                    

                Debug.Log(info.Name);
                foreach (FieldInfo infoInStruct in fieldType.GetFields(flags))
                {
                    Type fieldTypeInStruct = infoInStruct.FieldType;

                    if (fieldTypeInStruct == typeof(int))
                    {
                        if (!infoInStruct.Name.Equals("output"))
                        {
                            infoInStruct.SetValue(tempStruct, EditorGUILayout.IntField(infoInStruct.Name, (int)infoInStruct.GetValue(info.GetValue(tempItem))));
                        }
                    }
                    else if (fieldTypeInStruct.IsArray)
                    {
                    }
                }

                if (fieldType == typeof(Food.taste))
                {
                    info.SetValue(tempItem, (Food.taste)tempStruct); //needs a casting on the tempStruct
                }
                else if(fieldType == typeof(ComposedFood.recipe))
                {
                    info.SetValue(tempItem, (ComposedFood.recipe)tempStruct);
                }

                Debug.Log("========================================================");
                EditorGUILayout.Space();
            }//else { //class
                //Debug.Log(fieldType.Name + "  " + info.Name);
            //}
        }

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("submit"))
        {
            textWriter = new StreamWriter(Application.dataPath + itemFileName);
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