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

    BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

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
        food = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Items/Food/Food.cs");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        Type targetType = food.GetClass();

        foreach (FieldInfo info in targetType.GetFields(flags))
        {
            Type fieldType = info.FieldType;
            if (fieldType.IsEnum) {
                object weirdThing = Activator.CreateInstance(fieldType);
                EditorGUILayout.EnumPopup(info.Name, (Enum)weirdThing);
            }
            else if (fieldType == typeof(string))
            {
                EditorGUILayout.TextField(info.Name, "");
            }
            else if (fieldType == typeof(float))
            {
                EditorGUILayout.FloatField(info.Name, 0);
            }
            else if (fieldType.IsValueType && !fieldType.IsPrimitive) //struct
            {
                EditorGUILayout.Space();
                //Debug.Log(fieldType.Name + "  " + info.Name);
                foreach (FieldInfo infoInStruct in fieldType.GetFields(flags))
                {
                    Type fieldTypeInStruct = infoInStruct.FieldType;
                    //Debug.Log(fieldTypeInStruct.Name + "  " + infoInStruct.Name);
                    EditorGUILayout.IntField(infoInStruct.Name, 0);
                }
                EditorGUILayout.Space();
            }
            else { //class
                Debug.Log(fieldType.Name + "  " + info.Name);
            }
        }

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("submit"))
        {


        }
	}
}