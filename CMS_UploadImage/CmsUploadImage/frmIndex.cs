using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CmsUploadImage.Service;
using System.IO;
using System.Configuration;

namespace CmsUploadImage
{
    public partial class frmIndex : Form
    {
        string _shopCode = "";
        string _carID = "";
        DataTable imgList;

        public frmIndex()
        {
            InitializeComponent();
        }

        public frmIndex(string carID,string ShopCode,string url)
        {
            InitializeComponent();
            this._shopCode = ShopCode;
            this._carID = carID;
            Global.A_URL = url;
        }

        private void frmIndex_Load(object sender, EventArgs e)
        {
            lab_CarID.Text = _carID;

            //try
            //{
            //    Global.A_URL = ConfigurationManager.AppSettings["RemoteCarWebService"].ToString();
            //}
            //catch
            //{
            //    MessageBox.Show("文件异常,请联系车秘书!");
            //}
            if (string.IsNullOrEmpty(Global.A_URL))
            {
                MessageBox.Show("服务不存在,上传功能不可用,请联系车秘书!");
                this.Close();
                return;
            }

            LoadService();
        }

        private void LoadService()  
        {
            foreach (Control cmain in panel1.Controls)
            {
                if (cmain is Panel)
                {
                    foreach (Control c in cmain.Controls)
                    {
                        if (c is PictureBox && ((PictureBox)c).Image != null)
                        {
                            ((PictureBox)c).Image = null;
                            ((PictureBox)c).Tag = null;
                        }
                        if (c is CheckBox)
                        {
                            ((CheckBox)c).Checked = false;
                        }
                        c.Refresh();
                    }
                }
            }

            imgList = null;

            //dlService.GetAttachmentByBatchIDAsync(_batchID, _carID);
            //dlService.GetAttachmentByBatchIDCompleted += new Entity.CarWebService.GetAttachmentByBatchIDCompletedEventHandler(dlService_GetAttachmentByBatchIDCompleted);

            IUploadService us = FactoryService.CreateInstance();
            imgList = us.GetCarImages(_carID,_shopCode);
            if (imgList != null)
            {
                int row = 0;
                List<Control> cc = new List<Control>();
                foreach (Control cmain in panel1.Controls)
                {
                    cc.Add(cmain);
                }
                CommonService.QuickSort<Control>(cc, "TabIndex");
                foreach (Control cmain in cc)
                {
                    if (cmain is Panel)
                    {
                        if (row < imgList.Rows.Count)
                        {
                            DataRow dr = imgList.Rows[row];
                            if (DBNull.Value != dr["ThumbnailIMG"])
                            {
                                foreach (Control c in cmain.Controls)
                                {
                                    if (c is PictureBox && ((PictureBox)c).Image == null)
                                    {

                                        Stream s = new MemoryStream(Convert.FromBase64String(dr["ThumbnailIMG"].ToString()));
                                        ((PictureBox)c).Image = Image.FromStream(s);
                                        ((PictureBox)c).Tag = dr["ID"];

                                        toolTip1.SetToolTip(c, "点击查看原图");


                                    }
                                    else if (c is CheckBox)
                                    {
                                        CheckBox cb = (CheckBox)c;
                                        cb.Text = dr["CreateTime"].ToString();
                                    }

                                }
                            }
                            row++;
                        }
                    }
                }
            }

        }

        //void dlService_GetAttachmentByBatchIDCompleted(object sender, Entity.CarWebService.GetAttachmentByBatchIDCompletedEventArgs e)
        //{

        //    if (e.Error == null && imgList == null)
        //    {
        //        imgList = e.Result;
        //        if (e.Result != null)
        //        {
        //            int row = 0;
        //            List<Control> cc = new List<Control>();
        //            foreach (Control cmain in panel1.Controls)
        //            {
        //                cc.Add(cmain);
        //            }
        //            CommonService.QuickSort<Control>(cc, "TabIndex");
        //            foreach (Control cmain in cc)
        //            {
        //                if (cmain is Panel)
        //                {
        //                    if (row < e.Result.Rows.Count)
        //                    {
        //                        DataRow dr = e.Result.Rows[row];
        //                        if (DBNull.Value != dr["ThumbnailIMG"])
        //                        {
        //                            foreach (Control c in cmain.Controls)
        //                            {
        //                                if (c is PictureBox && ((PictureBox)c).Image == null)
        //                                {

