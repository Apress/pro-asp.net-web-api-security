using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenericIdentityWinForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            btnDelete.Enabled = Thread.CurrentPrincipal.IsInRole("Admin");
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")]
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Do some important admin stuff here
        }
    }
}
