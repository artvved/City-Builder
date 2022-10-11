using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private MapController mapController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private Camera camera;

        [Header("UI: Build")] [SerializeField] private Button buildButton;
        [SerializeField] private GameObject buildMenu;
        [SerializeField] private Button spawnButton1;
        [SerializeField] private Button spawnButton2;
        [SerializeField] private Button spawnButton3;
        [SerializeField] private Button finishBuildingButton;
        [Header("UI: Prepare")] [SerializeField] private Button prepareButton;
        


        private GameState gameState;

        private BuildingView tmpBuilding;
        private int tmpSize;


        private Plane plane;

        private void Start()
        {
            mapController.CreateMap();
        }

        private void Awake()
        {
            buildButton.onClick.AddListener(delegate
            {
                SwitchState(GameState.CHOOSE_BUILDING);
                buildMenu.gameObject.SetActive(true);
                if (tmpBuilding!=null)
                {
                    Destroy(tmpBuilding.gameObject);
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
            cameraController.SetBounds(1, mapController.Width - 1, mapController.Height - 1, 1);
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
            var pos = cameraController.GetRaycastPoint();
            Debug.DrawLine(new Vector3( pos.x, 0,  pos.z), new Vector3( pos.x, 5,  pos.z),
                Color.blue, 10000);
            mapController.PrepareCell(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.z));
            
        }

        private void FinishNewBuilding()
        { 
            SwitchState(GameState.LOOK);
            finishBuildingButton.gameObject.SetActive(false);
            mapController.PlaceBuilding(tmpBuilding);
            tmpBuilding.TurnBasic();
            tmpBuilding = null;
            tmpSize = 0;
        }

        private void CreateNewBuilding(Vector3 pos)
        {
            if (tmpBuilding == null)
            {
                tmpBuilding = mapController.CreateBuilding(pos, tmpSize);
            }
        }

        private void MoveNewBuilding(Vector3 pos)
        {
            pos.x = Mathf.Round(pos.x);
            pos.z = Mathf.Round(pos.z);
            
            if (tmpBuilding.Model.Size % 2 == 0)
            {
                pos.x += 0.5f;
                pos.z += 0.5f;
            }
            tmpBuilding.transform.position = pos;
        }

        private void SwitchBuildingEnablingVisual()
        {
            if (mapController.CanPlaceBuilding(tmpBuilding))
            {
                tmpBuilding.TurnGreen();
                finishBuildingButton.gameObject.SetActive(true);
            }
            else
            {
                tmpBuilding.TurnRed();
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