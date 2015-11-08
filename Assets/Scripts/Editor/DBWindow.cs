using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class DBWindow : EditorWindow {
	string myString = "Hello World";
	bool groupEnabled;
	bool myBool = true;
	float myFloat = 1.23f;

    MonoScript food;

    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    List<int> layerNumbers = new List<int>();

    Regex titleRegex = new Regex("([a-z1-9])([A-Z1-9])");

	// Add menu named "My Window" to the Window menu
	[MenuItem ("Window/My Window")]
	static void Init () {
		// Get existing open window or if none, make a new one:
        DBWindow window = (DBWindow)EditorWindow.GetWindow(typeof(DBWindow));
		window.Show();
        
	}
	
	void OnGUI () {
        food = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Items/Food/MainIngredient.cs");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        Type targetType = food.GetClass();
        //Debug.Log(targetType);
        //Debug.Log(targetType.GetFields(flags).Length);
        foreach (FieldInfo info in targetType.GetFields(flags))
        {
            Type fieldType = info.FieldType;
            Debug.Log(fieldType.Name +"  "+info.Name);
            if (fieldType.IsEnum) {
                object weirdThing = Activator.CreateInstance(fieldType);
                EditorGUILayout.EnumPopup(info.Name, (Enum)weirdThing);
            }else
            EditorGUILayout.TextField(info.Name, "");
        }

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("submit"))
        {


        }
	}
}