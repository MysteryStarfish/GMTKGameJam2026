using Script.Core.Ingredients;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Core
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int levelNumber;
        public string levelName;
        public int activePotsAmount;
        public Ingredient[] ingredients;
    }
}