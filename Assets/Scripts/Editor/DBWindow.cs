using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using UnityEditorInternal;

public class DBWindow : EditorWindow {

    MonoScript itemScript;
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
    object tempRecipeStruct;
    List<int> listInputRecipe;
    Food tempItem;
    bool simpleFood = true;
    bool mainIngredient = true;
    ReorderableList reorderableList;


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

        textReader.Close();
        textReader.Dispose();
        jsonReader.Close();

        listInputRecipe = new List<int>();
        textReader = File.OpenText(Application.dataPath + recipesFileName);
        jsonReader = new JsonTextReader(textReader);
        recipes = JsonConvert.DeserializeObject<List<ComposedFood.recipe>>(textReader.ReadToEnd(), new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        });
        if (recipes == null)
            recipes = new List<ComposedFood.recipe>();
            

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
                itemScript = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Items/Food/MainIngredient.cs");
                targetType = itemScript.GetClass();
                if (tempItem == null || tempItem.GetType() != targetType)
                {
                    tempItem = new MainIngredient();
                }
            }
            else
            {
                itemScript = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Items/Food/Accompaniment.cs");
                targetType = itemScript.GetClass();
                if (tempItem == null || tempItem.GetType() != targetType)
                {
                    tempItem = new Accompaniment();
                }
            }
        }
        else
        {
            itemScript = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Items/Food/ComposedFood.cs");
            targetType = itemScript.GetClass();
            if (tempItem == null || tempItem.GetType() != targetType)
            {
                tempItem = new ComposedFood();
            }

            if (reorderableList == null)
            {
                reorderableList = new ReorderableList(listInputRecipe, typeof(int),
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
                    
                foreach (FieldInfo infoInStruct in fieldType.GetFields(flags))
                {
                    Type fieldTypeInStruct = infoInStruct.FieldType;

                    if (fieldTypeInStruct == typeof(int))
                    {
                            infoInStruct.SetValue(tempStruct, EditorGUILayout.IntField(infoInStruct.Name, (int)infoInStruct.GetValue(info.GetValue(tempItem))));
                    }
                }

                if (fieldType == typeof(Food.taste))
                {
                    info.SetValue(tempItem, (Food.taste)tempStruct);
                }
                EditorGUILayout.Space();
            }
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

            if (!simpleFood) {
                tempRecipeStruct = new ComposedFood.recipe(listInputRecipe, tempItem.id);
                textWriter = new StreamWriter(Application.dataPath + recipesFileName);
                jsonWriter = new JsonTextWriter(textWriter);
                recipes.Add((ComposedFood.recipe)tempRecipeStruct);
                String recipeText = JsonConvert.SerializeObject(recipes, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                });
                textWriter.Write(recipeText);

                textWriter.Close();
                textWriter.Dispose();
                jsonWriter.Close();
            }
            tempItem = null;
            tempStruct = null;
            reorderableList = null;
            listInputRecipe = new List<int>();
        }
	}
}