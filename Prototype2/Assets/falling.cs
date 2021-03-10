using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class falling : MonoBehaviour
{
    public Color altColor;
    public Renderer rend;
    void Example()
    {
        altColor.g = 0f;
        altColor.r = 0f;
        altColor.b = 0f;
        altColor.a = 0f;
    }

    void Start()
    {
        //Get the renderer of the object so we can access the color
        rend = GetComponent<Renderer>();
    }
    void OnCollisionEnter(Collision collidedWithThis)
    {
        if (collidedWithThis.collider.gameObject.CompareTag("Player") && GetComponent<Rigidbody>().isKinematic == true)
        {
            StartCoroutine(FallAfterDelay());
        }
    }

    IEnumerator FallAfterDelay()
    {
        altColor.g = 255;
        rend.material.color = altColor;
        yield return new WaitForSeconds(1);
        altColor.r = 255;
        rend.material.color = altColor;
        yield return new WaitForSeconds(1);
        altColor.g = 0;
        rend.material.color = altColor;
        yield return new WaitForSeconds(1);
        Example();
        rend.material.color = altColor;

        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        

    }


 

    
    

  
}
