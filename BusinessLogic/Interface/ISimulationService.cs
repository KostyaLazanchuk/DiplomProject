﻿using Diplom.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface ISimulationService
    {
        double EvaluateNetworkReliability(List<Node> nodes, int iterations);
    }
}
