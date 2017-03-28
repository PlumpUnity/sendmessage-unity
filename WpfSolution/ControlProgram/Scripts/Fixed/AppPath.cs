using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlProgram.Fixed
{
    public static class AppPath
    {
        public static string ExePath
        {
            get{
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
        public static string WpfAssetPath
        {
            get
            {
                return ExePath.Substring(0, ExePath.IndexOf(@"\2.WPF")) + @"\3.Resource\WpfAsset\";
            }
        }
        public static string ThreeDExePath
        {
            get
            {
                return WpfAssetPath + @"ThreeD\ThreeD.exe";
            }
        }
    }
}
