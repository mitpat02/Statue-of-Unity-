using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private SqlConnection conn;
        private string conString = "Server=(local);Database=RegisteredUserDB;" +
                  "User=Tick2023;Password=Tick123";
        private SqlCommand cmd;

        public MainWindow()
        {
            InitializeComponent();
            // Initialize the SqlConnection
            conn = new SqlConnection(conString);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string email = tbEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter an email address.");
                return;
            }

            bool isRegistered = CheckUserRegistration(email);

            if (!isRegistered)
            {
                Register_Click(email);
                MessageBox.Show("Registration Sucessful");
            }
            else
            {
                MessageBox.Show("User is already registered");
            } 
        }
       

        public void Button_Click(object sender, RoutedEventArgs e)
        {
             string name = tbName.Text.Trim();
            string mobile = tbMobile.Text.Trim();
            string email = tbEmail.Text.Trim();
            int noOfTickets;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(email) || !int.TryParse(tbQtyTicket.Text, out noOfTickets))
            {
                MessageBox.Show("All fields should be filled and numbe rof quanitiy should be more than 1 non-negative number please");
                return;
            }

            //this will calculate total amount of tickets
            decimal totalAmount = CalculateTotalAmount(noOfTickets, email);
            

            // Check if the user is registered or not
            bool isRegistered = CheckUserRegistration(email);
            string discountMessage = isRegistered ? "10% will be deducted from your total\n" : "";

            lblRegistersuccess.Content = $"{discountMessage}";
               textblockBill.Text =  $"Thank you for shopping , {name}!\n---------------------------------------\n"
                                    +$"Amount with tax is: {totalAmount:C}\n";

            SaveBookingDetails(name, mobile, email, noOfTickets, totalAmount);
        }

     


        //this method will calculate total amount customer has to paid
        //also checks is users is registeerd user or not
        private decimal CalculateTotalAmount(int noOfTickets, string email)
        {
            // Replace this with your actual ticket price calculation logic
            decimal ticketPrice = 100.0m;

            // Check if the user is registered and apply the discount if applicable
            bool isRegistered = CheckUserRegistration(email);
            decimal discount;

            if (isRegistered)
            {
                discount = 0.1m; // 10% discount -> registered users
            }
            else
            {
                discount = 0.0m; // regular users -> 0% discount
            }
            decimal tax = 0.13M;
            decimal amount = ticketPrice * noOfTickets * (1 - discount);
            decimal amountwithTax = amount * tax;
            decimal totalAmount = amount + amountwithTax;
            return totalAmount;
        }
        private bool CheckUserRegistration(string email)
        {
            bool isRegistered = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM RegisteredUser WHERE Email = @Email";
                    using (cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            isRegistered = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking user registration: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return isRegistered;
        }
        private void SaveBookingDetails(string name, string mobile, string email, int noOfTickets, decimal totalAmount)
        {
            try
            {
                conn.Open();

                string query = "INSERT INTO TicketBooking (Name, Mobile, Email, NumberOfTickets, TotalAmount) " +
                               "VALUES (@Name, @Mobile, @Email, @NumberOfTickets, @TotalAmount)";
                using (cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Mobile", mobile);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@NumberOfTickets", noOfTickets);
                    cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving booking details: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void Register_Click(string email)
        {
            try
            {
                conn.Open();

                string query = "INSERT INTO RegisteredUser (Email) VALUES (@Email)";
                using (cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error registering user: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

     
    }
}
