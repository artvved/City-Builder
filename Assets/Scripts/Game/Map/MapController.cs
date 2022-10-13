using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Game
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private CellView cellViewPrefab;
        [SerializeField] private GameObject grassPrefab;
        [SerializeField] private GameObject sandPrefab;
        [SerializeField] private GameObject waterPrefab;
        [SerializeField] private GameObject swampPrefab;
        [SerializeField] private BuildingView buildingPrefab;

        [SerializeField] private int width;
        [SerializeField] private int height;
        [Range(0f, 1f)] [SerializeField] private float swampChance;
        [Range(0f, 1f)] [SerializeField] private float waterChance;
        [Range(0f, 1f)] [SerializeField] private float buildingChance;
        [Range(0f, 1f)] [SerializeField] private float sandChance;


        private MapModel mapModel;

        public MapModel MapModel => mapModel;

        public void CreateMap(MapModel model)
        {
            DestroyCurrentMap();
            mapModel = model;
            InstantiateCells();
            InstantiateBuildings();
        }

        public void CreateMap()
        {
            mapModel = new MapModel(width, height);

            int count = width * height;
            var pairs = new List<(int, int)>();

            int waterCellsNumber = (int) (count * waterChance);
            int swampCellsNumber = (int) (count * swampChance);
            int sandCellsNumber = (int) (count * sandChance);
            int buildingCellsNumber = (int) (count * buildingChance);


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var cell = new CellModel(i, j, CellState.GRASS);
                    mapModel.Map[i, j] = cell;
                    pairs.Add((i, j));
                }
            }


            GenerateStateToRandomCellsFromList(waterCellsNumber, CellState.WATER, ref pairs); //generate random
            GenerateStateToRandomCellsFromList(swampCellsNumber, CellState.SWAMP, ref pairs);
            GenerateStateToRandomCellsFromList(sandCellsNumber, CellState.SAND, ref pairs);

            InstantiateCells();

            GenerateAndInstantiateBuildingsToRandomCellsFromList(buildingCellsNumber, ref pairs);
        }

        private void InstantiateCells()
        {
            foreach (var cellModel in mapModel.Map)
            {
                var view = Instantiate(cellViewPrefab,
                    new Vector3(cellModel.X, 0, cellModel.Y),
                    Quaternion.identity,
                    transform);
                cellModel.CellView = view;
                view.ChangeVisual(ChooseCellVisualByState(cellModel.CellState));
            }
        }

        private void InstantiateBuildings()
        {
            HashSet<BuildingModel> set = new HashSet<BuildingModel>();
            foreach (var cellModel in mapModel.Map)
            {
                var buildingModel = cellModel.BuildingModel;
                if (buildingModel != null && !set.Contains(buildingModel))
                {
                    set.Add(buildingModel);
                    CreateUnfixedBuilding(new Vector3(buildingModel.X, 0, buildingModel.Y), buildingModel);
                    PlaceBuilding(buildingModel, false);
                }
            }
        }

        private void GenerateAndInstantiateBuildingsToRandomCellsFromList(int number, ref List<(int, int)> pairs)
        {
            for (int i = 0; i < number; i++)
            {
                var rnd = Random.Range(0, pairs.Count);
                (int x, int y) = pairs[rnd];
                var b = CreateUnfixedBuilding(mapModel.Map[x, y].CellView.transform.position, 1);
                PlaceBuilding(b);
                pairs.Remove((x, y));
            }
        }


        public BuildingModel CreateUnfixedBuilding(Vector3 pos, int size)
        {
            var view = Instantiate(buildingPrefab, pos, Quaternion.identity, transform);
            var model = new BuildingModel(size);
            model.BuildingView = view;
            view.ChangeVisualSize(model.Size);
            return model;
        }

        public void CreateUnfixedBuilding(Vector3 pos, BuildingModel model)
        {
            var view = Instantiate(buildingPrefab, pos, Quaternion.identity, transform);
            view.ChangeVisualSize(model.Size);
            model.BuildingView = view;
        }

        public void PlaceBuilding(BuildingModel buildingModel)
        {
            PlaceBuilding(buildingModel, true);
        }

        private void PlaceBuilding(BuildingModel buildingModel, bool check)
        {
            if (check && !CanPlaceBuilding(buildingModel))
            {
                return;
            }

            Vector3 pos = buildingModel.BuildingView.transform.position;
            int buildingSize = buildingModel.Size;

            int half = Mathf.FloorToInt(buildingSize / 2f);

            int centerX;
            int centerY;
            if (buildingSize % 2 == 0)
            {
                centerX = (int) (pos.x - 0.5f);
                centerY = (int) (pos.z + 0.5f);
            }
            else
            {
                centerX = (int) pos.x;
                centerY = (int) pos.z;
            }

            int len = buildingSize;
            int x = centerX + half;
            int y = centerY - half;

            int dx = 0;
            int dy = 1;

            int lenI = 0;


            for (int i = 0; i < (buildingSize * buildingSize); i++)
            {
                mapModel.Map[x, y].BuildingModel = buildingModel;
                buildingModel.SaveVisualPosition();
                lenI++;
                if (lenI == len)
                {
                    lenI = 0;

                    switch (dy)
                    {
                        case > 0:
                            dx = -1;
                            dy = 0;
                            len--;
                            break;
                        case < 0:
                            dx = 1;
                            dy = 0;
                            len--;
                            break;
                        case 0:
                        {
                            if (dx > 0)
                            {
                                dy = 1;
                                dx = 0;
                            }
                            else if (dx < 0)
                            {
                                dy = -1;
                                dx = 0;
                            }

                            break;
                        }
                    }
                }

                y += dy;
                x += dx;
            }
        }

        public bool CanPlaceBuilding(BuildingModel buildingModel)
        {
            Vector3 pos = buildingModel.BuildingView.transform.position;
            int buildingSize = buildingModel.Size;

            int half = Mathf.FloorToInt(buildingSize / 2f);

            int centerX;
            int centerY;
            if (buildingSize % 2 == 0)
            {
                centerX = (int) (pos.x - 0.5f);
                centerY = (int) (pos.z + 0.5f);
            }
            else
            {
                centerX = (int) pos.x;
                centerY = (int) pos.z;
            }

            //bounds check
            if (buildingSize % 2 == 0)
            {
                if (centerX + 1 - half < 0 || centerY - half < 0 || centerX + 1 + half > mapModel.Width ||
                    centerY + half > mapModel.Height)
                {
                    return false;
                }
            }
            else
            {
                if (centerX - half < 0 || centerY - half < 0 || centerX + half >= mapModel.Width ||
                    centerY + half >= mapModel.Height)
                {
                    return false;
                }
            }


            int len = buildingSize;
            int x = centerX + half;
            int y = centerY - half;

            int dx = 0;
            int dy = 1;

            int lenI = 0;


            for (int i = 0; i < (buildingSize * buildingSize); i++)
            {
                //print(x+ "-x y-" +y+" "+ map[x,y].CellState);
                if (!IsCellEmpty(x, y))
                {
                    return false;
                }

                lenI++;
                if (lenI == len)
                {
                    lenI = 0;

                    switch (dy)
                    {
                        case > 0:
                            dx = -1;
                            dy = 0;
                            len--;
                            break;
                        case < 0:
                            dx = 1;
                            dy = 0;
                            len--;
                            break;
                        case 0:
                        {
                            if (dx > 0)
                            {
                                dy = 1;
                                dx = 0;
                            }
                            else if (dx < 0)
                            {
                                dy = -1;
                                dx = 0;
                            }

                            break;
                        }
                    }
                }

                y += dy;
                x += dx;
            }

            return true;
        }


        private bool IsCellEmpty(int x, int y)
        {
            return mapModel.Map[x, y].BuildingModel == null &&
                   mapModel.Map[x, y].CellState != CellState.WATER &&
                   mapModel.Map[x, y].CellState != CellState.SWAMP;
        }

        private GameObject ChooseCellVisualByState(CellState state)
        {
            switch (state)
            {
                case CellState.GRASS:
                    return grassPrefab;
                case CellState.SAND:
                    return sandPrefab;
                case CellState.WATER:
                    return waterPrefab;
                case CellState.SWAMP:
                    return swampPrefab;
            }

            return null;
        }

        private void GenerateStateToRandomCellsFromList(int number, CellState cellState, ref List<(int, int)> pairs)
        {
            for (int i = 0; i < number; i++)
            {
                var rnd = Random.Range(0, pairs.Count);
                (int x, int y) = pairs[rnd];
                mapModel.Map[x, y].CellState = cellState;
                pairs.Remove((x, y));
            }
        }


        public void PrepareCell(int x, int y)
        {
            var cell = mapModel.Map[x, y];
            CellState newState;
            switch (cell.CellState)
            {
                case CellState.WATER:
                    newState = CellState.SWAMP;
                    break;
                case CellState.SWAMP:
                    newState = CellState.SAND;
                    break;
                default:
                    return;
            }

            cell.CellState = newState;
            cell.CellView.ChangeVisual(ChooseCellVisualByState(newState));
        }

        public void DestroyCurrentMap()
        {
            var cells = GetComponentsInChildren<CellView>();
            var buildingViews = GetComponentsInChildren<BuildingView>();

            for (int i = 0; i < cells.Length; i++)
            {
                Destroy(cells[i].gameObject);
            }

            for (int i = 0; i < buildingViews.Length; i++)
            {
                Destroy(buildingViews[i].gameObject);
            }
        }
    }
}