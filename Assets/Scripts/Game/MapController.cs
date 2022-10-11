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


        public int Width => width;

        public int Height => height;

        private CellModel[,] map;


        public void CreateMap()
        {
            map = new CellModel[width, height];
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
                    map[i, j] = cell;
                    pairs.Add((i, j));
                }
            }


            GenerateStateToRandomCellsFromList(waterCellsNumber, CellState.WATER, ref pairs); //generate random
            GenerateStateToRandomCellsFromList(swampCellsNumber, CellState.SWAMP, ref pairs);
            GenerateStateToRandomCellsFromList(sandCellsNumber, CellState.SAND, ref pairs);
            GenerateBuildingsToRandomCellsFromList(buildingCellsNumber, ref pairs);

            foreach (var cellModel in map)
            {
                var view = Instantiate(cellViewPrefab,
                    new Vector3(cellModel.X, 0, cellModel.Y),
                    Quaternion.identity,
                    transform);
                view.CellModel = cellModel; //?
                if (cellModel.BuildingModel != null)
                {
                    CreateBuilding(view.transform.position, cellModel.BuildingModel);
                }
                else
                    view.ChangeVisual(ChooseCellVisualByState(cellModel.CellState));
            }
        }

        private void GenerateBuildingsToRandomCellsFromList(int number, ref List<(int, int)> pairs)
        {
            for (int i = 0; i < number; i++)
            {
                var rnd = Random.Range(0, pairs.Count);
                (int x, int y) = pairs[rnd];
                map[x, y].BuildingModel = new BuildingModel(1);
                pairs.Remove((x, y));
            }
        }

        public BuildingView CreateBuilding(Vector3 pos, int size)
        {
            var view = Instantiate(buildingPrefab, pos, Quaternion.identity, transform);
            var model = new BuildingModel(size);
            view.Model = model;
            return view;
        }

        public BuildingView CreateBuilding(Vector3 pos, BuildingModel model)
        {
            var view = Instantiate(buildingPrefab, pos, Quaternion.identity, transform);
            view.Model = model;
            return view;
        }

        public void PlaceBuilding(BuildingView buildingView)
        {
            if (!CanPlaceBuilding(buildingView))
            {
                return ;
            }

            Vector3 pos = buildingView.transform.position;
            int buildingSize = buildingView.Model.Size;

            //bounds check
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
                
                    map[x, y].BuildingModel = buildingView.Model;
                

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

        public bool CanPlaceBuilding(BuildingView buildingView)
        {
            Vector3 pos = buildingView.transform.position;
            int buildingSize = buildingView.Model.Size;
            
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
                if (centerX+1 - half < 0 || centerY - half < 0 || centerX+1 + half > width || centerY + half > height)
                {
                    return false;
                }
            }
            else
            {
                if (centerX - half < 0 || centerY - half < 0 || centerX + half >= width || centerY + half >= height)
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
            return map[x, y].BuildingModel == null &&
                   map[x, y].CellState != CellState.WATER &&
                   map[x, y].CellState != CellState.SWAMP;
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
                map[x, y].CellState = cellState;
                pairs.Remove((x, y));
            }
        }
    }
}