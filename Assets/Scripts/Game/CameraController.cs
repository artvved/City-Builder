using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    private Camera camera;

    private Vector3 newTargetPosition;
    public bool IsDraggable { get; set; }

    private Vector3 dragStartPos;
    private Vector3 dragCurrentPos;

    private Plane plane;

    private float rBound;
    private float lBound;
    private float downBound;
    private float upBound;

    public void SetBounds(float l,float r,float up,float down)
    {
        rBound = r;
        lBound = l;
        upBound = up;
        downBound = down;
    }

    public void Init(Plane plane,Camera camera)
    {
        this.plane = plane;
        this.camera = camera;
        ResetTarget();
    }
    

    public void ResetTarget()
    {
        IsDraggable = false;
        newTargetPosition = transform.position;
    }

    public bool IsRaycastSucceessful()
    {
        float entry;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        return plane.Raycast(ray, out entry);
    }
    
    public Vector3 GetRaycastPoint()
    {
        float entry;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if ( plane.Raycast(ray, out entry))
        {
            return ray.GetPoint(entry);
        }
        return Vector3.zero;
    }

    public void SetDragStartPosition()
    {
        if (IsRaycastSucceessful())
        {
            IsDraggable = true;     
            dragStartPos = GetRaycastPoint();
           
        }
    }
    
    public void SetDragCurrentPosition()
    {
        if (IsRaycastSucceessful())
        {
            dragCurrentPos = GetRaycastPoint();
            newTargetPosition = transform.position + dragStartPos - dragCurrentPos;
                
        }
    }

    

    public void MoveCamera()
    {
        if (IsDraggable)
        {
            var newPos=Vector3.MoveTowards(transform.position,newTargetPosition,Time.deltaTime*movementSpeed);
            if (!IsOutOfBounds(newPos))
            {
                transform.position = newPos;
            }
        }
    }


    private bool IsOutOfBounds(Vector3 newPos)
    {
        return (newPos.x > rBound || newPos.x < lBound || newPos.z > upBound || newPos.z < downBound);
    }
}