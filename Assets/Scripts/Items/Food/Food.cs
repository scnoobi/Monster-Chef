using UnityEngine;
using System.Collections;

public abstract class Food : Item {

    [System.Serializable]
    public struct taste
    {
        public int saltyness;
        public int spicyness;
        public int sweetness;
        public int wetness;
        public int fat;

        public taste(int slt, int spc, int sweet, int wet, int fat) {
            this.saltyness = slt;
            this.spicyness = spc;
            this.sweetness = sweet;
            this.wetness = wet;
            this.fat = fat;
        }

        public void complexTaste(taste taste){
            this.saltyness += taste.saltyness;
            this.spicyness += taste.spicyness;
            this.sweetness += taste.sweetness;
            this.wetness += taste.wetness;
            this.fat += taste.fat;
        }
    }

    public enum cookingType { raw, fried, roasting, stewing} //cooking types
    public Food.cookingType currentCookingMethod;
    public float timeToCook;
    public string name;
    public taste foodTaste;
    public TypeOfFood typeOfFood;

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
