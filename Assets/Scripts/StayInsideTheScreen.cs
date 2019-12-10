using System;
using UnityEngine;

public class StayInsideTheScreen : MonoBehaviour 
{
    private float tolerance = 0.001f;
    
    // Update is called once per frame

    private void LateUpdate(){
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, Camera.main.transform.position.x - 2.25f, Camera.main.transform.position.x + 2.25f), 
                                        Mathf.Clamp(transform.position.y, Camera.main.transform.position.y - 4.75f, Camera.main.transform.position.y + 4.75f));
        if (!CompareTag("Enemy") && !CompareTag("Heavy")) return;
        if (ObjectIsOnVerticalCorners(transform.position))
            transform.up = Vector3.Reflect(transform.up, Vector3.down).normalized;
        if (ObjectIsOnHorizontalCorners(transform.position))
            transform.up = Vector3.Reflect(transform.up, Vector3.left).normalized;
    }

    private bool ObjectIsOnHorizontalCorners(Vector3 transformPosition)
    {
        return (Math.Abs(transformPosition.x - (Camera.main.transform.position.x - 2.25f)) < tolerance || Math.Abs(transformPosition.x - (Camera.main.transform.position.x + 2.25f)) < tolerance);
    }

    private bool ObjectIsOnVerticalCorners(Vector3 transformPosition)
    {
        return (Math.Abs(transformPosition.y - (Camera.main.transform.position.y - 4.75f)) < tolerance || Math.Abs(transformPosition.y - (Camera.main.transform.position.y + 4.75f)) < tolerance);
    }
}