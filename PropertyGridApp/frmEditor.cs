using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PropertyGridApp
{
    public partial class frmEditor : Form
    {

        private string _value;
        public string value { get { return _value; } set { _value = value; textBox1.Text = _value; } }

        public frmEditor()
        {
            InitializeComponent();
        }

        private void frmEditor_Load(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _value = textBox1.Text;
        }
    }
}
