﻿using Dist.Dme.Base.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Interfaces
{
    /// <summary>
    /// 业务服务接口
    /// </summary>
    public interface IBizService
    {
        /// <summary>
        /// 是否一个业务guid，格式如：97e1dd1f6c664128b93815f56256d0f1
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Boolean IsBizGuid(string code);
    }
}
