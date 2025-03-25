using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Data.Sql;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace GelecekNot
{
    public partial class TimedMessage : Form
    {
        private System.Timers.Timer timer;
        private SqlConnection databaseConnection;
        

        public TimedMessage()
        {
            InitializeComponent();
            InitalizeDatabaseConnection();
            RetrieveMessageAndInterval();  
            this.FormBorderStyle = FormBorderStyle.None;
            notBox.Text = "Mesaj giriniz."; 
            notBox.ForeColor = SystemColors.GrayText; 
            notBox.GotFocus += NotBox_GotFocus;
            notBox.LostFocus += NotBox_LostFocus;
        }

        
        private void NotBox_GotFocus(object sender, EventArgs e)
        {
            if (notBox.Text == "Mesaj giriniz.")
            {
                notBox.Text = "";
                notBox.ForeColor = SystemColors.WindowText;
            }
        }

        private void NotBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(notBox.Text))
            {
                notBox.Text = "Mesaj giriniz.";
                notBox.ForeColor = SystemColors.GrayText; 
            }
        }

        private void InitalizeDatabaseConnection()
        {
            string connectionString = "Server=DESKTOP-32BP985\\SQLEXPRESS;Database=futureNote;Integrated Security=True;";
            databaseConnection = new SqlConnection(connectionString);
            try
            {
                databaseConnection.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Veritabanı bağlantı hatası: " + ex.Message);
            }
        }

        private void sayacButton_Click(object sender, EventArgs e)
        {
            

            DateTime selectedDateTime = dateTimePicker1.Value;
            TimeSpan difference = selectedDateTime - DateTime.Now;

            if (difference.TotalMilliseconds > 0)
            {
                SaveMessageAndInterval(notBox.Text, selectedDateTime);
                System.Timers.Timer messageTimer = new System.Timers.Timer(difference.TotalMilliseconds);
                messageTimer.Elapsed += (src, evt) =>
                {
                    messageTimer.Stop();
                    label1.Invoke(new Action(() =>
                    {
                        label1.Text = notBox.Text;
                        label1.Visible = true;
                        
                    }));
                };
                messageTimer.Start();
            }
            else
            {
                MessageBox.Show("Geçerli bir tarih ve saat seçiniz.");
            }
        }


        private void SaveMessageAndInterval(string message, DateTime display_date)
        {
            string query = "INSERT INTO future_notess (message, display_date) VALUES (@Message, @DisplayDate)";
            using (SqlCommand command = new SqlCommand(query, databaseConnection))
            {
                command.Parameters.AddWithValue("@Message", message);
                command.Parameters.AddWithValue("@DisplayDate", display_date);
                try
                {
                    databaseConnection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Veritabanına veri ekleme hatası: " + ex.Message);
                }
                finally
                {
                    databaseConnection.Close();
                }
            }
        }

        


        private void RetrieveMessageAndInterval()
        {
            try
            {
                if (databaseConnection.State == ConnectionState.Closed)
                {
                    databaseConnection.Open();
                }

                string query = "SELECT TOP 1 message, display_date FROM future_notess ORDER BY id DESC";
                using (SqlCommand command = new SqlCommand(query, databaseConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            notBox.Text = reader.GetString(reader.GetOrdinal("message"));
                            dateTimePicker1.Value = reader.GetDateTime(reader.GetOrdinal("display_date"));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Veritabanından veri alınırken bir hata oluştu: " + ex.Message);
            }
            finally
            {
                if (databaseConnection.State == ConnectionState.Open)
                {
                    databaseConnection.Close();
                }
            }
        }

        private void cikisButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ListForm listForm = new ListForm();
            DataTable pastNotes = GetPastNotesFromDatabase();
            DataTable futureNotes = GetFutureNotesFromDatabase();
            listForm.SetPastNotes(pastNotes);
            listForm.SetFutureNotes(futureNotes);
            listForm.Show();
            this.Hide();

        }
        private DataTable GetPastNotesFromDatabase()
        {
            string connectionString = "Server=DESKTOP-32BP985\\SQLEXPRESS;Database=futureNote;Integrated Security=True;";
            DataTable pastNotes = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT message, display_date FROM future_notess WHERE display_date < GETDATE()";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            pastNotes.Load(reader);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Veritabanından geçmiş notlar alınırken bir hata oluştu: " + ex.Message);
            }
            return pastNotes;
        }
        private DataTable GetFutureNotesFromDatabase()
        {
            string connectionString = "Server=DESKTOP-32BP985\\SQLEXPRESS;Database=futureNote;Integrated Security=True;";
            DataTable futureNotes = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT message, display_date FROM future_notess WHERE display_date >= GETDATE()";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            futureNotes.Load(reader);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Veritabanından gelecek notlar alınırken bir hata oluştu: " + ex.Message);
            }
            return futureNotes;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            notBox.Text = "Mesaj giriniz.";
            notBox.ForeColor = SystemColors.GrayText;
            dateTimePicker1.Value = DateTime.Today;
            label1.Visible = false;
        }
    }
}
