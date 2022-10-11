using UnityEngine;

namespace Game
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private GameObject visual;
        public CellModel CellModel { get; set; }

        public void ChangeVisual(GameObject visualPrefab)
        {
           Destroy(visual.gameObject);
           visual = Instantiate(visualPrefab, transform);
        }
        
    }
}