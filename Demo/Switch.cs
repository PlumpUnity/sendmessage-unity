using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {

    // Use this for initialization
    void Start () {
#if UNITY_EDITOR
        gameObject.AddComponent<TestUnity1>();
#else
        gameObject.AddComponent<TestWindows1>();
#endif

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
