using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistent : MonoBehaviour
{
    static Persistent _instance;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        } else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }   
    }
}
