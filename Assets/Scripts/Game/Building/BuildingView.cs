using System;
using UnityEngine;

namespace Game
{
    public class BuildingView : MonoBehaviour
    {
        [SerializeField] private Material red;
        [SerializeField] private Material green;
        [SerializeField] private Material basic;
        [SerializeField] private MeshRenderer meshRenderer;
        private BuildingModel model;
        public BuildingModel Model {
            get => model;
            set
            {
                model = value;
                ChangeVisualSize();
            }
        }

        private Material basicMaterial;
       

        private void ChangeVisualSize()
        {
            var size = Model.Size;
            var t = transform.position;
            transform.transform.localScale = new Vector3(size, size, size);
            transform.position = t;
        }

        public void TurnRed()
        {
            meshRenderer.material = red;
        }
        public void TurnGreen()
        {
            meshRenderer.material = green;
        }
        
        public void TurnBasic()
        {
            
            meshRenderer.material = basic;
        }
    }
}