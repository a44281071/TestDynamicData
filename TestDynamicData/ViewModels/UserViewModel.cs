using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace TestDynamicData
{
    public class UserViewModel : Screen
    {
        public UserViewModel(UserInfo info)
        {
            Info = info;
        }

        public UserInfo Info { get; }
        public Guid Id => Info.Id;

        public bool IsMicOn { get; set; }
        public bool IsCamOn { get; set; }
    }
}