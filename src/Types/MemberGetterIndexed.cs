﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Types
{
    public delegate object MemberGetterIndexed(object instance, object index);
    public delegate T MemberGetterIndexed<T>(object instance, object index);
}
