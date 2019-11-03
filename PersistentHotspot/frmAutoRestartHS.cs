using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersistentHotspot
{
    public partial class frmAutoRestartHS : Form
    {
        public frmAutoRestartHS()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Program.Reg.auto_restart_hotspot = (int)nudMins.Value;
            this.Close();
        }
    }
}
