using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{

    private RectTransform crosshair;

    [SerializeField, Range(30, 150)] private float size;
    // Start is called before the first frame update
    void Start()
    {
        crosshair = GetComponent<RectTransform>();
      

    }

    // Update is called once per frame
    void Update()
    {
        crosshair.sizeDelta = new Vector2(size, size);
    }
}
