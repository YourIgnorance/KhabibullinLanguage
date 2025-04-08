using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

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
            IDTB.Visibility = Visibility.Visible;
            if (SelectedClient != null)
            {
                _currentClient = SelectedClient;
                IDTB.IsEnabled = false;
                RegistrationDateTB.Text = SelectedClient.RegistrationDate.ToShortDateString();
                RegistrationDateTB.IsEnabled = false;
                if (_currentClient.GenderCode == "м")
                    ComboType.SelectedIndex = 0;
                if (_currentClient.GenderCode == "ж")
                    ComboType.SelectedIndex = 1;
            }
            DataContext = _currentClient;

            ClientBirthday = _currentClient.Birthday;
            dtPicker.Text = ClientBirthday.ToString();
            if (SelectedClient == null)
            {
                RegistrationDateTB.Text = DateTime.Now.ToShortDateString();
                RegistrationDateTB.IsEnabled = false;
                IDTB.Visibility = Visibility.Hidden;
                IDText.Visibility = Visibility.Hidden;
            }
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            string clientsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Clients");

            OpenFileDialog myOpenFileDialog = new OpenFileDialog
            {
                InitialDirectory = clientsFolderPath,
                Filter = "Image files (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите изображение клиента"
            };

            if (myOpenFileDialog.ShowDialog() == true)
            {
                // Получаем имя файла и формируем относительный путь
                string fileName = Path.GetFileName(myOpenFileDialog.FileName);
                string relativePath = Path.Combine("Clients", fileName); // Относительный путь

                _currentClient.PhotoPath = fileName;

                // Устанавливаем изображение
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }
        private bool IsValidEmail()
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]{2,}\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(_currentClient.Email, pattern);
        }
        private bool IsValidName(string name)
        {
            string pattern = @"^[А-Яа-яЁё\s\-]+$";
            return Regex.IsMatch(name, pattern);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentClient.LastName))
                errors.AppendLine("Укажите фамилию клиента");
            else
            {
                if (_currentClient.LastName.Length > 50)
                    errors.AppendLine("В поле фамилия должено быть меньше 50 символов");
                if (!IsValidName(_currentClient.LastName))
                    errors.AppendLine("Поле фамилия указан некоректно (некоректный ввод данных)");
            }
            if (string.IsNullOrWhiteSpace(_currentClient.FirstName))
                errors.AppendLine("Укажите имя клиента");
            else
            {
                if (_currentClient.FirstName.Length > 50)
                    errors.AppendLine("В поле имя должено быть меньше 50 символов");
                if (!IsValidName(_currentClient.FirstName))
                    errors.AppendLine("Поле имя указан некоректно (некоректный ввод данных)");
            }
            if (string.IsNullOrWhiteSpace(_currentClient.Patronymic))
                errors.AppendLine("Укажите отчество клиента");
            else
            {
                if (_currentClient.Patronymic.Length > 50)
                    errors.AppendLine("В поле отчество должено быть меньше 50 символов");
                if (!IsValidName(_currentClient.Patronymic))
                    errors.AppendLine("Поле отчество указан некоректно (некоректный ввод данных)");
            }
            if (ComboType.SelectedIndex == -1)
                errors.AppendLine("Укажите пол клиента");

            if (string.IsNullOrWhiteSpace(_currentClient.Phone))
                errors.AppendLine("Укажите телефон клиента");
            else
            {
                string ph = _currentClient.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "");
                string allowedChars = "0123456789+-() ";

                if ((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11 || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон клиента");
                if (_currentClient.Phone.Any(c => !allowedChars.Contains(c)))
                {
                    errors.AppendLine("Поле телефон указана некоректно (некоректный ввод данных)");
                }
            }

            if (ClientBirthday == null)
                errors.AppendLine("Укажите дату рождения клиента");
            else
            {
                if(ClientBirthday > DateTime.Today)
                    errors.AppendLine("Дата рождения клиента указана неверно");
            }
                

            if (string.IsNullOrWhiteSpace(_currentClient.Email))
                errors.AppendLine("Укажите почту клиента");
            else
            {
                if(!IsValidEmail())
                    errors.AppendLine("Укажите корректную почту клиента");
            }


            if (string.IsNullOrWhiteSpace(_currentClient.RegistrationDate.ToString()))
                errors.AppendLine("Укажите дату регистрации клиента");
            //if (_currentClient.PhotoPath == null)
            //    errors.AppendLine("Укажите фото клиента");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (ComboType.SelectedIndex == 0)
                _currentClient.GenderCode = "м";
            if (ComboType.SelectedIndex == 1)
                _currentClient.GenderCode = "ж";
            _currentClient.Birthday = ClientBirthday;

            if (_currentClient.ID == 0)
            {
                _currentClient.RegistrationDate = DateTime.Now;
                KhabibullinLanguageEntities.getInstance().Client.Add(_currentClient);
            }

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
