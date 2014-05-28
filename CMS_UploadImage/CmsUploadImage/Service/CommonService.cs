using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace CmsUploadImage.Service
{
    public class CommonService
    {
        /// <summary>
        /// 获取文件MD5
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string md5_hash(string path)
        {
            FileStream get_file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hash_byte = get_md5.ComputeHash(get_file);
            string resule = System.BitConverter.ToString(hash_byte);
            resule = resule.Replace("-", "");
            get_file.Close();
            return resule;
        }

        #region 快速排序
        public static void QuickSort<T>(IList<T> ints, string orderBy)
        {
            System.Reflection.PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            PropertyInfo pi = null;
            foreach (PropertyInfo p in myPropertyInfo)
            {
                if (p.Name.Equals(orderBy))
                {
                    pi = p;
                    break;
                }
            }
            if (pi == null)
            {
                throw new Exception("无效属性" + orderBy);
            }
            QuickSort<T>(ints, 0, ints.Count - 1, pi, new DeleCompareAB<T>(CompareAMoreThenB));
        }
        public static void QuickSortDesc<T>(IList<T> ints, string orderBy)
        {
            System.Reflection.PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            PropertyInfo pi = null;
            foreach (PropertyInfo p in myPropertyInfo)
            {
                if (p.Name.Equals(orderBy))
                {
                    pi = p;
                    break;
                }
            }
            if (pi == null)
            {
                throw new Exception("无效属性" + orderBy);
            }
            QuickSort<T>(ints, 0, ints.Count - 1, pi, new DeleCompareAB<T>(CompareALessThenB));
        }
        static void QuickSort<T>(IList<T> ints, int i, int j, PropertyInfo pi, DeleCompareAB<T> CompareAB)
        {
            T tmp;
            int start = i, end = j;
            bool add = true;

            while (i < j)
            {
                if (add)
                {
                    if (CompareAB(ints, i, j, pi))

                    //if ((ints[i]) > ints[j])
                    {
                        tmp = ints[i];
                        ints[i] = ints[j];
                        ints[j] = tmp;
                        add = false;
                        j--;
                    }
                    else
                        i++;
                }
                else
                {
                    if (CompareAB(ints, i, j, pi))
                    {
                        tmp = ints[i];
                        ints[i] = ints[j];
                        ints[j] = tmp;
                        add = true;
                        i++;
                    }
                    else
                        j--;
                }

            }
            if (i > start + 1)
            {
                QuickSort(ints, start, i - 1, pi, CompareAB);

            } if (i < end - 1)
            {
                QuickSort(ints, i + 1, end, pi, CompareAB);
            }
        }
        delegate bool DeleCompareAB<T>(IList<T> ints, int i, int j, PropertyInfo pi);
        /// <summary>
        /// 第i个T的pi的值大于第j个T的pi的值?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ints"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        private static bool CompareAMoreThenB<T>(IList<T> ints, int i, int j, PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(DateTime?))
            {
                return Convert.ToDateTime(pi.GetValue(ints[i], null)) > Convert.ToDateTime(pi.GetValue(ints[j], null));
            }
            else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(double) || pi.PropertyType == typeof(float))
            {
                return Convert.ToDouble(pi.GetValue(ints[i], null)) > Convert.ToDouble(pi.GetValue(ints[j], null));
            }
            else
                return string.Compare((string)pi.GetValue(ints[i], null), (string)pi.GetValue(ints[j], null)) > 0;
        }
        /// <summary>
        /// 第j个T的pi的值大于第i个T的pi的值?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ints"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        private static bool CompareALessThenB<T>(IList<T> ints, int i, int j, PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(DateTime?))
            {
                return Convert.ToDateTime(pi.GetValue(ints[i], null)) < Convert.ToDateTime(pi.GetValue(ints[j], null));
            }
            else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(double) || pi.PropertyType == typeof(float))
            {
                return Convert.ToDouble(pi.GetValue(ints[i], null)) < Convert.ToDouble(pi.GetValue(ints[j], null));
            }
            else
                return string.Compare((string)pi.GetValue(ints[i], null), (string)pi.GetValue(ints[j], null)) < 0;
        }
        #endregion
    }
}
