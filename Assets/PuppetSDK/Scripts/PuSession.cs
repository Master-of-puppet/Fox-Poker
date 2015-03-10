using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puppet
{
    public class PuSession : BaseSingleton<PuSession>
    {
        public string c_userName;
        public string c_password;

        protected override void Init()
        {
        }


    }
}
