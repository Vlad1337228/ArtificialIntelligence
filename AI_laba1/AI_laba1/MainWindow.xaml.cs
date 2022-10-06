using AI_laba1.Controllers;
using AI_laba1.Model;
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

namespace AI_laba1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        public List<Question> questions;
        public List<string> chat;

        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            questions = WorkWithDB.ReadAllRow();
            var chat = new Chat(questions);
            chat.Owner = this;
            chat.ShowDialog();
        }

        private void Btn_AllRowDB_Click(object sender, RoutedEventArgs e)
        {
            var dictList = WorkWithDB.ReadAllRow()
                                        .Where(x => x.Answer == "да")
                                        .OrderBy(x => x.Tr)
                                        .GroupBy(x => x.Tr)
                                        .ToDictionary( x => x.Key, x => x.ToList());

            var chat = new Chat(dictList);
            chat.Owner = this;
            chat.ShowDialog();
        }

        private void Btn_Study_Click(object sender, RoutedEventArgs e)
        {
            var chat = new Chat(1);
            chat.Owner = this;
            chat.ShowDialog();
            
        }

        private void Btn_InfoObject_Click(object sender, RoutedEventArgs e)
        {
            var chat = new Chat(3);
            chat.Owner = this;
            chat.ShowDialog();
        }

        private void Btn_WhyAnswer_Click(object sender, RoutedEventArgs e)
        {
            var chat = new Chat(4);
            chat.Owner = this;
            chat.ShowDialog();
        }

        
    }
}
