using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurir.UserPages
{

    public class UserHomePageMenuItem
    {
        public UserHomePageMenuItem()
        {
            //TargetType = typeof(UserHomePageMenuItemDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}