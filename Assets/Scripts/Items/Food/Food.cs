using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public abstract class Food : Item {

    [System.Serializable]
    public struct taste
    {
        public int salt;
        public int sweet;
        public int bitter;
        public int sour;
        public int umami;
        public int spicy;
        public int tender;
        public int fat;

        public taste(int slt, int sweet, int bitter, int sour, int umami, int spicy, int tender, int fat) {
            this.salt = slt;
            this.sweet = sweet;
            this.bitter = bitter;
            this.sour = sour;
            this.umami = umami;
            this.spicy = spicy;
            this.tender = tender;
            this.fat = fat;
        }

        public void complexTaste(taste taste){
            this.salt += taste.salt;
            this.sweet += taste.sweet;
            this.bitter += taste.bitter;
            this.sour += taste.sour;
            this.umami += taste.umami;
            this.spicy += taste.spicy;
            this.tender += taste.tender;
            this.fat += taste.fat;
        }
    }

    public enum cookingType { raw, fried, roasting, stewing} //cooking types

        [JsonConverter(typeof(StringEnumConverter))]
    public cookingType currentCookingMethod;
    public float timeToCook;
    /*
            [JsonConverter(typeof(StringEnumConverter))]
    public TypeOfFood typeOfFood;
     * */

    public taste foodTaste;

    void cook(cookingType typeOfCooking, float time) {
        switch (typeOfCooking) {
            case cookingType.fried:

                break;

            case cookingType.roasting:

                break;

            case cookingType.stewing:

                break;

            default:
               
                break;
        }
    }
}
