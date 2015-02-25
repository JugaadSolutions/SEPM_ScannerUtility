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
        DataAccess da;

        String[] Receivers;
        public MainWindow()
        {
            InitializeComponent();

            ScannerTextBox.TextChanged += ScannerTextBox_TextChanged;

            String connectionString = System.Configuration.ConfigurationSettings.AppSettings["DBConStr"];

            da = new DataAccess(connectionString);

            String receiverList = System.Configuration.ConfigurationSettings.AppSettings["RECEIVER_LIST"];

            Char[] separator = {','};

             Receivers = receiverList.Split(separator);

            



        }

        void ScannerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool newlineFlag = false;
            TextBox tb = (TextBox)sender;
            if (tb.Text.Contains(Environment.NewLine))
            {
                foreach (String r in Receivers)
                    da.insertSmsTrigger(r, "Scanned Code - " + tb.Text, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            if (newlineFlag == true)
                tb.Clear();
        }
    }
}
