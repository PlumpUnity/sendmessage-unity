using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using MessageTrans;
public class TestUnity1 : MonoBehaviour {
    IntPtr target;
	// Update is called once per frame
	void Update () {
        if (target ==IntPtr.Zero)
        {
            target = DataUtility.FindWindow("UnityWndClass", null);
        }
        else
        {
            DataUtility.SendString(target, "你好啊");
        }
	}
}
