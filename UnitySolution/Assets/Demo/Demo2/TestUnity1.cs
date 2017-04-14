using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using MessageTrans;
using MessageTrans.Interal;
using Newtonsoft.Json;
public class Trs
{
    public int a;
    public string b;
}
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
            Trs trs = new global::Trs();
            trs.a = 1;
            trs.b = "2";
            sender.SendMessage("add", JsonConvert.SerializeObject(trs));
        }
	}
}
