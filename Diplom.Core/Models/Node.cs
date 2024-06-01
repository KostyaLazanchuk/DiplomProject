using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Core.Models
{
    public class Node
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Position { get; set; }
        public string Color { get; set; }
        public List<Edge>? Edge { get; set; }
    }
}
