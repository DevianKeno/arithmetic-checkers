using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFinder : MonoBehaviour
{
    public Vector2 cell;

    public Vector2 mousePosition;

    RectTransform rectTransform;

    Grid grid;
    public Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid();
        rectTransform = GetComponent<RectTransform>();
        pos = rectTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Input.mousePosition;
    }
}
