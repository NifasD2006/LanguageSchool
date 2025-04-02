using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.Win32;

namespace LanguageSchool
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Client _currentClient = new Client();

        public AddEditPage(Client SelectedClient)
        {
            InitializeComponent();
            if(SelectedClient != null)
            {
                _currentClient = SelectedClient;
                if (SelectedClient.GenderCode == "м")
                {
                    ManRB.IsChecked = true;
                }
                if (SelectedClient.GenderCode == "ж")
                {
                    WomanRB.IsChecked= true;
                }
            }
            else
            {
                IDTextBlock.Visibility = Visibility.Hidden;
                TextBoxID.Visibility = Visibility.Hidden;
            }
            DataContext= _currentClient;

        }
        public event Action OnReturningBack;
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentClient.FirstName))
                errors.AppendLine("Укажите имя пользователя");
            else
            {
                if(_currentClient.FirstName.Length > 50)
                    errors.AppendLine("Имя должно быть не больше 50 символов");
                else
                    if (!System.Text.RegularExpressions.Regex.IsMatch(_currentClient.FirstName, @"^[а-яА-ЯёЁa-zA-Zs-]+$"))
                {
                    errors.AppendLine("Имя может содержать только буквы, пробелы и дефисы");
                }
            }
            if (string.IsNullOrWhiteSpace(_currentClient.LastName))
                errors.AppendLine("Укажите фамилию пользователя");
            else
            {
                if (_currentClient.LastName.Length > 50)
                    errors.AppendLine("Фамилия должно быть не больше 50 символов");
                else
                    if (!System.Text.RegularExpressions.Regex.IsMatch(_currentClient.LastName, @"^[а-яА-ЯёЁa-zA-Zs-]+$"))
                {
                    errors.AppendLine("Фамилия может содержать только буквы, пробелы и дефисы");
                }
            }
            if (string.IsNullOrWhiteSpace(_currentClient.Patronymic))
                errors.AppendLine("Укажите отчество пользователя");
            else
            {
                if (_currentClient.Patronymic.Length > 50)
                    errors.AppendLine("Отчество должно быть не больше 50 символов");
                else
                    if (!System.Text.RegularExpressions.Regex.IsMatch(_currentClient.Patronymic, @"^[а-яА-ЯёЁa-zA-Zs-]+$"))
                {
                    errors.AppendLine("Отчество может содержать только буквы, пробелы и дефисы");
                }
            }
            if (string.IsNullOrWhiteSpace(_currentClient.Email))
                errors.AppendLine("Укажите email");
            else
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(_currentClient.Email,@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+\.[a-zA-Z]{2,}$"))
                {
                    errors.AppendLine("Укажите корректный email");
                }
            }

            if (string.IsNullOrWhiteSpace(_currentClient.Phone))
                errors.AppendLine("Укажите телефон");
            else
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(_currentClient.Phone, @"^[\d\s()+-]+$"))
                {
                    errors.AppendLine("Телефон может содержать только цифры, пробелы, '+', '-', '(', ')'");
                }
            }
            if (_currentClient.Birthday == null)
                errors.AppendLine("Укажите дату рождения");
            else
            {
                DateTime eighteenYearsAgo = DateTime.Today.AddYears(-18);
                if (_currentClient.Birthday > eighteenYearsAgo)
                {
                    errors.AppendLine("Пользователь должен быть совершеннолетним (18 лет и старше)");
                }
            }
            if(ManRB.IsChecked!=true && WomanRB.IsChecked!=true )
                errors.AppendLine("Укажите пол");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (ManRB.IsChecked==true)
            {
                _currentClient.GenderCode = "м";
            }
            if (WomanRB.IsChecked == true)
            {
                _currentClient.GenderCode = "ж";
            }

            _currentClient.RegistrationDate = DateTime.Now;

            if (_currentClient.ID == 0)
            {
                SafinLaunguageEntities.GetContext().Client.Add(_currentClient);
            }
            try
            {
                SafinLaunguageEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }






        private void EditPhotoBtn_Click(object sender, RoutedEventArgs e)
        {
            // 1. Определяем путь к папке "Клиенты"
            //string clientsDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Клиенты");
            string clientsDirectory = @"D:\УАТ\3 Курс\Разработка программных модулей Тимашева\Языковая школа\LanguageSchool\LanguageSchool\Клиенты"; // Замените на ваш полный путь
            // 2. Открываем OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            openFileDialog.InitialDirectory = clientsDirectory;  // Устанавливаем начальную папку

            if (Directory.Exists(clientsDirectory)) // Проверяем, существует ли папка
            {
                openFileDialog.InitialDirectory = clientsDirectory;
            }
            else
            {
                MessageBox.Show("Папка 'Клиенты' не найдена. Укажите путь вручную.");
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory; // Или другой путь по умолчанию
            }

            if (openFileDialog.ShowDialog() == true)
            {
                string git = "Клиенты\\";
                // 2. Получаем полный путь к выбранному файлу
                string fullPath = openFileDialog.FileName;
                if (!fullPath.Contains(git))
                {
                    MessageBox.Show("Выбирите фотографию из папки Клиенты! >:(");
                    return;
                }
                // 3. Получаем имя файла (относительный путь)
                string filename = System.IO.Path.GetFileName(fullPath);

                // 4. Сохраняем относительный путь (имя файла) в свойстве PhotoPath
                _currentClient.PhotoPath = git + filename;
                ClientPhotoImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

    }
}
