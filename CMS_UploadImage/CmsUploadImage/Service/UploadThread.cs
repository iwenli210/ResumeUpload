using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CmsUploadImage.Service
{
    public class UploadThread
    {
         /// <summary>
        /// 开始事件
        /// </summary>
        public event EventHandler stopEvent;

        /// <summary>
        /// 已存在事件
        /// </summary>
        public event EventHandler hadExistEvent;

        /// <summary>
        /// 上传成功事件
        /// </summary>
        public event EventHandler successEvent;

        /// <summary>
        /// 上传失败事件
        /// </summary>
        public event EventHandler failEvent;


        /// <summary>
        /// 进度显示事件
        /// </summary>
        public event EventHandler progressEvent;


        public int _blockSize { get; set; }

        public string _shopCode { get; set; }

        public string _carID { get; set; }

        public static bool _killThread { get; set; }

        //Entity.CarWebService.DealListService dlService = FactoryService.CreateWebService();

        /// <summary>
        /// 上传方法
        /// </summary>
        /// <param name="imgPath"></param>
        public void StartUpload(object imgPath)
        {
            //开始上传
            string _imgPath = imgPath.ToString();//((PictureBox)c).ImageLocation;
            try
            {

                long _currentSize = 0;
                string _tempFilePath = "";
                FileStream fStream = new FileStream(_imgPath, FileMode.Open, FileAccess.Read);
                long _allSize = fStream.Length;//文件总大小
                string _md5 = CommonService.md5_hash(_imgPath);//MD5码

                while (_currentSize < _allSize)
                {
                    //判断是否关闭线程
                    if (_killThread)
                    {
                        stopEvent.Invoke(_imgPath, null);
                        break;
                    }

                    Byte[] _blockArr = null;
                    if (_currentSize + _blockSize <= _allSize)
                    {
                        _blockArr = new Byte[_blockSize];
                    }
                    else
                    {
                        _blockArr = new Byte[_allSize - _currentSize];
                    }

                    fStream.Seek(_currentSize, SeekOrigin.Begin);
                    fStream.Read(_blockArr, 0, _blockArr.Length);

                    //上传
                    string _initInfo = "-1:";
                    try
                    {
                        IUploadService us = FactoryService.CreateInstance();
                        _initInfo = us.KeepUpload(_blockArr, _carID, _allSize, _md5, _tempFilePath, _currentSize,_shopCode);//dlService.UploadFile_Keep(_blockArr, _batchID, _carID, _allSize, _md5, _tempFilePath, _currentSize);
                    }
                    catch (Exception ex)
                    {
                        _initInfo += ex.Message;

                    }

                    if (_initInfo.Substring(0, 2) == ("2:"))
                    {
                        //开始续传
                        string[] reArr = _initInfo.Split('&');
                        _tempFilePath = reArr[1];
                        _currentSize = int.Parse(reArr[2]);

                        string _percent = Convert.ToInt32(_currentSize * 100 / _allSize) + "%";
                        progressEvent.Invoke(_imgPath + "&" + _percent, null);
                        continue;
                    }
                    else if (_initInfo.Substring(0, 2) == ("0:"))
                    {
                        //该图片已上传过
                        //hadExistEvent.Invoke(_imgPath, null);
                        successEvent.Invoke(_imgPath, null);
                        break;
                    }
                    else if (_initInfo.Substring(0, 2) == ("-1"))
                    {
                        //上传失败
                        //CarLog.WriteLog("上传图片失败:" + _initInfo);
                        failEvent.Invoke(_imgPath, null);
                        break;
                    }
                    else if (_initInfo.Substring(0, 2) == ("1:"))
                    {
                        //上传成功
                        successEvent.Invoke(_imgPath, null);
                        break;
                    }
                    else if (_initInfo.Substring(0, 2) == ("-3"))
                    {
                        //上传工作已被接手,本线程退出
                        break;
                    }
                }
                fStream.Flush();
                fStream.Close();
            }
            catch 
            {
                //CarLog.WriteLog(ex);
                failEvent.Invoke(_imgPath, null);
            }
        }
    }
}
