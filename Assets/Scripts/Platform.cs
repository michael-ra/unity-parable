using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject lava;
    float lavaZPos;

    // Start is called before the first frame update
    void Start(){}

    public void StoreLavaZPos()
    {
        // Store z position to reset it after randomizing the 
        // position of the platform
        lavaZPos = lava.transform.position.z;
    }

    public void ResetLavaZPos()
    {
        var lavaPosition = lava.transform.position;
        lava.transform.position = new Vector3(
            lavaPosition.x, lavaPosition.y, lavaZPos);
    }
}
