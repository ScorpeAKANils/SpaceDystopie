using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dematerilize : MonoBehaviour
{
    bool changeLayer = true;
    int layer = 9;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (changeLayer)
        {
            this.gameObject.layer = 9;
        }
        StartCoroutine(wait());

    }

    IEnumerator wait()
    {
        changeLayer = false;
        yield return new WaitForSeconds(1);
        if (layer == 6)
        {
            layer = 9;
        }
        else { layer = 6; }
        changeLayer = true;

    }
}
