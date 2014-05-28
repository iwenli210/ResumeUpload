using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CmsUploadImage.Service;

namespace CmsUploadImage
{
    public partial class frmOriginal : Form
    {
        int _thumbnailID = 0;
        public frmOriginal()
        {
            InitializeComponent();
        }

        public frmOriginal(Image img)
        {
            InitializeComponent();

            pictureBox1.Image = img;

            this.Width = img.Width;
            this.Height = img.Height;

            if (img.Width > 800 && img.Height > 600)
            {
                this.Width = 800;
                this.Height = 600;
            }
        }

        public frmOriginal(int thumbnailID)
        {
            InitializeComponent();

            this._thumbnailID = thumbnailID;

        }

        private void frmOriginalImg_Load(object sender, EventArgs e)
        {
            //this.pic_Loading.ImageLocation = "Skin\\SysImg\\loading.gif";

            if (_thumbnailID != 0)
            {
                pic_Loading.Visible = true;

                //dlService.GetOriginalImageByThumbnailAsync(_thumbnailID);
                //dlService.GetOriginalImageByThumbnailCompleted += new Entity.CarWebService.GetOriginalImageByThumbnailCompletedEventHandler(dlService_GetOriginalImageByThumbnailCompleted);

                IUploadService us = FactoryService.CreateInstance();
                 byte[] imgByte = us.GetOriginalImage(_thumbnailID);
                 if (imgByte != null)
                 {
                     Stream s = new MemoryStream(imgByte);
                     Image _pic = Image.FromStream(s);
                     pictureBox1.Image = _pic;


                     this.Width = _pic.Width;
                     this.Height = _pic.Height;

                     if (_pic.Width > 800 && _pic.Height > 600)
                     {
                         this.Width = 800;
                         this.Height = 600;
                     }

                     pic_Loading.Visible = false;
                 }

            }
        }

        //byte[] imgByte = null;
        //void dlService_GetOriginalImageByThumbnailCompleted(object sender, Entity.CarWebService.GetOriginalImageByThumbnailCompletedEventArgs e)
        //{
        //    if (e.Error == null && imgByte==null)
        //    {
        //        imgByte = e.Result;
        //        if (imgByte != null)
        //        {
        //            Stream s = new MemoryStream(imgByte);
        //            Image _pic = Image.FromStream(s);
        //            pictureBox1.Image = _pic;


        //            this.Width = _pic.Width;
        //            this.Height = _pic.Height;

        //            if (_pic.Width > 800 && _pic.Height > 600)
        //            {
        //                this.Width = 800;
        //                this.Height = 600;
        //            }

        //            pic_Loading.Visible = false;
        //        }
        //    }
        //}
    }
}
