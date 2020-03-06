using System;

namespace Monetizr
{
    public class ListProduct
    {
        public string Name { get; private set; }
        public string Tag { get; private set; }
        public bool Active { get; private set; }
        public bool Claimable { get; private set; }
    }
}