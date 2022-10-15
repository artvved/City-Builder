using System;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuildingInfoMenu : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI text;
        [SerializeField]private Button closeButton;

        private void Awake()
        {
            closeButton.onClick.AddListener(Hide);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowBuildingInfo(BuildingModel model)
        {
            var s = model.Size * model.Size;
            
            text.text = "Площадь здания в клетках: "+s+"!";
        }
    }
}