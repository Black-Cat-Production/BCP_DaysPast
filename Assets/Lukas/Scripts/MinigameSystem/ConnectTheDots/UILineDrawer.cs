using System.Collections.Generic;
using UnityEngine;

namespace Scripts.MinigameSystem.ConnectTheDots
{
    public class UILineDrawer : MonoBehaviour
    {
        [SerializeField] RectTransform groupingTransform;
        [SerializeField] GameObject linePrefab;
        [SerializeField] List<GameObject> lineObjects;

        public void DrawLine(Vector2 _from, Vector2 _to)
        {
            
            Debug.Log("Start: " + _from + ", To: " + _to);
            var lineObject = Instantiate(linePrefab, groupingTransform);
            lineObjects.Add(lineObject);
            var rectTransform = lineObject.GetComponent<RectTransform>();

            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            
            var direction = (_to - _from);
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            rectTransform.sizeDelta = new Vector2(distance, 5f);
            rectTransform.pivot = new Vector2(0, 0.5f);
            rectTransform.anchoredPosition = _from;
            rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        public Vector2 GetLocalCanvasPosition(RectTransform _target)
        {
            var screenPoint = RectTransformUtility.WorldToScreenPoint(null, _target.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(groupingTransform, screenPoint, null, out var localPoint);
            return localPoint;
        }

        public void ClearLinesFromScene()
        {
            foreach (var lineObject in lineObjects)
            {
                Destroy(lineObject);
            }
            lineObjects.Clear();
        }
    }
}