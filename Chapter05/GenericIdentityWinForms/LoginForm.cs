using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenericIdentityWinForms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Authenticate using this.txtUserId and this.txtPassword

            Thread.CurrentPrincipal = new GenericPrincipal
            (
                new GenericIdentity(this.txtUserId.Text),
                new[] { "General User", "Admin" }
            );

            this.DialogResult = DialogResult.OK;
            this.Close();

        }
    }
}
