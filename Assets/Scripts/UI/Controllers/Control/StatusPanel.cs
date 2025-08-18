using UnityEngine;
using UnityEngine.UI;
using System;

namespace AD.UI
{
    public class StatusPanel : MonoBehaviour
    {
        /****** Public Members ******/

        public void UpdateHPBar(int currentHP, int maxHP)
        {
            Debug.Assert(null != _hpBarFillImage, "HP bar fill image is not assigned");
            Debug.Assert(0 <= currentHP && currentHP <= maxHP, $"Invalid HP values: {currentHP}/{maxHP}");
            
            float fillAmount = maxHP > 0 ? (float)currentHP / maxHP : 0f;
            _hpBarFillImage.fillAmount = fillAmount;
        }


        /****** Private Members ******/

        [SerializeField] private Image _hpBarFillImage;

        private void OnValidate()
        {
            Debug.Assert(null != _hpBarFillImage, "Hp bar fill image is not assigned in status panel");
        }
    }
}