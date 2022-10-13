using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private MapController mapController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private SaveLoadController saveLoadController;
        [SerializeField] private Camera camera;

        [Header("UI: Build")] [SerializeField] private Button buildButton;
        [SerializeField] private GameObject buildMenu;
        [SerializeField] private Button spawnButton1;
        [SerializeField] private Button spawnButton2;
        [SerializeField] private Button spawnButton3;
        [SerializeField] private Button finishBuildingButton;
        [Header("UI: Prepare")] [SerializeField] private Button prepareButton;
        [Header("UI: Save Load")] [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        
        
        private GameState gameState;

        private BuildingModel tmpBuilding;
        private int tmpSize;
        
        private Plane plane;

        
        private void Awake()
        {

            mapController.CreateMap();
            
            saveButton.onClick.AddListener(delegate
            {
                saveLoadController.Save(mapController.MapModel);
                
            });
            loadButton.onClick.AddListener(delegate
            {
                var map=saveLoadController.Load();
                mapController.CreateMap(map);
                
            });
            buildButton.onClick.AddListener(delegate
            {
                SwitchState(GameState.CHOOSE_BUILDING);
                buildMenu.gameObject.SetActive(true);
                if (tmpBuilding!=null)
                {
                    Destroy(tmpBuilding.BuildingView.gameObject);
                    tmpBuilding = null;
                }
                
            });
            finishBuildingButton.onClick.AddListener(delegate
            {
                FinishNewBuilding();
            });
            
            spawnButton1.onClick.AddListener(delegate
            {
                ChooseBuilding(1);
            });
            spawnButton2.onClick.AddListener(delegate
            {
                ChooseBuilding(2);
            });
            spawnButton3.onClick.AddListener(delegate
            {
                ChooseBuilding(3);
            });
            
            prepareButton.onClick.AddListener(delegate
            {
                SwitchState(GameState.PREPARE);
            });


            gameState = GameState.LOOK;
            plane = new Plane(Vector3.up, Vector3.zero);
            cameraController.SetBounds(1, mapController.MapModel.Width - 1, mapController.MapModel.Height - 1, 1);
            cameraController.Init(plane, camera);
            //cameraController.transform.position = new Vector3(mapWidth / 2f, 0, mapHeight / 2f);
        }

        private void ChooseBuilding(int size)
        {
            SwitchState(GameState.BUILD);
            buildMenu.gameObject.SetActive(false);
            tmpSize = size;
        }

        private void Update()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                switch (gameState)
                {
                    case GameState.LOOK:

                        if (Input.GetMouseButtonDown(0))
                        {
                            cameraController.SetDragStartPosition();
                        }

                        if (Input.GetMouseButton(0))
                        {
                            cameraController.SetDragCurrentPosition();
                        }

                        cameraController.MoveCamera();
                        break;
                    case GameState.PREPARE:

                        if (Input.GetMouseButtonDown(0))
                        {
                            cameraController.SetDragStartPosition();
                            PrepareCell();
                        }

                        if (Input.GetMouseButton(0))
                        {
                            cameraController.SetDragCurrentPosition();
                        }

                        cameraController.MoveCamera();
                        break;
                    
                    case GameState.CHOOSE_BUILDING:
                        
                        if (Input.GetMouseButtonDown(0))
                        {
                            buildMenu.gameObject.SetActive(false);
                            SwitchState(GameState.LOOK);
                        }
                        break;

                    case GameState.BUILD:

                        if (Input.GetMouseButtonDown(0))
                        {
                            if (cameraController.IsRaycastSucceessful())
                            {
                                var pos = cameraController.GetRaycastPoint();
                                pos.x = Mathf.RoundToInt(pos.x);
                                pos.z = Mathf.RoundToInt(pos.z);
                                CreateNewBuilding(pos);
                            }
                        }

                        if (Input.GetMouseButton(0))
                        {
                            if (cameraController.IsRaycastSucceessful())
                            {
                                var pos = cameraController.GetRaycastPoint();
                                if (tmpBuilding != null)
                                {
                                    MoveNewBuilding(pos);
                                    SwitchBuildingEnablingVisual();
                                }
                            }
                        }

                        break;
                }
            }
        }

        private void PrepareCell()
        {
            if (cameraController.IsRaycastSucceessful())
            {
                var pos = cameraController.GetRaycastPoint();
                mapController.PrepareCell(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.z));
            }
        }

        private void FinishNewBuilding()
        { 
            SwitchState(GameState.LOOK);
            finishBuildingButton.gameObject.SetActive(false);
            mapController.PlaceBuilding(tmpBuilding);
            tmpBuilding.BuildingView.TurnBasic();
            tmpBuilding = null;
            tmpSize = 0;
        }

        private void CreateNewBuilding(Vector3 pos)
        {
            if (tmpBuilding == null)
            {
                tmpBuilding = mapController.CreateUnfixedBuilding(pos, tmpSize);
            }
        }

        private void MoveNewBuilding(Vector3 pos)
        {
            pos.x = Mathf.Round(pos.x);
            pos.z = Mathf.Round(pos.z);
            
            if (tmpBuilding.Size % 2 == 0)
            {
                pos.x += 0.5f;
                pos.z += 0.5f;
            }
            tmpBuilding.BuildingView.transform.position = pos;
        }

        private void SwitchBuildingEnablingVisual()
        {
            if (mapController.CanPlaceBuilding(tmpBuilding))
            {
                tmpBuilding.BuildingView.TurnGreen();
                finishBuildingButton.gameObject.SetActive(true);
            }
            else
            {
                tmpBuilding.BuildingView.TurnRed();
                finishBuildingButton.gameObject.SetActive(false);
            }
        }

        public void SwitchState(GameState state)
        {
            gameState = state;
            if (state == GameState.LOOK)
            {
                cameraController.IsDraggable = false;
            }
        }
    }
}