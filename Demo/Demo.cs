using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System;
using MessageTrans;

public class Demo : MonoBehaviour {
    public Text parentT;
    public Text childT;
    public Text messageT;
    public InputField parentI;
    public InputField childI;
    public Button openChild;
    public Button closeChild;

    IWindowSwitchCtrl windowswitch;
    string[] parmars;
    IntPtr parentHandle { get { return windowswitch.Parent; } }
    IntPtr childHandle { get { return windowswitch.Child; } }

    DataReceiver receiver;
    DataSender childSender;
    DataSender parentSender;
    private void Awake()
    {
        InitMessage();

        windowswitch = new WindowSwitchCtrl();
        if(windowswitch.OnOpenedByParent(out parmars)) {
            //以外部程序的方式打开
            parentT.text = parentHandle.ToString();
            parentSender.RegistHandle(parentHandle);
        }
        openChild.onClick.AddListener(TryOpenChild);
        closeChild.onClick.AddListener(TryCloseChild);
        parentI.onEndEdit.AddListener(SendInfoToParent);
        childI.onEndEdit.AddListener(SendInfoToChild);
    }
    private void InitMessage()
    {
        receiver = new MessageTrans.DataReceiver();
        receiver.RegistHook();

        childSender = new MessageTrans.DataSender();
        parentSender = new MessageTrans.DataSender();
        receiver.RegisterEvent("simpleText", OnReceive);
    }
    void TryOpenChild()
    {
        string path = FileDialog.OpenFileDialog(AssetFileType.Exe, "SelectExe", Application.dataPath);
        if (!string.IsNullOrEmpty(path)){
            if(windowswitch.OpenChildWindow(path, false))
            {
                //打开子应用程序
                StartCoroutine(DelyRegisterSender());
            }
        }
    }
    void TryCloseChild()
    {
        if (windowswitch.CloseChildWindow())
        {
            childT.text = null;
        }
    }
    void SendInfoToChild (string text) {
        if (!string.IsNullOrEmpty(text))
        {
            childSender.SendMessage("simpleText", text);
        }
	}
    void SendInfoToParent(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            parentSender.SendMessage("simpleText", text);
        }
    }

    IEnumerator DelyRegisterSender()
    {
        while(childHandle == IntPtr.Zero)
        {
            yield return null;
        }
        childSender.RegistHandle(childHandle);
        childT.text = childHandle.ToString();
    }

    void OnReceive(string receivedText)
    {
        messageT.text += receivedText + "\n";
    }

    private void OnDestroy()
    {
        receiver.RemoveHook();
        windowswitch.OnCloseThisWindow();
    }
}
