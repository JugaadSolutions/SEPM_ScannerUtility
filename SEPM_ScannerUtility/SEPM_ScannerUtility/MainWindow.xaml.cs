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

namespace SEPM_ScannerUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ScannerTextBox.TextChanged += ScannerTextBox_TextChanged;
        }

        void ScannerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool newlineFlag = false;
            TextBox tb = (TextBox)sender;
            if (tb.Text.Contains(Environment.NewLine))
            {
                MessageBox.Show("new entry", "alert", MessageBoxButton.OK, MessageBoxImage.Information);
                newlineFlag = true;
            }

            if (newlineFlag == true)
                tb.Clear();
        }
    }
}
