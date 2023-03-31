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

namespace PnC_Insurance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            using (var context = new Model.InsuranceDbContext())
            {
                var t = context.Agents.FirstOrDefault().Urn.ToString();
                var r = context.Agents.FirstOrDefault().FullName.ToString();

                MessageBox.Show("URN: " + t + " - Name: " + r);
            }
            
        }
    }
}
