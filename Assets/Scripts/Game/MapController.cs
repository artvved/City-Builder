using System;
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
        [SerializeField] private GameObject buildingPrefab;
        
        [SerializeField] private int width;
        [SerializeField] private int height;
        [Range( 0f,  1f)]
        [SerializeField] private float swampChance;
        [Range( 0f,  1f)]
        [SerializeField] private float waterChance;
        [Range( 0f,  1f)]
        [SerializeField] private float buildingChance;
        [Range( 0f,  1f)]
        [SerializeField] private float sandChance;


        public int Width => width;

        public int Height => height;

        private CellModel[,] map;

        void Start()
        {
            CreateMap();
        }

        private void CreateMap()
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

            
            GenerateStateToRandomCellsFromList(waterCellsNumber,CellState.WATER, ref pairs);//generate random
            GenerateStateToRandomCellsFromList(swampCellsNumber,CellState.SWAMP, ref pairs);
            GenerateStateToRandomCellsFromList(sandCellsNumber,CellState.SAND, ref pairs);
            GenerateStateToRandomCellsFromList(buildingCellsNumber,CellState.BUILDING, ref pairs);
            //gen build

            foreach (var cellModel in map)
            {
                var view = Instantiate(cellViewPrefab,
                    new Vector3(cellModel.X, 0,cellModel.Y),
                    Quaternion.identity,
                    transform);
                view.CellModel = cellModel;
                if (cellModel.CellState==CellState.BUILDING)
                {
                    view.CreateBuilding(buildingPrefab);
                }else
                    view.ChangeVisual(ChooseCellVisualByState(cellModel.CellState));
                //sign for events
            }
            
            
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