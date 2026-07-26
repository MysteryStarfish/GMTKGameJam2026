using UnityEngine;

namespace Script.Core.Crystals
{
    public class CrystalHandler : MonoBehaviour
    {
        [SerializeField] private Crystal[] crystals;
        private int maxMagicPower;
        private int currentMagicPower;
        

        public void ActiveCrystal(int activeAmount)
        {
            maxMagicPower = activeAmount;
            currentMagicPower = maxMagicPower;
            int index = activeAmount - 1;
            foreach (Crystal crystal in crystals)
            {
                crystal.crystalImage.enabled = index >= 0;
                if (index >= 0) crystal.TurnOn();
                index--;
            }
        }

        public void TurnOffCrystal()
        {
            if (currentMagicPower >= maxMagicPower)  return;
            crystals[currentMagicPower - 1].TurnOff();
            currentMagicPower--;
        }
    }
}