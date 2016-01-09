using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

class EnemyDatabase : MonoBehaviour
{
    private List<Enemies> database;
    private JsonSerializer jsonSerializer;
    private JsonReader jsonReader;
    private StreamReader textReader;
    List<int> listInputRecipe;
    private string filename = "/StreamingAssets/Enemies.json";

    void Start()
    {
        database = new List<Enemies>();
        jsonSerializer = new JsonSerializer();
        textReader = File.OpenText(Application.dataPath + filename);
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
    }

    public Enemies getEnemyByID(int id)
    {
        return new Enemies(database[id]);
    }

}

