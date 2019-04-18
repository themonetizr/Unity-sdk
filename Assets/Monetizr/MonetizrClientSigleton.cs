using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SingletonPattern
{
    public class MonetizrClient : Singleton<MonetizrMonoBehaviour>
    {
        public MonetizrClient()
        {
        }
    }
}
