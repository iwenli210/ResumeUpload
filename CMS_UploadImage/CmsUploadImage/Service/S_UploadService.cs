using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CmsUploadImage.Service
{
    public class S_UploadService : IUploadService
    {
        public DataTable GetCarImages(string carID, string shopCode)
        {
            return null;
        }

        public byte[] GetOriginalImage(int thumbnailID)
        {
            return null;
        }

        public string KeepUpload(Byte[] fileBytes, string carID, long allSize, string md5, string tempPath, long PerLength, string shopCode)
        {
            return "";
        }

        public int DeleteImage(int[] idList)
        {
            return 0;
        }
    }
}
