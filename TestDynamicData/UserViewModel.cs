using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDynamicData
{
    public class UserViewModel : Screen
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsMicOn { get; set; }
        public bool IsCamOn { get; set; }
    }
}
