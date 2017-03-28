using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using MessageTrans;
namespace ControlProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UnityLoder _3dLoader;
        private DataReceiver receiver = new DataReceiver();
        public MainWindow()
        {
            InitializeComponent();
            _3dLoader = new UnityLoder(this,leftUpGrid, threeD);
            Closed += OnClose;
            receiver.RegistHook();
            receiver.RegisterEvent("simpleText", SetText);
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void OnClose(object sender, EventArgs e)
        {
            receiver.RemoveHook();
        }

        private void SetText(string text)
        {
            label.Content += text + "\n";
        }
        private void runUnity_Click(object sender, RoutedEventArgs e)
        {
            //创建一个打开文件式的对话框  
            OpenFileDialog ofd = new OpenFileDialog();
            //设置这个对话框的起始打开路径  
            ofd.InitialDirectory = @"D:\";
            //设置打开的文件的类型，注意过滤器的语法  
            ofd.Filter = "程序|*.exe|程序文件|*.exe";
            //调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮  
            string path = "";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    path = ofd.FileName;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            if (!string.IsNullOrEmpty(path))
            {
                _3dLoader.Init3dScene(path);
            }
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendtext_Click(object sender, RoutedEventArgs e)
        {
            _3dLoader.SendInfo("哈哈哈");
            Console.WriteLine("发送信息");
        }
        private void OnStopThreeD(object sender, RoutedEventArgs e)
        {
            _3dLoader.CloseWindow();
        }
    }
}