using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurir.DispatcherPages
{

    public class DispatcherHomeMDPageMenuItem
    {
        public DispatcherHomeMDPageMenuItem()
        {
            TargetType = typeof(DispatcherHomeMDPageDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}