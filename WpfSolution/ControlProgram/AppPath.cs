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
                return ExePath.Substring(0, ExePath.IndexOf(@"\ControlProgram")) + @"\ThreeD\";
            }
        }
        public static string ThreeDExePath
        {
            get
            {
                return WpfAssetPath + @"ThreeD.exe";
            }
        }
    }
}
