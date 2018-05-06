using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Common
{
    /// <summary>
    /// 值类型元数据，包括数据类型
    /// </summary>
    public class ValueTypeMeta
    {
        /// <summary>
        /// 数字类型
        /// </summary>
        public static int TYPE_NUMBER = 1;
        /// <summary>
        /// 字符串类型
        /// </summary>
        public static int TYPE_STRING = 2;
        /// <summary>
        ///  时间类型
        /// </summary>
        public static int TYPE_DATE = 3;
        /// <summary>
        ///  布尔类型
        /// </summary>
        public static int TYPE_BOOLEAN = 4;
        /// <summary>
        ///  整型类型
        /// </summary>
        public static int TYPE_INTEGER = 5;
        /// <summary>
        ///  大整型类型
        /// </summary>
        public static int TYPE_BIGNUMBER = 6;
        /// <summary>
        ///  序列化类型
        /// </summary>
        public static int TYPE_SERIALIZABLE = 7;
        /// <summary>
        ///  二进制类型
        /// </summary>
        public static int TYPE_BINARY = 8;
        /// <summary>
        ///  时间戳（微秒）类型
        /// </summary>
        public static int TYPE_TIMESTAMP = 9;
        /// <summary>
        ///  网络路径类型
        /// </summary>
        public static int TYPE_INET = 10;
        /// <summary>
        /// 本地文件路径
        /// </summary>
        public static int TYPE_LOCAL_FILE = 11;
        /// <summary>
        /// 本地mdb的要素类
        /// </summary>
        public static int TYPE_MDB_FEATURECLASS = 12;
        /// <summary>
        /// 本地gdb路径
        /// </summary>
        public static int TYPE_GDB_PATH = 13;
        /// <summary>
        /// The Constant typeCodes
        /// </summary>
        static String[] typeCodes = new String[] {
        "-", "Number", "String", "Date", "Boolean", "Integer", "BigNumber", "Serializable", "Binary",
            "Timestamp","Internet Address", "Local File Path", "Local MDB Featureclass",
            "Local GDB Path"};
        /// <summary>
        /// 获取类型的描述信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static String GetTypeDescription(int type)
        {
            try
            {
                return typeCodes[type];
            }
            catch (Exception)
            {
                return "unknown/illegal";
            }
        }
    }
}
