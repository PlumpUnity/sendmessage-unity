using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch1 : MonoBehaviour {

    // Use this for initialization
    void Start () {
#if UNITY_EDITOR
        gameObject.AddComponent<TestUnity>();
#else
        gameObject.AddComponent<TestWindows>();
#endif

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
