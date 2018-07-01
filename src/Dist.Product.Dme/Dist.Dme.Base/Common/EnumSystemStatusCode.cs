using System.ComponentModel;

namespace Dist.Dme.Base.Common
{
    /// <summary>
    /// 系统状态码
    /// </summary>
    public enum SystemStatusCode
    {
        [Description("运行中")]
        [DisplayName("running")]
        DME_RUNNING = 0,
        [Description("已停止")]
        [DisplayName("stop")]
        DME_STOP = 1,
        /// <summary>
        ///  业务成功状态码 
        /// </summary>
        [Description("业务成功状态码")]
        [DisplayName("success")]
        DME_SUCCESS = 1000,
        /// <summary>
        /// 业务失败状态码
        /// </summary>
        [Description("业务失败状态码")]
        [DisplayName("fail")]
        DME_FAIL = 2000,
        /// <summary>
        /// 业务上初始化未完成
        /// </summary>
        [Description("业务上初始化未完成")]
        [DisplayName("fail_init")]
        DME_FAIL_INIT = 2001,
        /// <summary>
        /// 业务错误状态码
        /// </summary>
        [Description("业务错误状态码")]
        [DisplayName("error")]
        DME_ERROR = 3000
    }
}
