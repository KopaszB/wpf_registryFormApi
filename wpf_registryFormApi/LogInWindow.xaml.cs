using NetworkHelper;
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
using System.Windows.Shapes;

namespace wpf_registryFormApi
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        List<User> users = new List<User>();
        public LogInWindow()
        {
            InitializeComponent();
            ListaFeltolt();
        }
        private void ListaFeltolt()
        {
            string url = "http://localhost:3000/users";
            users = Backend.GET(url).Send().As<List<User>>();
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            MainWindow regWindow = new MainWindow();
            regWindow.Show();
            this.Close();
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            string email = userEmail.Text;
            string password = userPwd.Password;
            var user = users.FirstOrDefault(u => u.email == email && u.jelszo == password);
            if (user != null)
            {

                //MessageBox.Show("Sikeres bejelentkezés!");
                ActiveUser.BejelentkezettUser = user;
                UserDataWindow userDataWindow = new UserDataWindow();
                userDataWindow.Show();
                this.Close();
            }
            else if (userEmail.Text == "admin" && userPwd.Password == "admin")
            {
                AdminWindow adminWindow = new AdminWindow();
                adminWindow.Show();
                this.Close();
            }
            else if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Kérem töltse ki az összes mezőt!");
                return;
            }
            else if (!users.Any(u => u.email == email))
            {
                MessageBox.Show("Nincs ilyen email cím regisztrálva!");
                return;
            }
            else if (!users.Any(u => u.jelszo == password))
            {
                MessageBox.Show("Hibás jelszó!");
                return;
            }
            else
            {
                MessageBox.Show("Hibás email vagy jelszó!");
                return;
            }
            this.Close();
        }
    }
}
