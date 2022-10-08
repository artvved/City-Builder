using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private float movementSpeed;
    
    private Vector3 newTargetPosition;
    
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

    private void Start()
    {
        plane = new Plane(Vector3.up, Vector3.zero);
        newTargetPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            float entry;
          
            if (plane.Raycast(ray,out entry))
            { 
               
                dragStartPos = ray.GetPoint(entry);
            }
        }
        
        if (Input.GetMouseButton(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            float entry;
            if (plane.Raycast(ray,out entry))
            {
                dragCurrentPos = ray.GetPoint(entry);
                newTargetPosition = transform.position + dragStartPos - dragCurrentPos;
                
            }
        }

        var newPos=Vector3.MoveTowards(transform.position,newTargetPosition,Time.deltaTime*movementSpeed);
        
        
        if (!IsOutOfBounds(newPos))
        {
            transform.position = newPos;
        }

    }


    private bool IsOutOfBounds(Vector3 newPos)
    {
        return (newPos.x > rBound || newPos.x < lBound || newPos.z > upBound || newPos.z < downBound);
    }
}