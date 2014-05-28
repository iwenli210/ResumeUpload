using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CmsUploadImage.Service
{
    public class UploadService : IUploadService
    {

        public string KeepUpload(Byte[] fileBytes, string carID, long allSize, string md5, string tempPath, long PerLength,string shopCode)
        {
            object[] args = new object[7];
            args[0] = fileBytes;
            args[1] = carID;
            args[2] = allSize;
            args[3] = md5;
            args[4] = tempPath;
            args[5] = PerLength;
            args[6] = shopCode;


            object re = CallWebService.InvokeWebService(Global.A_URL, Global.A_Func_Upload, args);
            return (string)re;
        }

        public DataTable GetCarImages(string carID, string shopCode)
        {
            string[] args = new string[2];
            args[0] = carID;
            args[1] = shopCode;
            object re = CallWebService.InvokeWebService(Global.A_URL, Global.A_Func_GetCarImages, args);
            return (DataTable)re;
        }

        public byte[] GetOriginalImage(int thumbnailID)
        {
            object[] args = new object[1];
            args[0] = thumbnailID;
            object re = CallWebService.InvokeWebService(Global.A_URL, Global.A_Func_GetOriginal, args);
            return (byte[])re;
        }

        public int DeleteImage(int[] idList)
        {
            object[] args = new object[1];
            args[0] = idList;
            object re = CallWebService.InvokeWebService(Global.A_URL, Global.A_Func_Remove, args);
            return (int)re;
        }
    }
}
