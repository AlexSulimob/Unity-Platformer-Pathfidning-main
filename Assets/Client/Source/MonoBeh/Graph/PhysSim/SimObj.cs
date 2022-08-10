using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimObj : MonoBehaviour
{
    public Transform GroundCheck;
    public LayerMask layerMask;
    public float groundCheckDistance;
    [HideInInspector]public Rigidbody2D rb;
    ContactPoint2D[] contactPoints;

    [HideInInspector]public bool isTopContact;
    [HideInInspector]public bool isBottomContact;
    [HideInInspector]public bool isLeftContact;
    [HideInInspector]public bool isRightContact;
    [HideInInspector]public bool isBottomRayContact;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        contactPoints = new ContactPoint2D[10];
    }
    public void CheckCollider()
    {
        int hits = rb.GetContacts(contactPoints); 
        isBottomContact = isTopContact = isLeftContact = isRightContact = false;
        for (int i = 0; i < hits; i++)
        {

            var normal = contactPoints[i].normal;
            if (normal.y > 0.1f)
            {
                isBottomContact = true; 
            }
            if(normal.y < -0.1f)
            {
                isTopContact = true; 
            }
            if (normal.x > 0.1f)
            {
                isLeftContact = true; 

            }
            if(normal.x < -0.1f)
            {
                isRightContact = true; 
            }
        }
        isBottomRayContact = Physics2D.Raycast(GroundCheck.position, Vector2.down, groundCheckDistance, layerMask);
    }
    private void OnDrawGizmos() {
        Gizmos.DrawRay(GroundCheck.position, Vector3.down * groundCheckDistance);   
    }
    
}
