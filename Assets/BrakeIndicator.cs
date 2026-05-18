using UnityEngine;

namespace Root
{
    public class BrakeIndicator : MonoBehaviour
    {
        [SerializeField] private TrainBrakeController brakeController;
        [SerializeField] private Transform needle;

        [SerializeField] private float minY = 1f;   
        [SerializeField] private float maxY = -1f; 
        [SerializeField] private float moveSpeed = 1f; 

        private void Update()
        {
            int level = brakeController.GetBrakeLevel();
            int maxLevel = brakeController.brakeLevels.Count - 1;

            float t = maxLevel > 0 ? (float)level / maxLevel : 0;
            float targetY = Mathf.Lerp(minY, maxY, t);

            Vector3 pos = needle.localPosition;
            pos.y = Mathf.MoveTowards(pos.y, targetY, moveSpeed * Time.deltaTime);
            needle.localPosition = pos;
        }
    }
}