using System;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuildingSelectMenu : MonoBehaviour
    {
        [SerializeField] private BuildingInfoMenu buildingInfoMenu;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button deleteButton;

        private BuildingModel buildingModel;
        private MapController mapController;

        public void SetBuilding(BuildingModel buildingModel)
        {
            this.buildingModel = buildingModel;
        }

        public void Init(MapController mapController)
        {
            this.mapController = mapController;
        }

        private void Awake()
        {
            infoButton.onClick.AddListener(delegate
            {
                buildingInfoMenu.gameObject.SetActive(true);
                buildingInfoMenu.ShowBuildingInfo(buildingModel);
                gameObject.SetActive(false);
            });

            deleteButton.onClick.AddListener(delegate
            {
                mapController.UnplaceBuilding(buildingModel);
                gameObject.SetActive(false);
            });
        }
    }
}