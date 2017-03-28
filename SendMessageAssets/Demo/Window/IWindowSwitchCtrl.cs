using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
public interface IWindowSwitchCtrl {
    IntPtr Parent { get; }
    IntPtr Child { get; }
    IntPtr Current { get; }
    bool OnOpenedByParent(out string[] args);
    bool OpenChildWindow(string path,bool hideThis,params string[] args);
    bool CloseChildWindow();
    void OnCloseThisWindow();
}
