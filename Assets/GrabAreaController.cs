using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrabAreaController : MonoBehaviour
{
    private SphereCollider area;

    private void Awake()
    {
        area = GetComponent<SphereCollider>();
    }

    public bool canGrab
    {
        get
        {
            return items.Length > 0;
        }
    }

    public Collider[] items
    {
        get
        {
            return Physics.OverlapSphere(transform.position, area.radius, 1 << LayerMask.NameToLayer("Grabbable"));
        }
    }

    public GameObject closest
    {
        get
        {
            float minDist = Mathf.Infinity;
            Collider closest = new Collider();

            foreach (Collider c in items)
            {
                float dist = Vector3.Distance(transform.position, c.transform.position);

                if (dist < minDist)
                {
                    minDist = dist;
                    closest = c;
                }
            }
            return closest.gameObject;
        }
    }
}
