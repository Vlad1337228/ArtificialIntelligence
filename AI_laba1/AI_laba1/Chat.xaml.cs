using AI_laba1.Controllers;
using AI_laba1.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace AI_laba1
{
    enum OperatingMode
    {
        searchAnswear,
        training,
        oneObject,
        whySuchAnAnswer
    }


    /// <summary>
    /// Логика взаимодействия для Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        private OperatingMode _operatingMode;
        private List<Question> _objects;
        private string _nameTransport;

        public Chat(List<Question> list)
        {
            InitializeComponent();
            _operatingMode = OperatingMode.searchAnswear;
            _objects = list;
            listChat.ItemsSource = new List<string>() {  "Вы загадали транспорт?" };
        }
        public Chat(Dictionary<string, List<Question>> questions)
        {
            InitializeComponent();
            textBox_answer.IsEnabled = false;
            btn_Send.IsEnabled = false;
            var promList = FormattingListString(questions);
            listChat.ItemsSource = promList;
        }

        public Chat(int n)
        {
            InitializeComponent();
            switch (n)
            {
                case 1:
                    _operatingMode = OperatingMode.training;
                    listChat.ItemsSource = new List<string> { "Подскажите правильный ответ. " };
                    break;
                case 3:
                    _operatingMode = OperatingMode.oneObject;
                    listChat.Items.Add("Введите транспорт, по которому нужна информация. ");
                    break;
                case 4:
                    _operatingMode = OperatingMode.whySuchAnAnswer;
                    listChat.Items.Add($"Введите объект, для которого хотите узнать \"Почему такой ответ?\" ");
                    break;
            }
            
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (_operatingMode)
            {
                case OperatingMode.searchAnswear:
                    SearchAnswer();
                    break;
                case OperatingMode.training:
                    Training();
                    break;
                case OperatingMode.oneObject:
                    GetOneObject();
                    break;
                case OperatingMode.whySuchAnAnswer:
                    _nameTransport = textBox_answer.Text;
                    WhyThatAnswer(_nameTransport);
                    break;
            }
            textBox_answer.Text = "";
        }

        
    }

    // режим поиска ответа
    public partial class Chat
    {
        private void SearchAnswer()
        {
            var answer = textBox_answer.Text;
            if (string.IsNullOrWhiteSpace(answer) || (answer != "да" && answer != "нет"))
            {
                MessageBox.Show("Некорректный ввод");
                return;
            }

            var listStringChat = listChat.ItemsSource.Cast<string>().ToList();
            listStringChat.Add(answer);

            if (listStringChat.Count <= 2)
            {
                if (answer == "нет")
                {
                    AddMessageToList(listStringChat, null);
                }
                else
                {
                    AddMessageToList(listStringChat, RandomTransport(_objects)?.Quest);
                }
                listChat.ItemsSource = listStringChat;
                return;
            }

            if (_objects.Count == 0)
            {
                if (answer == "да")
                    AddMessageToList(listStringChat, "Я выиграл");
                else
                    AddMessageToList(listStringChat, "Сдаюсь");
                listChat.ItemsSource = listStringChat;
                btn_Send.IsEnabled = false;
                return;
            }


            if (_objects.Count != 1)
            {
                var prom = _objects.Where(x => x.Quest == listStringChat[listStringChat.Count - 2] && x.Answer == "да").Select(x => x.Tr).ToList();

                if (answer == "да")
                {
                    _objects = _objects.Where(x => prom.Contains(x.Tr)).ToList();
                }

                if (answer == "нет")
                {
                    _objects = _objects.Where(x => !prom.Contains(x.Tr)).ToList();
                }

                if (!AnswerToQuestion(listStringChat)) return;
            }
            if (_objects.Count == 1)
            {
                listStringChat.RemoveAt(listStringChat.Count - 1);
                LastObjectInList(listStringChat, _objects);
                return;
            }
            listChat.ItemsSource = listStringChat;
        }

        private bool AnswerToQuestion(List<string> l)
        {
            if (_objects.Count == 1)
            {
                LastObjectInList(l, _objects);
                return false;
            }
            else
                _objects = _objects.Where(x => x.Quest != l[l.Count - 2]).ToList();

            AddMessageToList(l, RandomTransport(_objects)?.Quest);
            return true;
        }

        private void LastObjectInList(List<string> l, List<Question> obj)
        {
            var transport = obj[0].Tr;
            AddMessageToList(l, transport);
            listChat.ItemsSource = l;
            obj.Clear();
        }

        private void AddMessageToList(List<string> list, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                list.Add("Сдаюсь");
                btn_Send.IsEnabled = false;
                return;
            }
            list.Add(message);

        }

        private Question RandomTransport(List<Question> questions)
        {
            Random random = new Random();
            if(questions.Count == 1)
                return questions[0];
            if(questions.Count == 0)
                return null;
            return questions[random.Next(questions.Count)];
        }
    }

    // режим вывода всех данных
    public partial class Chat
    {
        private List<string> FormattingListString(Dictionary<string, List<Question>> dict)
        {
            var result = new List<string>();

            foreach (var item in dict)
            {
                var str = "Вы загадали транспорт? да -> ";
                foreach (var question in item.Value)
                {
                    str += question.Quest + " " + question.Answer + " -> ";
                }
                str += item.Key;
                result.Add(str);
            }
            return result;
        }
    }

    // режим обучения
    public partial class Chat
    {
        private int _numberQuestion = 0;
        private List<string> _questionTeaching = new List<string>() { "Сформулируйте впорос к этому транспорту. ", "Подскажите правильный ответ. " };
        private Question _questionForTeach = new Question();


        private void Training()
        {
            var answer = textBox_answer.Text;
            if (string.IsNullOrWhiteSpace(answer))
            {
                MessageBox.Show("Некорректный ввод");
                return;
            }

            var listStringChat = listChat.ItemsSource.Cast<string>().ToList();
            listStringChat.Add(answer);

            // обработка
            switch (_numberQuestion)
            {
                case 0:
                    _questionForTeach.Tr = answer;
                    break;
                case 1:
                    _questionForTeach.Quest = answer;
                    break;
                case 2:
                    _questionForTeach.Answer = answer;
                    break;
            }
            //

            if(_numberQuestion < _questionTeaching.Count)
            {
                listStringChat.Add(_questionTeaching[_numberQuestion]);
                _numberQuestion++;
            }
            else
            {
                btn_Send.IsEnabled = false;
                textBox_answer.IsEnabled = false;
                btn_Save.IsEnabled = true;
            }
            listChat.ItemsSource = listStringChat;
            

        }
    }

    // сохранение данных
    public partial class Chat
    {
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_questionForTeach == null)
            {
                MessageBox.Show("Неполная информация об объекте.");
                return;
            }
            if(string.IsNullOrEmpty(_questionForTeach.Quest) || string.IsNullOrEmpty(_questionForTeach.Answer) || string.IsNullOrEmpty(_questionForTeach.Tr))
            {
                MessageBox.Show("Неполная информация об объекте.");
                return;
            }

            WorkWithDB.SaveQuestion(_questionForTeach);
        }
    }

    // вывод одного объекта
    public partial class Chat
    {
        private void GetOneObject()
        {

            if (string.IsNullOrEmpty(textBox_answer.Text) || string.IsNullOrWhiteSpace(textBox_answer.Text))
            {
                MessageBox.Show("Введите название транспорта. ");
                return;
            }

            var answer = textBox_answer.Text;
            listChat.Items.Add(answer);

            var listOneTransport = WorkWithDB.GetOneQuestion(answer);

            foreach (var item in listOneTransport)
            {
                listChat.Items.Add(item.Quest + " " + item.Answer);
            }

            textBox_answer.IsEnabled = false;
            btn_Send.IsEnabled = false;

        }
    }

    // почему такой ответ?
    public partial class Chat
    {
        private void WhyThatAnswer(string nameTransport)
        {
            if(string.IsNullOrEmpty(nameTransport))
            {
                MessageBox.Show("Введите корректный транспорт! ");
                return;
            }

            var listTr = WorkWithDB.GetOneQuestion(nameTransport);
            if(listTr == null)
            {
                listChat.Items.Add("Система не знает такого траспорта! ");
                return;
            }
            if (listTr.Count <= 0)
            {
                listChat.Items.Add("Ввидите другой транспорт ");
                return;
            }

            var listProm = listTr.Where(x => x.Answer.ToLower() == "да").ToList();

            if(listProm.Count <= 0)
            {
                listProm = listTr.Where(x => x.Answer.ToLower() == "нет").ToList();
                if(listProm.Count <= 0)
                {
                    listChat.Items.Add("Ввидите другой транспорт ");
                    return;
                }
            }

            try
            {
                var resultStr = "У транспорта ";
                var countListTr = listTr.Count;
                Random random = new Random();
                var tr = listTr[random.Next(countListTr)];
                if(tr.Answer == "да")
                {
                    resultStr += $"\"{tr.Quest.Trim('?').ToLower()}\" " + ", следовательно, это " + $"{tr.Tr}. ";
                }
                else
                {
                    resultStr += $"\"{tr.Quest.Trim('?').ToLower().Replace("есть","нет")}\" " + ", следовательно, это " + $"{tr.Tr}. ";
                }

                listChat.Items.Add(resultStr);
            }
            catch
            {
                MessageBox.Show("Ошибка на сервере! ");
                return;
            }

            textBox_answer.IsEnabled = false;
            btn_Send.IsEnabled = false;

        }
    }

}
