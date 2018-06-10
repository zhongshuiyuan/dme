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
        /// The Constant typeCodes
        /// </summary>
        static String[] typeCodes = new String[] {
        "-", "Number", "String", "Date", "Boolean", "Integer", "BigNumber", "Serializable", "Binary",
            "Timestamp","Internet Address", "Local File Path", "Local MDB Featureclass",
            "Local GDB Path", "Millisecond", "Folder", "ListOfString, multiple value", "The Featureclass From SDE",
        "A Feature Class"};
        /// <summary>
        /// 获取类型的描述信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static String GetTypeDescription(ValueTypeEnum type)
        {
            try
            {
                return typeCodes[(int)type];
            }
            catch (Exception)
            {
                return "unknown/illegal";
            }
        }
    }
    /// <summary>
    /// 值类型枚举
    /// </summary>
    public enum ValueTypeEnum
    {
        /// <summary>
        /// 未知
        /// </summary>
        TYPE_UNKNOWN = 0,
        /// <summary>
        /// 数字类型
        /// </summary>
        TYPE_NUMBER = 1,
        /// <summary>
        /// 简单的字符串类型
        /// </summary>
        TYPE_STRING = 2,
        /// <summary>
        /// 时间类型
        /// </summary>
        TYPE_DATE = 3,
        /// <summary>
        /// 布尔类型
        /// </summary>
        TYPE_BOOLEAN = 4,
        /// <summary>
        /// 短整数型
        /// </summary>
        TYPE_INTEGER = 5,
        /// <summary>
        /// 大整型类型
        /// </summary>
        TYPE_BIGNUMBER = 6,
        /// <summary>
        /// 序列化类型
        /// </summary>
        TYPE_SERIALIZABLE = 7,
        /// <summary>
        /// 二进制类型
        /// </summary>
        TYPE_BINARY = 8,
        /// <summary>
        /// 微秒类型
        /// </summary>
        TYPE_TIMESTAMP = 9,
        /// <summary>
        /// 网络路径类型
        /// </summary>
        TYPE_INET = 10,
        /// <summary>
        /// 本地文件类型
        /// </summary>
        TYPE_LOCAL_FILE = 11,
        /// <summary>
        /// 本地MDB要素类
        /// </summary>
        TYPE_MDB_FEATURECLASS = 12,
        /// <summary>
        /// 本地GDB路径
        /// </summary>
        TYPE_GDB_PATH = 13,
        /// <summary>
        /// 毫秒类型
        /// </summary>
        TYPE_MILLISECOND = 14,
        /// <summary>
        /// 文件夹类型，包括文件夹路径
        /// </summary>
        TYPE_FOLDER = 15,
        /// <summary>
        /// 字符串列表，多值列表
        /// </summary>
        TYPE_STRING_LIST = 16,
        /// <summary>
        /// SDE数据源中的要素类
        /// </summary>
        TYPE_SDE_FEATURECLASS = 17,
        /// <summary>
        /// 泛型要素类
        /// </summary>
        TYPE_FEATURECLASS  =18

    }
}
