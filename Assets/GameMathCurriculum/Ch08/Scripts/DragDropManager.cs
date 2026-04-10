using System;
using NUnit.Framework;
using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    public Camera camera;

    public LayerMask ground;
    public LayerMask dragObject;
    public LayerMask dropZone;
    private DragObject draggingObject;



    private bool isDragging = false;
    
    public void Update()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, dragObject))
            {

                isDragging = true;
                draggingObject = hitInfo.collider.GetComponent<DragObject>();
                draggingObject.DragStart();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, dropZone))
                {
                    draggingObject.DragEnd();
                }
                else
                {
                    draggingObject.Return();
                }
                isDragging = false;   
                draggingObject = null; 
                 
            }
        }

        else if (isDragging)
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ground))
            {
                draggingObject.transform.position = hitInfo.point;
            }
        }
    }
}
