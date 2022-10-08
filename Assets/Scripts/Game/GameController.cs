using System;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private MapController mapController;
        [SerializeField] private CameraController cameraController;

        private void Awake()
        {
            cameraController.SetBounds(1,mapController.Width-1,mapController.Height-1,1);
            //cameraController.transform.position = new Vector3(mapWidth / 2f, 0, mapHeight / 2f);
        }
    }
}