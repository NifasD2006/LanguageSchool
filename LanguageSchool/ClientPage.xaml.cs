using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace LanguageSchool
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        //Создать переменные которые посчитают количество записей.
        int CountRecords; //Кол-во запесей в таблице
        int CountPage; // Кол-во страниц
        int CurrentPage;//Текущая страница
        List<Client> CurrentPageList= new List<Client>(); //Текущая страница
        List<Client> TableList;
        List<Client> TableList2;//лист, содержащий все записи, все сортировки/фильтры/поиски применяются к данной переменной.
        int selectIndexOutoutCB = 0;
        public ClientPage()
        {
            InitializeComponent();
            var currentClients = SafinLaunguageEntities.GetContext().Client.ToList();
            ClietnLV.ItemsSource = currentClients;
            OutputCB.SelectedIndex = 0;

            UpdateClients();
        }

        private void UpdateClients()
        {
            var currentClients = SafinLaunguageEntities.GetContext().Client.ToList();
            TableList2 = currentClients;

            if (SortCB.SelectedIndex == 1)
            {
                currentClients = currentClients.OrderBy(p => p.LastName).ToList();
            }
            if (SortCB.SelectedIndex == 2)
            {
                currentClients = currentClients
                    .OrderByDescending(p =>
                    {
                        // Укажите формат даты, который используется в строке LastDate
                        string format = "dd.MM.yyyy"; // Пример формата

                        if (DateTime.TryParseExact(p.LastVisit, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastVisit))
                        {
                            return lastVisit;
                        }
                        else
                        {
                            // Обработка ошибки: не удалось преобразовать дату
                            // Например, можно вернуть DateTime.MinValue или DateTime.MaxValue
                            return DateTime.MinValue;
                        }
                    })
                    .ToList();
            }

            if (SortCB.SelectedIndex == 3)
            {
                currentClients = currentClients.OrderByDescending(p => p.VisitCount).ToList();
            }






            if (GenderCB.SelectedIndex == 2)
            {
                currentClients = currentClients.Where(p => p.GenderString.ToString() == "Мужской").ToList();
            }
            if (GenderCB.SelectedIndex == 1)
            {
                currentClients = currentClients.Where(p => p.GenderString.ToString() == "Женский").ToList();
            }
            
            currentClients = currentClients.Where(p => p.LastName.ToLower().Contains(TBoxSearch.Text.ToLower())|| p.FirstName.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Patronymic.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Phone.Replace("+", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Contains(TBoxSearch.Text.ToLower())).ToList();

            ClietnLV.ItemsSource = currentClients.ToList();
            TableList = currentClients;
            
            ChangePage(0, 0);
        }
        private void ChangePage(int direction, int? selectedPage)
        {
            int n = selectIndexOutoutCB;
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            
            if (CountRecords % n > 0)
            {
                CountPage = CountRecords / n + 1;
            }
            else
            {
                CountPage = CountRecords / n;
            }

            Boolean Ifupdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * n + n < CountRecords ? CurrentPage * n + n : CountRecords;
                    for (int i = CurrentPage * n; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
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
                            if(CurrentPage < 0)
                            {
                                CurrentPage = 0;
                            }
                            min = CurrentPage * n + n < CountRecords ? CurrentPage * n + n : CountRecords;
                            for (int i = CurrentPage * n; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
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
                            if (CurrentPage > 10)
                            {
                                CurrentPage = 0;
                            }
                            min = CurrentPage * n + n < CountRecords ? CurrentPage * n + n : CountRecords;
                            for (int i = CurrentPage * n; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
/*                    default:
                        if (selectedPage < 0)
                        {
                            selectedPage = 0;
                        }
                        CurrentPage = (int)selectedPage;
                        min = CurrentPage * n + n < CountRecords ? CurrentPage * n + n : CountRecords;
                        for (int i = CurrentPage * n; i < min; i++)
                        {
                            CurrentPageList.Add(TableList[i]);
                        }
                        break;*/
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

                min = CurrentPage * n + n < CountRecords ? CurrentPage * n + n : CountRecords;
                TBCount.Text = TableList.Count().ToString();
                TBAllRecords.Text = TableList2.Count().ToString();
                ClietnLV.ItemsSource = CurrentPageList;
                ClietnLV.Items.Refresh();
            }
        }
        private void LeftBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {

            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        
        private void OutputCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OutputCB.SelectedItem != null)
            {
                // Получаем выбранный элемент
                TextBlock selectedTextBlock = OutputCB.SelectedItem as TextBlock;

                if (selectedTextBlock != null)
                {
                    // Получаем текст выбранного элемента
                    string selectedText = selectedTextBlock.Text;

                    // Выполняем действия в зависимости от выбранного текста
                    switch (selectedText)
                    {
                        case "10":
                            selectIndexOutoutCB = 10;
                            break;
                        case "50":
                            selectIndexOutoutCB = 50;
                            break;
                        case "200":
                            selectIndexOutoutCB = 200;
                            break;
                        case "Все":
                            selectIndexOutoutCB = CurrentPageList.Count;
                            break;
                        default:
                            // Обработка неизвестного выбора
                            break;
                    }
                }

            }
            UpdateClients();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients();
        }

        private void GenderCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void SortCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentClients = (sender as Button).DataContext as Client;

            var currentClientService = SafinLaunguageEntities.GetContext().ClientService.ToList();
            currentClientService = currentClientService.Where(p => p.ClientID == currentClients.ID).ToList();
            if (currentClientService.Count != 0)
            {
                MessageBox.Show("Не возможно выполнить удаление так, как есть записи клиентов на эту услугу.");
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        SafinLaunguageEntities.GetContext().Client.Remove(currentClients);
                        SafinLaunguageEntities.GetContext().SaveChanges();
                        ClietnLV.ItemsSource = SafinLaunguageEntities.GetContext().Service.ToList();
                        UpdateClients();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }
    }
}
