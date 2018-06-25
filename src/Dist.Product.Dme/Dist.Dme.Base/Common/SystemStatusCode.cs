namespace Dist.Dme.Base.Common
{
    /// <summary>
    /// 系统状态码
    /// </summary>
    public class SystemStatusCode
    {
        /// <summary>
        ///  业务成功状态码 
        /// </summary>
        public const int DME_SUCCESS = 1000;
        /// <summary>
        /// 业务失败状态码
        /// </summary>
        public const int DME_FAIL = 2000;
        /// <summary>
        /// 业务上初始化未完成
        /// </summary>
        public const int DME_FAIL_INIT = 2001;
        /// <summary>
        /// 业务错误状态码
        /// </summary>
        public const int DME_ERROR = 3000;
    }
}
