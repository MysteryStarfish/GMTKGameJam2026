using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count == 0)
            {
                Debug.LogWarning("<color=red>[Raycast] 畫面上此處沒有命中任何 UI！</color>");
            }
            else
            {
                foreach (var result in results)
                {
                    Debug.Log($"<color=green>[Raycast 命中]</color>: {result.gameObject.name} (層級深度: {result.depth})");
                }
            }
        }
    }
}