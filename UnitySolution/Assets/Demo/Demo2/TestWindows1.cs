using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessageTrans;
public class TestWindows1 : MonoBehaviour {
    IDataReceiver receive;
    public UnityEngine.UI.Text text;
	// Use this for initialization
	void Start () {
        receive = new DataReceiver();
        receive.RegistHook();
        //EventHolder.RegisterEvent("add", OnReceive);
        text = FindObjectOfType<UnityEngine.UI.Text>();

    }
	
	// Update is called once per frame
	void OnReceive (string body) {
        Debug.Log(body);
        text.text += body;
        if (text.text.Length > 3000)
        {
            text.text = "";
        }

    }
    private void OnDestroy()
    {
        receive.RemoveHook();
    }
}
