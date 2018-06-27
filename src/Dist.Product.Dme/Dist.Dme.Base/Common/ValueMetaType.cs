using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dist.Dme.Base.Common
{
    /// <summary>
    /// 值类型枚举
    /// </summary>
    public enum ValueMetaType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知，unknown")]
        TYPE_UNKNOWN = 0,
        /// <summary>
        /// 数字类型
        /// </summary>
        [Description("简单数字类型，Number")]
        TYPE_NUMBER = 1,
        /// <summary>
        /// 简单的字符串类型
        /// </summary>
        [Description("简单字符串类型，String")]
        TYPE_STRING = 2,
        /// <summary>
        /// 时间类型
        /// </summary>
        [Description("日期类型，Date")]
        TYPE_DATE = 3,
        /// <summary>
        /// 布尔类型
        /// </summary>
        [Description("布尔类型，Boolean")]
        TYPE_BOOLEAN = 4,
        /// <summary>
        /// 短整数型
        /// </summary>
        [Description("短整数，Integer")]
        TYPE_INTEGER = 5,
        /// <summary>
        /// 大整型类型
        /// </summary>
        [Description("大整型，Big Number")]
        TYPE_BIGNUMBER = 6,
        /// <summary>
        /// 序列化类型
        /// </summary>
        [Description("序列化，Serializable")]
        TYPE_SERIALIZABLE = 7,
        /// <summary>
        /// 二进制类型
        /// </summary>
        [Description("二进制流，Binary")]
        TYPE_BINARY = 8,
        /// <summary>
        /// 毫秒类型
        /// </summary>
        [Description("时间戳，毫秒")]
        TYPE_TIMESTAMP = 9,
        /// <summary>
        /// 网络路径类型
        /// </summary>
        [Description("网络路径")]
        TYPE_INET = 10,
        /// <summary>
        /// 本地文件类型
        /// </summary>
        [Description("本地文件类型")]
        TYPE_LOCAL_FILE = 11,
        /// <summary>
        /// 本地MDB要素类
        /// </summary>
        [Description("本地MDB要素类")]
        TYPE_MDB_FEATURECLASS = 12,
        /// <summary>
        /// 本地GDB路径
        /// </summary>
        [Description("GDB类型，file gdb or personal gdb")]
        TYPE_GDB_PATH = 13,
        /// <summary>
        /// 文件夹类型，包括文件夹路径
        /// </summary>
        [Description("文件夹类型")]
        TYPE_FOLDER = 14,
        /// <summary>
        /// 字符串列表，多值列表
        /// </summary>
        [Description("字符串列表，多值列表")]
        TYPE_STRING_LIST = 15,
        /// <summary>
        /// SDE数据源中的要素类
        /// </summary>
        [Description("SDE数据源中的要素类")]
        TYPE_SDE_FEATURECLASS = 16,
        /// <summary>
        /// 泛型要素类
        /// </summary>
        [Description("泛型要素类")]
        TYPE_FEATURECLASS  =17,
        /// <summary>
        /// JSON对象
        /// </summary>
        [Description("JSON对象")]
        TYPE_JSON = 18

    }
}
