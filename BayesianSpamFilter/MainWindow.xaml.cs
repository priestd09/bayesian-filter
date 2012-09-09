using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Bayesian;

namespace BayesianSpamFilter
{
    public partial class MainWindow : Window
    {
        private SpamFilter _filter;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void TestFile(string file)
        {
            txtOut.Document.Blocks.Clear();
            string body = new StreamReader(file).ReadToEnd();

            FlowDocument d = new FlowDocument();
            Paragraph p = new Paragraph();

            double rate = _filter.Test(body);
            string textrate = "";

            if (rate > 0.7 & rate < 0.9)
            {
                textrate = "Сомнительное";
            }
            else if (rate < 0.7)
            {
                textrate = "OK";
            }
            else
            {
                textrate = "Спам";
            }
            label1.Content = "Оценка: " + textrate + "\tВероятность: " + rate.ToString("0.000000");
            p.Inlines.Add(body);
            d.Blocks.Add(p);
            txtOut.Document = d;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Application.Current.Shutdown();
        }

        void SaveToSpam()
        {
            try
            {
                TextRange textRange = new TextRange(txtOut.Document.ContentStart, txtOut.Document.ContentEnd);
                string body = textRange.Text;
                using (StreamWriter sw = new StreamWriter("../../Data/spam.txt", true, System.Text.Encoding.Default))
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(body);
                }

                ClearBase();

                Corpus bad = new Corpus();
                Corpus good = new Corpus();
                bad.LoadFromFile("../../Data/spam.txt");
                good.LoadFromFile("../../Data/ham.txt");

                _filter = new SpamFilter();
                _filter.Load(good, bad);

                foreach (string key in _filter.Prob.Keys)
                {
                    if (_filter.Prob[key] > 0.02)
                    {
                        lst.Add(new WordInfo(key, _filter.Prob[key].ToString("0.0000")));
                    }
                }

                for (int i = 0; i < lst.Count; i++)
                {
                    listView1.ItemsSource = lst;
                }

                _filter.ToFile("../../Data/out.txt");

            }
            catch
            {
                MessageBox.Show("Error.");
            }
        }

        private void exitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void loadmsgItem_Click(object sender, RoutedEventArgs e)
        {
            if (_filter == null)
            {
                MessageBox.Show("Загрузите данные обучения.");
                return;
            }
            else
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    TestFile(dlg.FileName);
                }
            }
        }

        public void ClearBase()
        {
            listView1.ItemsSource = null;
            listView1.Items.Clear();
            _filter = null;
            lst.Clear();
        }

        public class WordInfo
        {
            public string wordName { get; set; }
            public string wordProb { get; set; }

            public WordInfo(string wn, string wp)
            {
                wordName = wn;
                wordProb = wp;
            }
        }

        List<WordInfo> lst = new List<WordInfo>();

        private void loadItem_Click(object sender, RoutedEventArgs e)
        {
            ClearBase();

            Corpus bad = new Corpus();
            Corpus good = new Corpus();
            bad.LoadFromFile("../../Data/spam.txt");
            good.LoadFromFile("../../Data/ham.txt");

            _filter = new SpamFilter();
            _filter.Load(good, bad);

            //FlowDocument d = new FlowDocument();
            //Paragraph p = new Paragraph();

            //p.Inlines.Add(String.Format(@"{0} {1} {2}{3}", _filter.Good.Tokens.Count, _filter.Bad.Tokens.Count, _filter.Prob.Count, Environment.NewLine));

            foreach (string key in _filter.Prob.Keys)
            {
                if (_filter.Prob[key] > 0.02)
                {
                    lst.Add(new WordInfo(key, _filter.Prob[key].ToString("0.0000")));
                    //p.Inlines.Add(String.Format("{0}, {1}{2}", _filter.Prob[key].ToString("0.0000"), key, Environment.NewLine));
                }
            }
            for (int i = 0; i < lst.Count; i++)
            {
                listView1.ItemsSource = lst;
            }
            //d.Blocks.Add(p);
            //txtOut.Document = d;
        }

        private void clrItem_Click(object sender, RoutedEventArgs e)
        {
            txtOut.Document.Blocks.Clear();
        }

        private void testItem_Click(object sender, RoutedEventArgs e)
        {
            if (_filter == null)
            {
                MessageBox.Show("Load the learning data first.");
                return;
            }


            TextRange textRange = new TextRange(txtOut.Document.ContentStart, txtOut.Document.ContentEnd);

            string body = textRange.Text;

            double rate = _filter.Test(body);
            string textrate = "";

            if (rate > 0.7 & rate < 0.9)
            {
                textrate = "Сомнительное";
            }
            else if (rate < 0.7)
            {
                textrate = "OK";
            }
            else
            {
                textrate = "Спам";
            }

            label1.Content = "Оценка: " + textrate + "\tВероятность: " + rate.ToString("0.000000");
        }

        private void spamItem_Click(object sender, RoutedEventArgs e)
        {
            SaveToSpam();
        }

        private void fromItem_Click(object sender, RoutedEventArgs e)
        {
            ClearBase();

            _filter = new SpamFilter();
            _filter.FromFile("../../Data/out.txt");

            //FlowDocument d = new FlowDocument();
            //Paragraph p = new Paragraph();

            //p.Inlines.Add(String.Format(@"{0} {1} {2}{3}", _filter.Good.Tokens.Count, _filter.Bad.Tokens.Count, _filter.Prob.Count, Environment.NewLine));

            foreach (string key in _filter.Prob.Keys)
            {
                if (_filter.Prob[key] > 0.02)
                {
                    lst.Add(new WordInfo(key, _filter.Prob[key].ToString("0.0000")));
                    //p.Inlines.Add(String.Format("{0}, {1}{2}", _filter.Prob[key].ToString("0.0000"), key, Environment.NewLine));
                }
            }

            for (int i = 0; i < lst.Count; i++)
            {
                listView1.ItemsSource = lst;
            }

            //d.Blocks.Add(p);
            //txtOut.Document = d;
        }

        private void toItem_Click(object sender, RoutedEventArgs e)
        {
            if (_filter == null)
            {
                MessageBox.Show("Load first!");
                return;
            }

            _filter.ToFile("../../Data/out.txt");
        }
    }
}