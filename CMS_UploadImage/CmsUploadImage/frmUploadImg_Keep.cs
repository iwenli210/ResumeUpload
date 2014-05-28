using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CmsUploadImage.Service;
using System.Threading;
using System.IO;

namespace CmsUploadImage
{
    public partial class frmUploadImg_Keep : Form
    {
        public event EventHandler ImgMangeWindowRefreshed;

        string _shopCode = "", _carID = "";
        public bool _isSuccess = false;
        readonly int _blockSize = 1024 * 60;//每块流大小
        bool _killThread = false;//是否结束线程
        List<Thread> _threadList = new List<Thread>();

        public frmUploadImg_Keep()
        {
            InitializeComponent();
        }

        public frmUploadImg_Keep(string batchID, string carID)
        {

            InitializeComponent();

            lab_remark.Tag = "9999";
            lab_remark.ForeColor = Color.Blue;

            this._shopCode = batchID;
            this._carID = carID;
            lab_CarID.Text = carID;

        }

        private void btn_Sel1_Click(object sender, EventArgs e)
        {
            //openFileDialog1 ff = new OpenFileDialog();

            this.openFileDialog1.Filter = "允许格式(*.JPG;*.GIF;*.PNG;*.BMP)|*.JPG;*.JPEG;*.GIF;*.PNG;*.BMP";//|所有文件(*.*)|*.* "(*.*)|*.*";
            this.openFileDialog1.Title = "选择数据源";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string ExistsImg = "";
                foreach (string img in openFileDialog1.FileNames)
                {
                    FileInfo file = new FileInfo(img);
                    long sizeM = (file.Length / (1024 * 1024));
                    if (sizeM > 5)
                    {
                        MessageBox.Show("图片：'" + img + "'超过规定大小，请处理后再上传!");
                        return;
                    }

                    bool existed = false;
                    foreach (Control c in panel1.Controls)
                    {
                        if (c is PictureBox && ((PictureBox)c).ImageLocation != null)
                        {
                            PictureBox pbtmp = new PictureBox();
                            pbtmp.ImageLocation = img;
                            if (((PictureBox)c).ImageLocation == img || CommonService.md5_hash(((PictureBox)c).ImageLocation) == CommonService.md5_hash(img))
                            {
                                existed = true;
                                break;
                            }
                        }
                    }
                    if (existed)
                    {
                        ExistsImg += img + Environment.NewLine; ;
                        continue;
                    }
                    foreach (Control c in panel1.Controls)
                    {
                        if (c is PictureBox && ((PictureBox)c).ImageLocation == null)
                        {
                            ((PictureBox)c).ImageLocation = img;
                            if (File.Exists(((PictureBox)c).ImageLocation))
                            {
                                ((PictureBox)c).Refresh();

                                string _name = "link_" + c.Name.Substring(c.Name.Length - 1);
                                foreach (Control x in panel1.Controls)
                                {
                                    if (x.Name.Equals(_name))
                                    {
                                        x.Visible = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("文件不存在！    ", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(ExistsImg))
                {
                    MessageBox.Show("无法添加:指定图片与现有图片相同.请重新指定图片.");
                }
            }
        }


        private void btn_Upload_Click(object sender, EventArgs e)
        {
            try
            {


                int _hasCount = 0;
                _killThread = false;

                foreach (Control c in panel1.Controls)
                {
                    if (c is PictureBox && ((PictureBox)c).ImageLocation != null)
                    {
                        _hasCount++;
                        Label _lab = new Label();
                        LinkLabel _linkLab = new LinkLabel();
                        string _name = "lab_Progress" + c.Name.Substring(c.Name.Length - 1);
                        string _linkName = "link_" + c.Name.Substring(c.Name.Length - 1);
                        foreach (Control x in panel1.Controls)
                        {
                            if (x.Name.Equals(_name))
                            {
                                _lab = (Label)x;
                            }
                            if (x.Name.Equals(_linkName))
                            {
                                _linkLab = (LinkLabel)x;
                            }
                        }
                        _lab.Text = "进度:0%";
                        _lab.Visible = true;
                        _linkLab.Visible = false;


                        //多线程
                        UploadThread utc = new UploadThread();

                        utc._blockSize = _blockSize;
                        utc._shopCode = _shopCode;
                        utc._carID = _carID;
                        UploadThread._killThread = _killThread;

                        //订阅事件
                        utc.successEvent += new EventHandler(utc_successEvent);
                        utc.failEvent += new EventHandler(utc_failEvent);
                        utc.hadExistEvent += new EventHandler(utc_hadExistEvent);
                        utc.progressEvent += new EventHandler(utc_progressEvent);
                        utc.stopEvent += new EventHandler(utc_stopEvent);

                        ParameterizedThreadStart ts = new ParameterizedThreadStart(utc.StartUpload);

                        Thread td = new Thread(ts);
                        td.IsBackground = true;
                        //td.SetApartmentState = ApartmentState.STA;
                        td.Start(((PictureBox)c).ImageLocation);
                        _threadList.Add(td);
                    }
                }

                if (_hasCount > 0)
                {
                    btn_Upload.Enabled = false;
                }
                else
                {
                    MessageBox.Show("请选择需要上传的图片!");
                    btn_Sel1.PerformClick();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //CarLog.WriteLog(ex.Message);
            }
        }




        #region 同步显示进度
        private delegate void viewMessageDelete(string path, string message);

        void utc_stopEvent(object sender, EventArgs e)
        {

        }

        void utc_progressEvent(object sender, EventArgs e)
        {
            string[] strArr = sender.ToString().Split('&');
            string _path = strArr[0];
            string _percent = strArr[1];

            viewMessageDelete view = new viewMessageDelete(ViewTextMessage);
            object[] obj = new object[2];
            obj[0] = _path;
            obj[1] = "进度:" + _percent;
            if (this.IsHandleCreated)
            {
                this.Invoke(view, obj);
            }
            //Thread.Sleep(5000);
        }

        void utc_hadExistEvent(object sender, EventArgs e)
        {
            string _path = sender.ToString();
            viewMessageDelete view = new viewMessageDelete(ViewTextMessage);
            object[] obj = new object[2];
            obj[0] = _path;
            obj[1] = "图片已存在";
            if (this.IsHandleCreated)
            {
                this.Invoke(view, obj);
            }

        }

        void utc_failEvent(object sender, EventArgs e)
        {
            string _path = sender.ToString();
            viewMessageDelete view = new viewMessageDelete(ViewTextMessage);
            object[] obj = new object[2];
            obj[0] = _path;
            obj[1] = "上传失败";
            if (this.IsHandleCreated)
            {
                this.Invoke(view, obj);
            }
        }

        void utc_successEvent(object sender, EventArgs e)
        {
            string _path = sender.ToString();
            viewMessageDelete view = new viewMessageDelete(ViewTextMessage);
            object[] obj = new object[2];
            obj[0] = _path;
            obj[1] = "上传成功";
            if (this.IsHandleCreated)
            {
                this.Invoke(view, obj);
            }
        }

        /// <summary>
        /// 显示进度
        /// </summary>
        /// <param name="_path"></param>
        /// <param name="message"></param>
        private void ViewTextMessage(string _path, string message)
        {
            foreach (Control c in panel1.Controls)
            {
                if (c is PictureBox && ((PictureBox)c).ImageLocation == _path)
                {
                    //找出对应的进度显示LABEL
                    Label _lab = new Label();
                    LinkLabel _linkLab = new LinkLabel();
                    string _name = "lab_Progress" + c.Name.Substring(c.Name.Length - 1);
                    string _linkName = "link_" + c.Name.Substring(c.Name.Length - 1);
                    foreach (Control x in panel1.Controls)
                    {
                        if (x.Name.Equals(_name))
                        {
                            _lab = (Label)x;
                        }
                        if (x.Name.Equals(_linkName))
                        {
                            _linkLab = (LinkLabel)x;
                        }
                    }

                    _lab.Text = message;
                    _lab.Visible = true;
                    _lab.Refresh();

                    _linkLab.Visible = false;
                    if (message.Contains("成功"))
                    {
                        if (ImgMangeWindowRefreshed != null)
                        {
                            ImgMangeWindowRefreshed(null, null);
                        }
                        ((PictureBox)c).ImageLocation = null;
                        ((PictureBox)c).Refresh();
                        _lab.Visible = false;
                    }
                    else if (message.Contains("已存在") || message.Contains("失败"))
                    {
                        _linkLab.Visible = true;
                    }
                    break;
                }
            }

            bool _isFinish = true;
            foreach (Control c in panel1.Controls)
            {
                if (c is Label && c.Name.Contains("Progress"))
                {
                    if (c.Visible && c.Text.Contains("进度"))
                    {
                        _isFinish = false;
                    }
                    if (message.Contains("已停止"))
                    {
                        c.Text = "已停止";
                    }
                }
            }
            if (_isFinish)
            {
                btn_Upload.Enabled = true;
            }
        }
        #endregion


        private void link_1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            ((LinkLabel)sender).Visible = false;
            string _labName = "lab_Progress" + ((LinkLabel)sender).Name.Substring(((LinkLabel)sender).Name.Length - 1);
            string _picName = "pic_" + ((LinkLabel)sender).Name.Substring(((LinkLabel)sender).Name.Length - 1);
            foreach (Control c in panel1.Controls)
            {
                if (c.Name.Equals(_picName))
                {
                    ((PictureBox)c).ImageLocation = null;
                    ((PictureBox)c).Refresh();
                }
                if (c.Name.Equals(_labName))
                {
                    ((Label)c).Visible = false;
                }
            }

            //pic_1.ImageLocation = null;
            //pic_1.Refresh();
            //link_1.Visible = false;
        }

        private void pic_1_Click(object sender, EventArgs e)
        {

            if (((PictureBox)sender).ImageLocation != null)
            {
                string _path = ((PictureBox)sender).ImageLocation;
                Image _img = Image.FromFile(_path);
                frmOriginal ff = new frmOriginal(_img);
                ff.ShowDialog();
                ff.Dispose();
            }
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {

            foreach (Thread th in _threadList)
            {
                if (th.IsAlive)
                {
                    th.Abort();
                }
            }
            UploadThread._killThread = true;
            _threadList.Clear();
            btn_Upload.Enabled = true;
        }


        private void frmUploadImg_Keep_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool _isFinish = true;
            foreach (Control c in panel1.Controls)
            {
                if (c is Label && c.Name.Contains("Progress"))
                {
                    if (c.Visible && c.Text.Contains("进度"))
                    {
                        _isFinish = false;
                    }
                }
            }
            if (!_isFinish)
            {
                if (MessageBox.Show(this, "还有未完成的上传任务，确定要退出吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            UploadThread._killThread = true;
        }

        private void frmUploadImg_Keep_Load(object sender, EventArgs e)
        {

        }



    }
}
