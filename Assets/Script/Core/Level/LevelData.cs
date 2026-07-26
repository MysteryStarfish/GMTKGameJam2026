using System;
using Script.Core.Ingredients;
using Script.Core.Pots.PotTypes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Core
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int levelNumber;
        public string levelName;
        public int magicUseAmount;
        public int activePotsAmount;
        public Ingredient[] ingredients;
        public PotType[] pots;
        public Sprite DishIcon;

        // public void Setup()
        // {
        //     int index = 0;
        //     foreach (var ingredient in ingredients)
        //     {
        //         pots[index] = ingredient.potType;
        //         index++;
        //     }
        // }
    }
}