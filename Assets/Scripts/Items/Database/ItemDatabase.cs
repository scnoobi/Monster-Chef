using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

public class ItemDatabase : MonoBehaviour {
    private List<Item> database = new List<Item>();
    List<ComposedFood.recipe> recipes;
    private JsonSerializer jsonSerializer;
    private JsonReader jsonReader;
    private StreamReader textReader;
    List<int> listInputRecipe;
    private string filename = "/StreamingAssets/Items.json";
    string recipesFileName = "/StreamingAssets/Recipes.json";

    void Start() {
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
    }

    ComposedFood getCraftedFood(List<int> ingredients)
    {
        ComposedFood craftedFood = null;
        int chosenFood = -1;
        int sizeOfMatches = 0;
        for(int i = 0; i < recipes.Count; i++)
        {
            bool isSubsect = false;
            int currSizeOfMatches = 0;
            for (int j = 0; j < recipes[i].input.Count; j++)
            {
                isSubsect |= ingredients.Contains(recipes[i].input[j]);
                currSizeOfMatches++;
            }
            if (currSizeOfMatches > sizeOfMatches) {
                chosenFood = recipes[i].output;
                sizeOfMatches = currSizeOfMatches;
            }
        }

        if (chosenFood > 0)
            craftedFood = (ComposedFood)database[chosenFood];

        return craftedFood;
    }
}
