using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using MessageTrans;
public class TestUnity1 : MonoBehaviour {
    IntPtr target;
    DataSender sender;
	// Update is called once per frame
	void Update () {
        if (target == IntPtr.Zero)
        {
            target = DataUtility.FindWindow("UnityWndClass", null);
            if (target != IntPtr.Zero)
            {
                sender = new DataSender();
                sender.RegistHandle(target);
            }
        }
        else
        {
            sender.SendMessage("add","hellow world");
        }
	}
}
