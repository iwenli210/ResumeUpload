using System;
using System.Collections.Generic;
using System.Text;

namespace CmsUploadImage.Service
{
    public class FactoryService
    {
        public static IUploadService CreateInstance()
        {
            IUploadService us = null;

            if (Global.WebType == "A")
                us = new UploadService();
            else if (Global.WebType == "S")
                us = new S_UploadService();

            return us;
        }
    }
}
