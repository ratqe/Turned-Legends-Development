using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    [Range(-1f, 1f)]
    public float movementSpeed = 0.5f;
    private float offset;
    private Material mat;


    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material; //gets the material
    }

    // Update is called once per frame
    void Update()
    {
        offset += (Time.deltaTime * movementSpeed) / 10f; //Movement speed
        mat.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
