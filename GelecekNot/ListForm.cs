using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GelecekNot
{
    public partial class ListForm : Form
    {
        public ListForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
        }
        public void SetPastNotes(DataTable pastNotes)
        {
            dataGridView1.DataSource = pastNotes;
        }
        public void SetFutureNotes(DataTable futureNotes)
        {
            dataGridView2.DataSource = futureNotes;
        }
        private void backButton_Click(object sender, EventArgs e)
        {
            TimedMessage timedMessageForm = new TimedMessage();
            timedMessageForm.Show();
            this.Hide();
        }
    }
}
