using System;
using System.Collections.Generic;
using System.Xml;
using Monetizr.Dto;

namespace Monetizr
{
    public class ListProduct
    {
        public string Name { get; private set; }
        public string Tag { get; private set; }
        public bool Active { get; private set; }
        public bool Claimable { get; private set; }
        public Product.DownloadableImage Thumbnail { get; private set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Tag: {1}, Active: {2}, Claimable: {3}", Name, Tag, Active, Claimable);
        }

        public static ListProduct FromDto(ProductListDto.Line p)
        {
            return new ListProduct
            {
                Name = p.name,
                Tag = p.product_tag,
                Active = p.is_active,
                Claimable = p.claimable,
                Thumbnail = new Product.DownloadableImage(p.product_thumbnail)
            };
        }
    }
}

namespace Monetizr.Dto
{
    [Serializable]
    public class ProductListDto
    {
        [Serializable]
        public class Line
        {
            public string name;
            public string product_tag;
            public bool is_active;
            public bool claimable;
            public string product_thumbnail;
        }

        public List<Line> array;
    }
}