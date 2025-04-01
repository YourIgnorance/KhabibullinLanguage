using Microsoft.Win32;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KhabibullinLanguage
{
    public partial class AddEditPage : Page
    {
        private ClientPage _ClientsPage = new ClientPage();
        private Client _currentClient = new Client();
        DateTime? ClientBirthday;
        public AddEditPage(Client SelectedClient)
        {
            InitializeComponent();

            if (SelectedClient != null)
                _currentClient = SelectedClient;

            DataContext = _currentClient;
            if (_currentClient.GenderCode == "м")
                ComboType.SelectedIndex = 0;
            if (_currentClient.GenderCode == "ж")
                ComboType.SelectedIndex = 1;
            else
                ComboType.SelectedIndex = 0;

            ClientBirthday = _currentClient.Birthday;
            dtPicker.Text = ClientBirthday.ToString();
            if (SelectedClient == null)
                RegistrationDateTB.Text = DateTime.Now.ToString();
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                _currentClient.PhotoPath = myOpenFileDialog.FileName; // !!
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }


        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentClient.LastDateTime))
                errors.AppendLine("Укажите фамилию клиента");

            if (string.IsNullOrWhiteSpace(_currentClient.FirstName))
                errors.AppendLine("Укажите имя клиента");

            if (string.IsNullOrWhiteSpace(_currentClient.Patronymic))
                errors.AppendLine("Укажите отчество клиента");

            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите пол клиента");

            if (string.IsNullOrWhiteSpace(_currentClient.Phone))
                errors.AppendLine("Укажите телефон агента");

            if (ClientBirthday == null)
                errors.AppendLine("Укажите дату рождения клиента");

            if (string.IsNullOrWhiteSpace(_currentClient.Email))
                errors.AppendLine("Укажите почту клиента");

            if (string.IsNullOrWhiteSpace(_currentClient.RegistrationDate.ToString()))
                errors.AppendLine("Укажите дату регистрации клиента");

            else
            {
                string ph = _currentClient.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "");

                if ((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11 || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон клиента");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentClient.Gender = (Gender)KhabibullinLanguageEntities.getInstance().Gender.Where(p => p.Code == ComboType.SelectedValue).Select(p => p.Code);
            _currentClient.Birthday = ClientBirthday;

            if (_currentClient.ID == 0)
                KhabibullinLanguageEntities.getInstance().Client.Add(_currentClient);

            try
            {
                KhabibullinLanguageEntities.getInstance().SaveChanges();

                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void dtPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ClientBirthday = (DateTime)(((DatePicker)sender).SelectedDate);
        }
    }
}
