using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CmsUploadImage.Service
{
    public interface IUploadService
    {
        //获取全部已上传图片
        DataTable GetCarImages(string CarID, string shopCode);

        //获取原图
        byte[] GetOriginalImage(int thumbnailID);

        //断点续传
        string KeepUpload(Byte[] fileBytes, string carID, long allSize, string md5, string tempPath, long PerLength, string shopCode);

        //删除图片
        int DeleteImage(int[] idList);
    }
}
