using System;
using System.Windows;
using System.Windows.Controls;
using FSharperRegexCompiler;

namespace TextAnalystUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Input.TextChanged +=Input_TextChanged;
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Output.Text = Program.Do(Input.Text);
            }
            catch (Exception exception)
            {
                Output.Text = "Exception: \r\n" + exception.Message;
            }
        }
    }
}
