﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework
{
    public interface IStepMeta
    {
        void SetDefault();
        void SaveMeta(int stepId, int modelId, int versionId);
    }
}
