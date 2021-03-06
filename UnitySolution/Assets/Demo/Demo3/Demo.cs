﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System;
using MessageTrans;
using FormSwitch;
using FormSwitch.Internal;
public class Demo : MonoBehaviour {
    public Text parentT;
    public Text childT;
    public Text messageT;
    public Text thisT;
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

        windowswitch = new WindowSwitchUnity();
        if(windowswitch.OnOpenedByParent(ref parmars)) {
            //以外部程序的方式打开
            parentT.text = parentHandle.ToString();
            parentSender.RegistHandle(parentHandle);
        }
        openChild.onClick.AddListener(TryOpenChild);
        closeChild.onClick.AddListener(TryCloseChild);
        parentI.onEndEdit.AddListener(SendInfoToParent);
        childI.onEndEdit.AddListener(SendInfoToChild);
    }

    void Start()
    {
        thisT.text = windowswitch.Current.ToString();
        receiver.MessageNotHandled = (x) =>
        {
            Debug.Log(x);
        };
    }
    private void InitMessage()
    {
        receiver = new MessageTrans.DataReceiver();
        if (!receiver.RegistHook())
        {
            Debug.LogError("hook register err");
        }

        childSender = new MessageTrans.DataSender();
        parentSender = new MessageTrans.DataSender();
        receiver.RegisterEvent("simpleText", OnReceive);
        receiver.MessageNotHandled = Nohandle;
        receiver.OnError += OnError;
    }

    private void Nohandle(string str)
    {
        Debug.Log(str);
    }
    private void OnError(string err)
    {
        Debug.Log(err);
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
        childT.text = null;
        windowswitch.CloseChildWindow();
    }
    void SendInfoToChild (string text) {

        Win32API.SendMessage(windowswitch.Current, Win32API.WM_ACTIVATE, Win32API.WA_ACTIVE, IntPtr.Zero);

        if (!string.IsNullOrEmpty(text))
        {
            childSender.SendMessage("simpleText", text);
        }
	}
    void SendInfoToParent(string text)
    {
        Win32API.SendMessage(windowswitch.Current, Win32API.WM_ACTIVATE, Win32API.WA_ACTIVE, IntPtr.Zero);

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
        receiver.OnError -= OnError;
        receiver.RemoveHook();
        windowswitch.OnCloseThisWindow();
    }
}
