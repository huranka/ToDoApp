using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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
using System.Text.RegularExpressions;

namespace TaskList
{
    /// <summary>
    /// TaskList.xaml の相互作用ロジック
    /// </summary>
    public partial class TaskListM : UserControl
    {
        TextBox focus_textBox_ = new TextBox();
        StackPanel stack_panel_top_ = new StackPanel();

        static readonly string checkBoxMsgOn_ = "{checkBox On}";
        static readonly string checkBoxMsgOff_ = "{checkBox Off}";
        static readonly string checkBoxMsgRegex_ = "{checkBox (On|Off)}";
        static readonly string checkBoxTextBoxEndFlag_ = "`````";

        public TaskListM()
        {
            InitializeComponent();

            Border border = new Border();
            border.CornerRadius = new CornerRadius(20);
            border.Background = Brushes.Yellow;

            Grid grid = new Grid();
            grid.Margin = new Thickness(10);
            grid.RowDefinitions.Add(new RowDefinition());
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(30);
            grid.RowDefinitions.Add(rowDefinition);

            border.Child = grid;

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Height = 200;

            Grid.SetRow(scrollViewer,0);

            stack_panel_top_.HorizontalAlignment = HorizontalAlignment.Left;
            stack_panel_top_.VerticalAlignment = VerticalAlignment.Top;
            stack_panel_top_.Width = 350;

            scrollViewer.Content = stack_panel_top_;

            TextBox textBox = new TextBox();
            textBox.Style = (Style)this.FindResource("DefaultTextBox1");
            //textBox.HorizontalAlignment = HorizontalAlignment.Left;
            //textBox.TextWrapping = TextWrapping.Wrap;
            //textBox.BorderThickness = new Thickness(0);
            //textBox.Background = Brushes.Transparent;
            //textBox.Width = 330;
            textBox.Text = "testBox";
            textBox.SelectionChanged += new RoutedEventHandler(TextBox_SelectionChanged);

            stack_panel_top_.Children.Add(textBox);

            Button button = new Button();
            button.Content = "☑";
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.Height = 25;
            button.Width = 25;
            button.Margin = new Thickness(33, 0, 0, 0);
            button.Background = Brushes.WhiteSmoke;
            button.Click += new RoutedEventHandler(button_Click_Create_CheckBox);

            Grid.SetRow(button, 1);

            grid.Children.Add(scrollViewer);
            grid.Children.Add(button);

            this.Content = border;

            focus_textBox_ = textBox;    //初期チェックボックス追加位置
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var txt = sender.ToString();
            focus_textBox_ = (TextBox)(sender);
        }

