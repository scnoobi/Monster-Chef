using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;


class AbilityDatabase : MonoBehaviour
{
    private List<Ability> database;
    private JsonSerializer jsonSerializer;
    private JsonReader jsonReader;
    private StreamReader textReader;
    List<int> listInputRecipe;
    private string filename = "/StreamingAssets/Abilities.json";

    void Start()
    {
        database = new List<Ability>();
        jsonSerializer = new JsonSerializer();
        textReader = File.OpenText(Application.dataPath + filename);
        jsonReader = new JsonTextReader(textReader);
        database = JsonConvert.DeserializeObject<List<Ability>>(textReader.ReadToEnd(), new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        });
        if (database == null)
            database = new List<Ability>();

        textReader.Close();
        textReader.Dispose();
        jsonReader.Close();
    }

    public Ability getAbilityById(int id)
    {
        return database[id];
    }


}
