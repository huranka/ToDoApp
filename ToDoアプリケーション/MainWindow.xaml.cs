using System;
using System.Collections.Generic;
using System.IO;
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
using TaskList;

namespace ToDoアプリケーション
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        WrapPanel wrapPanelTop_ = new WrapPanel();

        public MainWindow()
        {
            InitializeComponent();

            wrapPanelTop_.HorizontalAlignment = HorizontalAlignment.Left;
            wrapPanelTop_.VerticalAlignment = VerticalAlignment.Top;

            scrollViewer.Content = wrapPanelTop_;

        }

        private void MenuItem_TaskListAddClick(object sender, RoutedEventArgs e)
        {
            var taskList = new TaskList.TaskListM();
            taskList.Margin = new Thickness(3);
            wrapPanelTop_.Children.Add(taskList);

            //コンテキストメニュー
            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem();
            menuItem.Click += MenuItemClick_TaskListDelete;
            menuItem.Header = "タスクリスト削除";
            contextMenu.Items.Add(menuItem);
            wrapPanelTop_.ContextMenu = contextMenu;
        }

        private void MenuItemClick_TaskListDelete(object sender, RoutedEventArgs e)
        {
            var taskList = (TaskList.TaskListM)sender;
            wrapPanelTop_.Children.Remove(taskList);
            
        }


        private void ButtonFileSave_Click(object sender, RoutedEventArgs e)
        {
            int i = 1;  //添付番号をファイル名にする
            foreach(var element in wrapPanelTop_.Children)
            {
                var taskList = (TaskList.TaskListM)element;

                FileControl.SaveFile(taskList.GetString(), i.ToString()+".txt");
                i++;
            }
        }

        private void ButtonFileLoad_Click(object sender, RoutedEventArgs e)
        {
            wrapPanelTop_.Children.Clear();

            var files = Directory.GetFiles(FileControl.topFolderPath_);
            foreach (var path in files)
            {
                string data = "";
                FileControl.LoadFile(ref data, path);

                var taskList = new TaskList.TaskListM();
                taskList.Margin = new Thickness(3);
                taskList.SetString(data);

                wrapPanelTop_.Children.Add(taskList);
            }

        }
    }
}