        //                                    Stream s = new MemoryStream(Convert.FromBase64String(dr["ThumbnailIMG"].ToString()));
        //                                    ((PictureBox)c).Image = Image.FromStream(s);
        //                                    ((PictureBox)c).Tag = dr["ID"];

        //                                    toolTip1.SetToolTip(c, "点击查看原图");


        //                                }
        //                                else if (c is CheckBox)
        //                                {
        //                                    CheckBox cb = (CheckBox)c;
        //                                    cb.Text = dr["CreateTime"].ToString();
        //                                }

        //                            }
        //                        }
        //                        row++;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}


        private void btn_Upload_Click(object sender, EventArgs e)
        {
            if (imgList != null && imgList.Rows.Count >= 12)
            {
                MessageBox.Show("您上传的图片数量过多，请删除无用图片后再进行上传!");
                return;
            }


            frmUploadImg_Keep fui = new frmUploadImg_Keep(_shopCode, _carID);
            fui.ImgMangeWindowRefreshed += new EventHandler(fui_ImgMangeWindowRefreshed);
            fui.Show();
        }

        void fui_ImgMangeWindowRefreshed(object sender, EventArgs e)
        {
            btn_Refresh_Click(null, null);
        }

        //刷新
        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            LoadService();
        }

        private void pic_1_Click(object sender, EventArgs e)
        {
            int _id = Convert.ToInt32(((PictureBox)sender).Tag);
            frmOriginal foi = new frmOriginal(_id);
            foi.ShowDialog();
            foi.Dispose();
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            List<int> imgList = new List<int>();
            foreach (Control cmain in panel1.Controls)
            {
                if (cmain is Panel)
                {
                    bool isChecked = false;
                    foreach (Control c in cmain.Controls)
                    {
                        if (c is CheckBox && ((CheckBox)c).Checked)
                        {
                            isChecked = true;
                            break;
                        }
                    }
                    if (isChecked)
                    {
                        foreach (Control c in cmain.Controls)
                        {
                            if (c is PictureBox)
                            {
                                PictureBox pic = (PictureBox)c;
                                if (pic.Image != null)
                                {
                                    int id = Convert.ToInt32(pic.Tag);
                                    imgList.Add(id);
                                }
                            }
                        }
                    }
                }
            }
            if (imgList.Count > 0)
            {
                IUploadService us = FactoryService.CreateInstance();
                int _failCount = us.DeleteImage(imgList.ToArray());
                //int _failCount = dlService.RemoveImgsByThumbnail(imgList.ToArray());
                if (_failCount > 0)
                {
                    MessageBox.Show("删除完成，失败'" + _failCount + "'张");
                }
                else
                {
                    MessageBox.Show("删除完成");
                }
                LoadService();
            }
        }

        private void btn_Download_Click(object sender, EventArgs e)
        {
             try
            {
                folderBrowserDialog1.ShowDialog();
                if (Directory.Exists(folderBrowserDialog1.SelectedPath))
                {
                    IUploadService us = FactoryService.CreateInstance();

                    List<int> imgList = new List<int>();
                    foreach (Control cmain in panel1.Controls)
                    {
                        if (cmain is Panel)
                        {
                            bool needOutPut = false;
                            foreach (Control c in cmain.Controls)
                            {
                                if (c is CheckBox && ((CheckBox)c).Checked)
                                {
                                    needOutPut = true;
                                    break;
                                }
                            }
                            if (needOutPut)
                            {
                                foreach (Control c in cmain.Controls)
                                {
                                    if (c is PictureBox)
                                    {
                                        PictureBox pic = (PictureBox)c;
                                        if (pic.Image != null)
                                        {
                                            int picID = Convert.ToInt32(pic.Tag);
                                            
                                            byte[] BytImg = us.GetOriginalImage(picID);
                                            //byte[] BytImg = dlService.GetOriginalImageByThumbnail(picID);
                                            if (BytImg != null)
                                            {
                                                Stream s = new MemoryStream(BytImg);
                                                Image img = Image.FromStream(s);
                                                img.Save(folderBrowserDialog1.SelectedPath + "\\" + _carID + "_" + picID + ".jpg");

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                MessageBox.Show("导出完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
    }
}
