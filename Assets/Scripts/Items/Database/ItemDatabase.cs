using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

public class ItemDatabase : MonoBehaviour {
    private List<Item> database = new List<Item>();
    private JsonSerializer jsonSerializer;
    private JsonReader jsonReader;
    private StreamReader textReader; 
    private string filename = "/StreamingAssets/Items.json";

    void Start() {
        jsonSerializer = new JsonSerializer();
        textReader = File.OpenText(Application.dataPath + filename);
        jsonReader = new JsonTextReader(textReader);
        //database = jsonSerializer.Deserialize<List<Item>>(jsonReader);
    }

}
