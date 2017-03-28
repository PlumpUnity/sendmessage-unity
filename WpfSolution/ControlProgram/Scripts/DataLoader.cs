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
using ControlProgram.Fixed;
namespace ControlProgram
{
    
    class DataLoader
    {
        Window window;
        public DataLoader(Window window)
        {
            this.window = window;
        }
        public void LoadProject(TreeView treeView)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(AppPath.WpfAssetPath+"/Text/Projects.xml");
            var xdp = ((XmlDataProvider)window.FindResource("xdp"));
            xdp.Document = doc;

            treeView.DataContext = xdp;
            treeView.SetBinding(TreeView.ItemsSourceProperty, new Binding());

        }
    }
}
