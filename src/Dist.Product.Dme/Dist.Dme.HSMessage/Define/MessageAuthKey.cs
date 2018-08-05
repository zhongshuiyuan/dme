using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.HSMessage.Define
{
    /// <summary>
    /// 消息验证和唯一key
    /// </summary>
    public class MessageAuthKey
    {
        /// <summary>
        /// app验证key
        /// </summary>
        public string AppKey { get; set; }
        /// <summary>
        /// app唯一id，可以是其用户的 ID，用户帐号名，或者 Email，总之任何一个能唯一地标识其用户的都可以
        /// </summary>
        public string AppId{ get; set; }
        /// <summary>
        /// 客户端类型（设备类型）
        /// </summary>
        public string AgentType { get; set; }

        /// <summary>
        /// 覆写相等方法
        /// </summary>
        /// <param name="desObj"></param>
        /// <returns></returns>
        public override bool Equals(object desObj)
        {
            MessageAuthKey desMessageAuthKey = (MessageAuthKey)desObj;
            if (null == desMessageAuthKey)
            {
                return false;
            }
            if (Object.Equals(this.AppKey, desMessageAuthKey.AppKey) && Object.Equals(this.AppId, desMessageAuthKey.AppId))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 重写GetHashCode方法（重写Equals方法必须重写GetHashCode方法，否则发生警告
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.AppKey.GetHashCode() + this.AppId.GetHashCode();
        }
    }
}
