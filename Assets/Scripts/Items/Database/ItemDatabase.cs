using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

public class ItemDatabase : MonoBehaviour {
    private List<Item> database;
    List<ComposedFood.recipe> recipes;
    private JsonSerializer jsonSerializer;
    private JsonReader jsonReader;
    private StreamReader textReader;
    List<int> listInputRecipe;
    private string filename = "/StreamingAssets/Items.json";
    string recipesFileName = "/StreamingAssets/Recipes.json";

    void Start() {
        database = new List<Item>();
        jsonSerializer = new JsonSerializer();
        textReader = File.OpenText(Application.dataPath + filename);
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

        foreach(Item item in database)
        {
            Debug.Log(item.name +" in DB");
        }
    }

    public ComposedFood getCraftedFood(List<int> ingredients)
    {
        ComposedFood craftedFood = null;
        int chosenFood = -1;
        int sizeOfMatches = 0;
        for(int i = 0; i < recipes.Count; i++)
        {
            int currSizeOfMatches = 0;
            sizeOfMatches = recipes[i].input.Count;
            for (int j = 0; j < recipes[i].input.Count; j++)
            {
                bool isSubsect = ingredients.Contains(recipes[i].input[j]);
                if(isSubsect)
                    currSizeOfMatches++;
            }
            if (currSizeOfMatches == sizeOfMatches) {
                chosenFood = recipes[i].output;
                sizeOfMatches = currSizeOfMatches;
            }
        }

        if (chosenFood > 0)
        {
            craftedFood = (ComposedFood)database[chosenFood];
        }

        return craftedFood;
    }

    public Item getItemByName(string name)
    {
        Item item = null;
        for(int i = 0; i < database.Count; i++)
        {
            if (database[i].realName.Equals(name))
            {
                item = database[i];
                break;
            }
        }
        return item;
    }
}
