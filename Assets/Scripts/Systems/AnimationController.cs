using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public RectTransform m_RectTransform;
    public Vector2 xy = new Vector2(0, 800);

    // Start is called before the first frame update
    void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_RectTransform.sizeDelta = xy;
    }

    void OnGUI()
    {
        if (GUI.changed)
        {
            //Change the RectTransform's anchored positions depending on the Slider values
            m_RectTransform.sizeDelta = xy;
        }
    }
    // Update is called once per framem_SizeDelta.x
    void Update()
    {
        
    }
}
