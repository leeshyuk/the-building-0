using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public float rayDistance = 2f;
    public LayerMask layerMask = 1;

    public GameObject targetObject;

    private void Update()
    {
        Ray ray = new(transform.position, transform.forward);

        Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, layerMask))
        {
            Transform parent = hit.transform;
            while (parent.parent && !parent.CompareTag("Interactable Object")) parent = parent.parent;
            targetObject = parent.gameObject;
        }
        else
        {
            targetObject = null;
        }

        if (targetObject && targetObject.CompareTag("Interactable Object"))
        {
            transform.GetComponentInChildren<UnityEngine.UI.Image>().color = Color.red;

            if (Input.GetMouseButtonDown(0))
            {
                if (targetObject.GetComponent<LRD1Controller>()) targetObject.GetComponent<LRD1Controller>().ExecuteInteraction();
                else if (targetObject.GetComponent<LRD2Controller>()) targetObject.GetComponent<LRD2Controller>().ExecuteInteraction();
            }
        }
        else
        {
            transform.GetComponentInChildren<UnityEngine.UI.Image>().color = Color.white;

        }
    }
}
