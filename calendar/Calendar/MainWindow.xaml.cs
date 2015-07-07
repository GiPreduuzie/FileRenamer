using System.Windows;
using FSharpCalendar;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // printYear year orientation monthInRow
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int year = int.Parse(YearInput.Text);
            int inRow = int.Parse(InRowInput.Text);

            var orientaion = FSharpCalendar.Calendar.Orientation.Ver;

            if (Vertical.IsChecked.Value)
                orientaion = FSharpCalendar.Calendar.Orientation.Ver;

            if (Horizontal.IsChecked.Value)
                orientaion = FSharpCalendar.Calendar.Orientation.Hor;


            Output.Text = new Class1().PrintYear(year, orientaion, inRow);
        }
    }
}