        private void button_Click_Create_CheckBox(object sender, RoutedEventArgs e)
        {
             var txt = sender.ToString();
            try
            {
                if (focus_textBox_ == null) return;

                var textMsg = focus_textBox_.Text;
                if (textMsg.Length == 0) return;

                var front_text = textMsg.Substring(0, focus_textBox_.CaretIndex);
                var back_text = textMsg.Substring(focus_textBox_.CaretIndex);

                int focus_number = stack_panel_top_.Children.IndexOf(focus_textBox_);


                TextBox textBox_front = new TextBox();
                textBox_front.Style = (Style)this.FindResource("DefaultTextBox1");
                textBox_front.Text = front_text;
                textBox_front.SelectionChanged += new RoutedEventHandler(TextBox_SelectionChanged);


                StackPanel stackPanel = new StackPanel();
                stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
                stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
                stackPanel.Orientation = Orientation.Horizontal;

                CheckBox checkBox = new CheckBox();
                checkBox.Style = (Style)FindResource("DefaultStackPanelCheckBox");
                checkBox.Click += CheckBox_Click;
                stackPanel.Children.Add(checkBox);

                TextBox textBox = new TextBox();
                textBox.Style = (Style)FindResource("DefaultTextBox1");
                textBox.SelectionChanged += new RoutedEventHandler(TextBox_SelectionChanged);
                stackPanel.Children.Add(textBox);

                TextBox textBox_back = new TextBox();
                textBox_back.Style = (Style)this.FindResource("DefaultTextBox1");
                textBox_back.Text = back_text;
                textBox_back.SelectionChanged += new RoutedEventHandler(TextBox_SelectionChanged);

                stack_panel_top_.Children.Remove(focus_textBox_);
                stack_panel_top_.Children.Insert(focus_number, textBox_front);
                stack_panel_top_.Children.Insert(focus_number + 1, stackPanel);
                stack_panel_top_.Children.Insert(focus_number + 2, textBox_back);

            } catch (Exception exception) {
                MessageBox.Show(exception.Message);
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            foreach(var element in stack_panel_top_.Children)
            {
                if (!(element is StackPanel)) continue;

                var stackPanel = (StackPanel)element;
                if (!(stackPanel.Children.Contains(checkBox)) ) continue;

                var textBox = (TextBox)stackPanel.Children[1];
                if (checkBox.IsChecked == true)
                {
                    textBox.TextDecorations = TextDecorations.Strikethrough;
                } else
                {
                    textBox.TextDecorations = null;
                }

            }

        }

        public string GetString()
        {
            string allMsg = "";
            foreach (var element in stack_panel_top_.Children)
            {
                if (element is TextBox)
                {
                    var textBox = (TextBox)element;
                    allMsg += textBox.Text;

                } else if (element is StackPanel)
                {
                    var stackPanel = (StackPanel)element;
                    var checkBox = (CheckBox)stackPanel.Children[0];
                    if (checkBox.IsChecked == true)
                    {
                        allMsg += checkBoxMsgOn_;
                    } else
                    {
                        allMsg += checkBoxMsgOff_;
                    }
                    var textBox = (TextBox)stackPanel.Children[1];
                    allMsg += textBox.Text + checkBoxTextBoxEndFlag_;
                }
            }

            return allMsg;
        }

        // 文字列に応じて、コントロールをセット
        public void SetString(string data)
        {
            stack_panel_top_.Children.Clear();

            int prev_end_index = 0; //検索後のチェックボックス横のテキストボックスの終端インデックス
            while(true)
            {
                var rx = new Regex(checkBoxMsgRegex_, RegexOptions.Compiled);
                var match = rx.Match(data, prev_end_index + 1);
                if (match.Success)
                {
                    int textBoxLength1 = match.Index - prev_end_index;
                    int textBox2IndexStartIndex = match.Index + match.Length;
                    int textBox2IndexEndIndex = data.IndexOf(checkBoxTextBoxEndFlag_, textBox2IndexStartIndex);
                    string textBox2Data = data.Substring(textBox2IndexStartIndex, textBox2IndexEndIndex - textBox2IndexStartIndex);

                    if (textBoxLength1 != 0)
                    {
                        //チェックボックス前のTextBox追加
                        var textBox = new TextBox();
                        textBox.Style = (Style)this.FindResource("DefaultTextBox1");
                        textBox.SelectionChanged += new RoutedEventHandler(TextBox_SelectionChanged);
                        textBox.Text = data.Substring(0, match.Index);
                        stack_panel_top_.Children.Add(textBox);
                    }

                    var stackPanel = new StackPanel();
                    stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
                    stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
                    stackPanel.Orientation = Orientation.Horizontal;

                    var checkBox = new CheckBox();
                    checkBox.Style = (Style)FindResource("DefaultStackPanelCheckBox");
                    checkBox.Click += CheckBox_Click;
                    stackPanel.Children.Add(checkBox);

                    //チェックボックス横のテキストボックス追加
                    var textBox2 = new TextBox();
                    textBox2.Style = (Style)this.FindResource("DefaultTextBox1");
                    textBox2.SelectionChanged += new RoutedEventHandler(TextBox_SelectionChanged);
                    textBox2.SelectionChanged += new RoutedEventHandler(TextBox_SelectionChanged);
                    textBox2.Text = textBox2Data;
                    stackPanel.Children.Add(textBox2);
                    if (match.Value == checkBoxMsgOn_)
                    {
                        checkBox.IsChecked = true;
                        textBox2.TextDecorations = TextDecorations.Strikethrough;
                    }
                    else
                    {
                        checkBox.IsChecked = false;
                    }

                    stack_panel_top_.Children.Add(stackPanel);

                    prev_end_index = textBox2IndexEndIndex + checkBoxTextBoxEndFlag_.Length;

                    continue;

                } else
                {
                    //最後のテキストボックス追加
                    var textBox = new TextBox();
                    textBox.Style = (Style)this.FindResource("DefaultTextBox1");
                    textBox.SelectionChanged += new RoutedEventHandler(TextBox_SelectionChanged);
                    textBox.Text = data.Substring(prev_end_index);
                    stack_panel_top_.Children.Add(textBox);

                    focus_textBox_ = textBox;    //初期チェックボックス追加位置
                    break;
                }
            }
        }
    }
}
