using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetect : MonoBehaviour
{
    private BrickProperties brickProperties;

    private void Start()
    {
        brickProperties = gameObject.transform.parent.GetComponent<BrickProperties>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == false) { return; }
        if (brickProperties.isPreRender == true) { return; }
        if (other.gameObject.transform.parent != null) 
        {
            if (other.gameObject.transform.parent.GetComponent<BrickProperties>() != null)
            {
                if (other.gameObject.transform.parent.GetComponent<BrickProperties>().isPreRender == true) { return; }
            }
        }

        if (brickProperties.isEditMode)
        {
            brickProperties.Triggered();
            print("trigger1: " + gameObject.transform.parent.name + " collides with " + other + " of " + other.gameObject.transform.parent);
        }
        else
        {
            brickProperties.isCollided = true;
            print("trigger2: " + gameObject.transform.parent.name + " collides with " + other + " of " + other.gameObject.transform.parent);
        }
    }
}
