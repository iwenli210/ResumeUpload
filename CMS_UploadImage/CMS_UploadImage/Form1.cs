using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CMS_UploadImage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            CmsUploadImage.frmIndex f = new CmsUploadImage.frmIndex("粤A12345", "1234", "http://192.168.17.129/CarWebService/DealListService.asmx");
            f.ShowDialog();
        }
    }
}
