using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistent : MonoBehaviour
{
    static Persistent _instance;

    void Awake()
    {
        if (_instance != null && _instance == this)
        {
            Destroy(this);
        } else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }   
    }
}
