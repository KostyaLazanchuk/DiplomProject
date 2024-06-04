using Diplom.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApplication.ModelsAPI
{
    public class NodeCreateAPI
    {
        [FromForm(Name = "name")]
        public string Name { get; set; }

        [FromForm(Name = "position")]
        public int Position { get; set; }
    }
}
