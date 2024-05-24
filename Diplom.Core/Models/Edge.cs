using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Core.Models
{
    public class Edge
    {
        public Guid Id { get; set; }
        public int Weight { get; set; }
        public Guid? EndNode { get; set; }
    }
}
