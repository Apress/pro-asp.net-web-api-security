using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenericIdentityWinForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DialogResult result = DialogResult.None;
            using (var loginForm = new LoginForm())
                result = loginForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                var permission = new PrincipalPermission(null, "General User");
                permission.Demand();

                Application.Run(new MainForm());
            }

        }
    }
}
