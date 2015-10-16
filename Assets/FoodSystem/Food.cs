using UnityEngine;
using System.Collections;

public abstract class Food : MonoBehaviour {

    public struct taste
    {
        int saltyness;
        int spicyness;
        int sweetness;
        int wetness;
        int fat;

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

    public enum cookingType { fried, roasting, stewing}
    public float timeToCook;
    public string name;
    public taste foodTaste;
    public TypeOfFood typeOfFood;
    public int quantity;

	// Use this for initialization
	void Start () {
	
	}

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
	
	// Update is called once per frame
	void Update () {
	
	}
}
