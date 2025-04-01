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
        public int CountRecords, CountPage, CurrentPage = 0, ALLClientCount;
        List<Client> CurrentClientList = new List<Client>();
        List<Client> TableList;

        public ClientPage()
        {
            InitializeComponent();

            var currentClients = KhabibullinLanguageEntities.getInstance().Client.ToList();

            ClientListView.ItemsSource = currentClients;

            SortComboBox.SelectedIndex = 0;

            UpdateClients();
        }
        public void UpdateClients()
        {
            var currentAgents = KhabibullinLanguageEntities.getInstance().Client.ToList();
            ALLClientCount = KhabibullinLanguageEntities.getInstance().Client.Count();

            if (SortComboBox1.SelectedIndex == 0)
            {
                currentAgents = currentAgents.OrderBy(p => p.ID).ToList();
            }
            if (SortComboBox1.SelectedIndex == 1)
            {
                currentAgents = currentAgents.OrderBy(p => p.LastName).ToList();
            }
            if (SortComboBox1.SelectedIndex == 2)
            {
                currentAgents = currentAgents.OrderByDescending(p => p.StartDateTime).ToList(); 
            }
            if (SortComboBox1.SelectedIndex == 3)
            {
                currentAgents = currentAgents.OrderByDescending(p => p.VisitCount).ToList();
            }

            if (FiltrComboBox.SelectedIndex == 0)
            {
                currentAgents = currentAgents.ToList();
            }
            if (FiltrComboBox.SelectedIndex == 1)
            {
                currentAgents = currentAgents.Where(p => Convert.ToString(p.GenderName) == "женский").ToList();
            }
            if (FiltrComboBox.SelectedIndex == 2)
            {
                currentAgents = currentAgents.Where(p => Convert.ToString(p.GenderName) == "мужской").ToList();
            }
            
            currentAgents = currentAgents.Where(p => p.FIO.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Phone.ToLower().Replace("(", "").Replace(")", "").Replace("+", "").Replace("-", "").Replace(" ", "").Contains(TBoxSearch.Text.ToLower())).ToList();
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
            UpdateClients();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients();
        }

        private void FiltrComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void SortComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Client));
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;

            if (currentClient.VisitCount != 0)
            {
                MessageBox.Show("Внимание удаление невозможно, так как у этого пользователя есть записи посещения!");
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        KhabibullinLanguageEntities.getInstance().Client.Remove(currentClient);
                        KhabibullinLanguageEntities.getInstance().SaveChanges();

                        ClientListView.ItemsSource = KhabibullinLanguageEntities.getInstance().Client.ToList();

                        UpdateClients();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }

        }
        public void ChangePage(int direction, int? selectedPage)
        {
            int currentRecordsOnPage = 10;

            CurrentClientList.Clear();
            CountRecords = TableList.Count;

            if (SortComboBox.SelectedIndex == 0)
            {
                currentRecordsOnPage = 10;
                if (CountRecords % currentRecordsOnPage > 0)
                {
                    CountPage = CountRecords / currentRecordsOnPage + 1;
                }
                else
                {
                    CountPage = CountRecords / currentRecordsOnPage;
                }
            }
            if (SortComboBox.SelectedIndex == 1)
            {
                currentRecordsOnPage = 50;
                if (CountRecords % currentRecordsOnPage > 0)
                {
                    CountPage = CountRecords / currentRecordsOnPage + 1;
                }
                else
                {
                    CountPage = CountRecords / currentRecordsOnPage;
                }
            }
            if (SortComboBox.SelectedIndex == 2)
            {
                currentRecordsOnPage = 200;
                if (CountRecords % currentRecordsOnPage > 0)
                {
                    CountPage = CountRecords / currentRecordsOnPage + 1;
                }
                else
                {
                    CountPage = CountRecords / currentRecordsOnPage;
                }
            }
            if (SortComboBox.SelectedIndex == 3)
            {
                currentRecordsOnPage = KhabibullinLanguageEntities.getInstance().Client.Select(p => p.ID).Count();
                CountPage = 1;
            }
            Boolean Ifupdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * currentRecordsOnPage + currentRecordsOnPage < CountRecords ? CurrentPage * currentRecordsOnPage + currentRecordsOnPage : CountRecords;

                    for (int i = CurrentPage * currentRecordsOnPage; i < min; i++)
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
                            min = CurrentPage * currentRecordsOnPage + currentRecordsOnPage < CountRecords ? CurrentPage * currentRecordsOnPage + currentRecordsOnPage : CountRecords;
                            for (int i = CurrentPage * currentRecordsOnPage; i < min; i++)
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
                            min = CurrentPage * currentRecordsOnPage + currentRecordsOnPage < CountRecords ? CurrentPage * currentRecordsOnPage + currentRecordsOnPage : CountRecords;
                            for (int i = CurrentPage * currentRecordsOnPage; i < min; i++)
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

                min = CurrentPage * currentRecordsOnPage + currentRecordsOnPage < CountRecords ? CurrentPage * currentRecordsOnPage + currentRecordsOnPage : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = $" из {ALLClientCount.ToString()}";

                ClientListView.ItemsSource = CurrentClientList;

                ClientListView.Items.Refresh();
            }
        }
    }
}
