using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyGo : MonoBehaviour
{

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
