using NetworkHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace wpf_registryFormApi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
     
    public partial class MainWindow : Window
    {
        List<User> users = new List<User>();

        public MainWindow()
        {
            InitializeComponent();
            DataGridFeltolt();
        }
        private void DataGridFeltolt()
        {
            string url = "http://localhost:3000/all";
            users = Backend.GET(url).Send().As<List<User>>();
            usersTable.ItemsSource = users;
        }

        private void userEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            string email = userEmail.Text;

            if (IsValidEmail(email))
            {
                userEmail.BorderBrush = Brushes.Green;
                //MessageBox.Show("Érvényes e-mail cím!");
            }
            else
            {
                userEmail.BorderBrush = Brushes.Red;
                MessageBox.Show("Érvénytelen e-mail cím!");
            }
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
        private void userDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (userDate.SelectedDate == null)
                return; // még nincs kiválasztva dátum

            DateTime date = userDate.SelectedDate.Value;

            // Nem lehet jövőbeli dátum
            if (date > DateTime.Today)
            {
                MessageBox.Show("A dátum nem lehet a jövőben!");
                userDate.SelectedDate = null;
            }
        }

        private void userPwd1_LostFocus(object sender, RoutedEventArgs e)
        {
            string password = userPwd1.Password;

            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$"))
            {
                userPwd1.BorderBrush = Brushes.Green;
                MessageBox.Show("Érvényes jelszó!");
            }
            else
            {
                userPwd1.BorderBrush = Brushes.Red;
                MessageBox.Show("A jelszónak legalább 8 karakter hosszúnak kell lennie, és tartalmaznia kell legalább egy nagybetűt, egy kisbetűt és egy számot.");
            }
        }

        private void userPwd2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (userPwd1.Password == userPwd2.Password)
            {
                userPwd2.BorderBrush = Brushes.Green;
                //MessageBox.Show("A jelszavak megegyeznek!");
            }
            else
            {
                userPwd2.BorderBrush = Brushes.Red;
                MessageBox.Show("A jelszavak nem egyeznek meg!");
            }
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            string email = userEmail.Text.Trim().ToLower();
            bool emailExists = users.Any(u => u.email.Trim().ToLower() == email);

            if (userName.Text != null && userEmail.Text != null && userDate.SelectedDate.Value != null && userPwd1.Password != null && userPwd2.Password != null && conditions.IsChecked == true)
            {
                if (!emailExists)
                {
                    try
                    {
                        string url = "http://localhost:3000/users";
                        User newUser = new User()
                        {
                            nev = userName.Text.Trim(),
                            email = userEmail.Text.Trim(),
                            szul_datum = userDate.SelectedDate?.ToString("yyyy-MM-dd"),
                            jelszo = userPwd1.Password.Trim()
                        };
                        string response = Backend.POST(url).Body(newUser).Send().As<string>();
                        users.Add(newUser);
                        MessageBox.Show(response);
                        DataGridFeltolt();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hiba! {ex}");
                    }
                }
                else
                {
                    MessageBox.Show("Ez az e-mail cím már regisztrálva van!");
                }
            }
            else 
            {
                MessageBox.Show("Kérem töltse ki az összes mezőt és fogadja el a feltételeket!");
                return;
            }
            
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            LogInWindow logInWindow = new LogInWindow();
            logInWindow.Show();
            this.Hide();
        }
    }
}
