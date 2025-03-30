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

namespace KhabibullinLanguage
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        public int CountRecords, CountPage, CurrentPage = 0;
        List<Client> CurrentClientList = new List<Client>();
        List<Client> TableList;

        public ClientPage()
        {
            InitializeComponent();

            var currentClients = KhabibullinLanguageEntities.getInstance().Client.ToList();

            ClientListView.ItemsSource = currentClients;

            UpdateProducts();
        }
        public void UpdateProducts()
        {
            var currentAgents = KhabibullinLanguageEntities.getInstance().Client.ToList();

            ClientListView.ItemsSource = currentAgents;
           
            TableList = currentAgents;
            ChangePage(0, 0);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox.SelectedIndex == 0)
            {
                var currentAgents = KhabibullinLanguageEntities.getInstance().Client.ToList();
                List<Client> clList = new List<Client>();
                for(int i = 0; i < 10; i++)
                    currentAgents = clList.Add(KhabibullinLanguageEntities.getInstance().Client.)
                ClientListView.ItemsSource = currentAgents;
            }
            if (SortComboBox.SelectedIndex == 1)
            {
                var currentAgents = KhabibullinLanguageEntities.getInstance().Client.ToList();
            }
            if (SortComboBox.SelectedIndex == 2)
            {

            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentClients = (sender as Button).DataContext as Client;
            
            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    KhabibullinLanguageEntities.getInstance().Client.Remove(currentClients);
                    KhabibullinLanguageEntities.getInstance().SaveChanges();

                    ClientListView.ItemsSource = KhabibullinLanguageEntities.getInstance().Client.ToList();

                    UpdateProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        public void ChangePage(int direction, int? selectedPage)
        {
            CurrentClientList.Clear();
            CountRecords = TableList.Count;

            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }
            Boolean Ifupdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;

                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentClientList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentClientList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;

                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentClientList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                }
            }
            if (Ifupdate)
            {
                PageListBox.Items.Clear();

                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = $" из {CountRecords.ToString()}";

                ClientListView.ItemsSource = CurrentClientList;

                ClientListView.Items.Refresh();
            }
        }
    }
}
