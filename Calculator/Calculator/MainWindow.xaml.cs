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


namespace Calculator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window { 
    
        private char previous = 'E';
        private bool resultGot = false;
        private InputParser parser = new InputParser();

        public MainWindow()
        {
            InitializeComponent();
            foreach (UIElement c in LayoutRoot.Children)
            {
                if (c is Button)
                {
                    ((Button)c).Click += Button_Click;
                }
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = (string)((Button)e.OriginalSource).Content;
            if (resultGot)
            {
                Input.Text = "";
                Answer.Text = "";
                resultGot = false;
            }
            switch (buttonName)
            {
                case "=":
                    Answer.Text = parser.ParserMain(Input.Text); 
                    resultGot = true;
                    break;
                case "BACKSPACE":
                    if(Input.Text.Length>0)
                    {
                       Input.Text = Input.Text.Remove(Input.Text.Length - 1);                       
                    }                    
                    break;
                case "CLEAR":
                    Input.Text = "";            
                    break;
                default:
                    if (parser.isOperation(previous)&&parser.isOperation(buttonName[0])||previous==','&&buttonName==",")
                    {
                        Input.Text = Input.Text.Remove(Input.Text.Length - 1);
                    }                    
                    Input.Text += buttonName;
                    break;                
            }
            previous = Input.Text.Length > 0 ? Input.Text[Input.Text.Length - 1] : 'E';
        }          
    }
}
