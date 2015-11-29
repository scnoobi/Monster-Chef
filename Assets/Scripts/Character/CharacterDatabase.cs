using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;


class CharacterDatabase : MonoBehaviour
{
    private List<Character> database;
    private JsonSerializer jsonSerializer;
    private JsonReader jsonReader;
    private StreamReader textReader;
    List<int> listInputRecipe;
    private string filename = "/StreamingAssets/Characters.json";

    void Start()
    {
        database = new List<Character>();
        jsonSerializer = new JsonSerializer();
        textReader = File.OpenText(Application.dataPath + filename);
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
    }

    public Character getCharacterById(int id)
    {
        return database[id];
    }


}
