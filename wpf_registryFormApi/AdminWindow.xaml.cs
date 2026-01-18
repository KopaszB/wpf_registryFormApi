using NetworkHelper;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace wpf_registryFormApi
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        List<User> users = new List<User>();
        public AdminWindow()
        {
            InitializeComponent();
            DataGridFeltolt();
        }
        private void DataGridFeltolt()
        {
            string url = "http://localhost:3000/users";
            users = Backend.GET(url).Send().As<List<User>>();
            usersDataGrid.ItemsSource = users;
        }

        private void linkExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void linkReg_Click(object sender, RoutedEventArgs e)
        {
            MainWindow regWindow = new MainWindow();
            regWindow.Show();
            this.Close();
        }

        private void linkLogin_Click(object sender, RoutedEventArgs e)
        {
            LogInWindow logInWindow = new LogInWindow();
            logInWindow.Show();
            this.Close();
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            userName.Text = ""; 
            userEmail.Text = "";
            userDate.Text = "";
            userPassword.Text = "";
            DataGridFeltolt();
        }

        private void usersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User selectedUser = usersDataGrid.SelectedItem as User;
            if (selectedUser != null)
            {
                userName.Text = selectedUser.nev;
                userEmail.Text = selectedUser.email;
                userDate.Text = selectedUser.szul_datum;
                userPassword.Text = selectedUser.jelszo;
            }
            else return;
        }

        private void btn_modify_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = usersDataGrid.SelectedItem as User;
            // van-e kiválasztott user?
            if (selectedUser == null)
            {
                MessageBox.Show("Válassz ki egy felhasználót a táblázatból!");
                return;
            }

            // egyszerű validáció
            if (string.IsNullOrWhiteSpace(userName.Text) ||
                string.IsNullOrWhiteSpace(userEmail.Text) ||
                string.IsNullOrWhiteSpace(userDate.Text))
            {
                MessageBox.Show("A név, e-mail és dátum mező kötelező!");
                return;
            }

            // frissített adatok összeállítása
            var updatedUser = new
            {
                nev = userName.Text,
                email = userEmail.Text,
                szul_datum = userDate.Text,
                jelszo = userPassword.Text
            };

            string url = $"http://localhost:3000/users/{selectedUser.id}";

            try
            {
                Backend.PUT(url)
                       .Body(updatedUser)
                       .Send();

                MessageBox.Show("Sikeres módosítás!");
                DataGridFeltolt(); // frissítés
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt a módosítás során:\n" + ex.Message);
            }
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = usersDataGrid.SelectedItem as User;
            // van-e kiválasztott user?
            if (selectedUser == null)
            {
                MessageBox.Show("Válassz ki egy felhasználót a táblázatból!");
                return;
            }
            // 2. Megerősítés
            MessageBoxResult confirm = MessageBox.Show(
                $"Biztosan törlöd ezt a felhasználót?\n\nNév: {selectedUser.nev}\nE-mail: {selectedUser.email}",
                "Törlés megerősítése",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm != MessageBoxResult.Yes)
                return;

            // 3. DELETE kérés
            string url = $"http://localhost:3000/users/{selectedUser.id}";

            try
            {
                Backend.DELETE(url)
                       .Send();

                MessageBox.Show("Felhasználó sikeresen törölve!");
                DataGridFeltolt(); // frissítés
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt a törlés során:\n" + ex.Message);
            }
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            // 1. Egyszerű validáció
            if (string.IsNullOrWhiteSpace(userName.Text) ||
                string.IsNullOrWhiteSpace(userEmail.Text) ||
                string.IsNullOrWhiteSpace(userDate.Text) ||
                string.IsNullOrWhiteSpace(userPassword.Text))
            {
                MessageBox.Show("Minden mezőt kötelező kitölteni!");
                return;
            }

            // 2. Új felhasználó objektum összeállítása
            var newUser = new
            {
                nev = userName.Text,
                email = userEmail.Text,
                szul_datum = userDate.Text,
                jelszo = userPassword.Text
            };

            string url = "http://localhost:3000/users";

            try
            {
                Backend.POST(url)
                       .Body(newUser)
                       .Send();

                MessageBox.Show("Felhasználó sikeresen hozzáadva!");

                // 3. Mezők ürítése
                userName.Text = "";
                userEmail.Text = "";
                userDate.Text = "";
                userPassword.Text = "";

                // 4. DataGrid frissítése
                DataGridFeltolt();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt a hozzáadás során:\n" + ex.Message);
            }
        }
    }
}
