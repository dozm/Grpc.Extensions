﻿using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Client
{
    public interface ILoadBalancer
    {
        Channel GetChannel();

    }
}