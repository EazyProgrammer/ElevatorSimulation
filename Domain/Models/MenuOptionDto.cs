using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MenuOptionDto
    {
        public string Name { get; }
        public Action Selected { get; }

        public MenuOptionDto(string name, Action selected)
        {
            Name = name;
            Selected = selected;
        }
    }
}
