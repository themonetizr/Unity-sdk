using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ShopifyCountries
{
    public static ShopifyCountry FromShortCode(string shortCode)
    {
        return Collection.FirstOrDefault(p => p.countryShortCode == shortCode);
    }
    
    public static ShopifyCountry FromName(string name)
    {
        return Collection.FirstOrDefault(p => p.countryName == name);
    }

    public static ShopifyCountry.Region FromShortCode(ShopifyCountry country, string shortCode)
    {
        return country.regions.FirstOrDefault(p => p.shortCode == shortCode);
    }
    
    public static ShopifyCountry.Region FromName(ShopifyCountry country, string name)
    {
        return country.regions.FirstOrDefault(p => p.name == name);
    }

    #region Build Collection

    private static List<ShopifyCountry> _collection;

    public static List<ShopifyCountry> Collection
    {
        get
        {
            if (_collection == null)
            {
                var json = Resources.Load<TextAsset>("MTZ-country-data");
                var countries = JsonUtility.FromJson<JsonCountries>(json.text);
                _collection = countries.countries;
            }
            return _collection;
        }
    }
    #endregion
}

/// <summary>
/// Representation of an ShopifyCountries-1 Country
/// </summary>
[Serializable]
public class ShopifyCountry
{
    public string countryName;
    public string countryShortCode;
    public List<Region> regions;

    [Serializable]
    public class Region
    {
        public string name;
        public string shortCode;
    }
}

[Serializable]
public class JsonCountries
{
    public List<ShopifyCountry> countries;
}