using UnityEngine;

namespace Script.Core.Pots.PotTypes
{
    public class PotTypeSelector : MonoBehaviour
    {
        [SerializeField] PotType[] potTypes;
        public enum PotSelectType
        {
            LittlePot,
            Pot,
            Pan
        }
        public PotType GetPotTypeBySelectType(PotSelectType potTypeName)
        {
            return potTypes[(int)potTypeName];
        }
    }
}