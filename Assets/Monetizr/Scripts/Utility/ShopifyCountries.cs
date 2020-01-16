using System.Collections.ObjectModel;
using System.Linq;

// Quick and dirty but flexible solution
// https://stackoverflow.com/a/35295634

public static class ShopifyCountries
{
    /// <summary>
    /// Obtain ShopifyCountries-1 Country based on its alpha2 code.
    /// </summary>
    /// <param name="alpha2"></param>
    /// <returns></returns>
    public static ShopifyCountry FromAlpha2(string alpha2)
    {
        return Collection.FirstOrDefault(p => p.Alpha2 == alpha2);
    }
    
    /// <summary>
    /// Obtain ShopifyCountries-1 Country based on its full name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static ShopifyCountry FromName(string name)
    {
        return Collection.FirstOrDefault(p => p.Name == name);
    }

    #region Build Collection

    private static Collection<ShopifyCountry> _collection;

    public static Collection<ShopifyCountry> Collection
    {
        get
        {
            if (_collection == null)
            {
                _collection = new Collection<ShopifyCountry>();
                _collection.Add(new ShopifyCountry("Afghanistan", "AF"));
                _collection.Add(new ShopifyCountry("Aland Islands", "AX"));
                _collection.Add(new ShopifyCountry("Albania", "AL"));
                _collection.Add(new ShopifyCountry("Algeria", "DZ"));
                _collection.Add(new ShopifyCountry("Andorra", "AD"));
                _collection.Add(new ShopifyCountry("Angola", "AO"));
                _collection.Add(new ShopifyCountry("Anguilla", "AI"));
                _collection.Add(new ShopifyCountry("Antigua And Barbuda", "AG"));
                _collection.Add(new ShopifyCountry("Argentina", "AR"));
                _collection.Add(new ShopifyCountry("Armenia", "AM"));
                _collection.Add(new ShopifyCountry("Aruba", "AW"));
                _collection.Add(new ShopifyCountry("Australia", "AU"));
                _collection.Add(new ShopifyCountry("Austria", "AT"));
                _collection.Add(new ShopifyCountry("Azerbaijan", "AZ"));
                _collection.Add(new ShopifyCountry("Bahamas", "BS"));
                _collection.Add(new ShopifyCountry("Bahrain", "BH"));
                _collection.Add(new ShopifyCountry("Bangladesh", "BD"));
                _collection.Add(new ShopifyCountry("Barbados", "BB"));
                _collection.Add(new ShopifyCountry("Belarus", "BY"));
                _collection.Add(new ShopifyCountry("Belgium", "BE"));
                _collection.Add(new ShopifyCountry("Belize", "BZ"));
                _collection.Add(new ShopifyCountry("Benin", "BJ"));
                _collection.Add(new ShopifyCountry("Bermuda", "BM"));
                _collection.Add(new ShopifyCountry("Bhutan", "BT"));
                _collection.Add(new ShopifyCountry("Bolivia", "BO"));
                _collection.Add(new ShopifyCountry("Bosnia And Herzegovina", "BA"));
                _collection.Add(new ShopifyCountry("Botswana", "BW"));
                _collection.Add(new ShopifyCountry("Bouvet Island", "BV"));
                _collection.Add(new ShopifyCountry("Brazil", "BR"));
                _collection.Add(new ShopifyCountry("British Indian Ocean Territory", "IO"));
                _collection.Add(new ShopifyCountry("Brunei", "BN"));
                _collection.Add(new ShopifyCountry("Bulgaria", "BG"));
                _collection.Add(new ShopifyCountry("Burkina Faso", "BF"));
                _collection.Add(new ShopifyCountry("Burundi", "BI"));
                _collection.Add(new ShopifyCountry("Cambodia", "KH"));
                _collection.Add(new ShopifyCountry("Canada", "CA"));
                _collection.Add(new ShopifyCountry("Cape Verde", "CV"));
                _collection.Add(new ShopifyCountry("Caribbean Netherlands", "BQ"));
                _collection.Add(new ShopifyCountry("Cayman Islands", "KY"));
                _collection.Add(new ShopifyCountry("Central African Republic", "CF"));
                _collection.Add(new ShopifyCountry("Chad", "TD"));
                _collection.Add(new ShopifyCountry("Chile", "CL"));
                _collection.Add(new ShopifyCountry("China", "CN"));
                _collection.Add(new ShopifyCountry("Christmas Island", "CX"));
                _collection.Add(new ShopifyCountry("Cocos (Keeling) Islands", "CC"));
                _collection.Add(new ShopifyCountry("Colombia", "CO"));
                _collection.Add(new ShopifyCountry("Comoros", "KM"));
                _collection.Add(new ShopifyCountry("Congo", "CG"));
                _collection.Add(new ShopifyCountry("Congo, The Democratic Republic Of The", "CD"));
                _collection.Add(new ShopifyCountry("Cook Islands", "CK"));
                _collection.Add(new ShopifyCountry("Costa Rica", "CR"));
                _collection.Add(new ShopifyCountry("Croatia", "HR"));
                _collection.Add(new ShopifyCountry("Cuba", "CU"));
                _collection.Add(new ShopifyCountry("Curaçao", "CW"));
                _collection.Add(new ShopifyCountry("Cyprus", "CY"));
                _collection.Add(new ShopifyCountry("Czech Republic", "CZ"));
                _collection.Add(new ShopifyCountry("Côte d'Ivoire", "CI"));
                _collection.Add(new ShopifyCountry("Denmark", "DK"));
                _collection.Add(new ShopifyCountry("Djibouti", "DJ"));
                _collection.Add(new ShopifyCountry("Dominica", "DM"));
                _collection.Add(new ShopifyCountry("Dominican Republic", "DO"));
                _collection.Add(new ShopifyCountry("Ecuador", "EC"));
                _collection.Add(new ShopifyCountry("Egypt", "EG"));
                _collection.Add(new ShopifyCountry("El Salvador", "SV"));
                _collection.Add(new ShopifyCountry("Equatorial Guinea", "GQ"));
                _collection.Add(new ShopifyCountry("Eritrea", "ER"));
                _collection.Add(new ShopifyCountry("Estonia", "EE"));
                _collection.Add(new ShopifyCountry("Eswatini", "SZ"));
                _collection.Add(new ShopifyCountry("Ethiopia", "ET"));
                _collection.Add(new ShopifyCountry("Falkland Islands (Malvinas)", "FK"));
                _collection.Add(new ShopifyCountry("Faroe Islands", "FO"));
                _collection.Add(new ShopifyCountry("Fiji", "FJ"));
                _collection.Add(new ShopifyCountry("Finland", "FI"));
                _collection.Add(new ShopifyCountry("France", "FR"));
                _collection.Add(new ShopifyCountry("French Guiana", "GF"));
                _collection.Add(new ShopifyCountry("French Polynesia", "PF"));
                _collection.Add(new ShopifyCountry("French Southern Territories", "TF"));
                _collection.Add(new ShopifyCountry("Gabon", "GA"));
                _collection.Add(new ShopifyCountry("Gambia", "GM"));
                _collection.Add(new ShopifyCountry("Georgia", "GE"));
                _collection.Add(new ShopifyCountry("Germany", "DE"));
                _collection.Add(new ShopifyCountry("Ghana", "GH"));
                _collection.Add(new ShopifyCountry("Gibraltar", "GI"));
                _collection.Add(new ShopifyCountry("Greece", "GR"));
                _collection.Add(new ShopifyCountry("Greenland", "GL"));
                _collection.Add(new ShopifyCountry("Grenada", "GD"));
                _collection.Add(new ShopifyCountry("Guadeloupe", "GP"));
                _collection.Add(new ShopifyCountry("Guatemala", "GT"));
                _collection.Add(new ShopifyCountry("Guernsey", "GG"));
                _collection.Add(new ShopifyCountry("Guinea", "GN"));
                _collection.Add(new ShopifyCountry("Guinea Bissau", "GW"));
                _collection.Add(new ShopifyCountry("Guyana", "GY"));
                _collection.Add(new ShopifyCountry("Haiti", "HT"));
                _collection.Add(new ShopifyCountry("Heard Island And Mcdonald Islands", "HM"));
                _collection.Add(new ShopifyCountry("Holy See (Vatican City State)", "VA"));
                _collection.Add(new ShopifyCountry("Honduras", "HN"));
                _collection.Add(new ShopifyCountry("Hong Kong", "HK"));
                _collection.Add(new ShopifyCountry("Hungary", "HU"));
                _collection.Add(new ShopifyCountry("Iceland", "IS"));
                _collection.Add(new ShopifyCountry("India", "IN"));
                _collection.Add(new ShopifyCountry("Indonesia", "ID"));
                _collection.Add(new ShopifyCountry("Iran, Islamic Republic Of", "IR"));
                _collection.Add(new ShopifyCountry("Iraq", "IQ"));
                _collection.Add(new ShopifyCountry("Ireland", "IE"));
                _collection.Add(new ShopifyCountry("Isle Of Man", "IM"));
                _collection.Add(new ShopifyCountry("Israel", "IL"));
                _collection.Add(new ShopifyCountry("Italy", "IT"));
                _collection.Add(new ShopifyCountry("Jamaica", "JM"));
                _collection.Add(new ShopifyCountry("Japan", "JP"));
                _collection.Add(new ShopifyCountry("Jersey", "JE"));
                _collection.Add(new ShopifyCountry("Jordan", "JO"));
                _collection.Add(new ShopifyCountry("Kazakhstan", "KZ"));
                _collection.Add(new ShopifyCountry("Kenya", "KE"));
                _collection.Add(new ShopifyCountry("Kiribati", "KI"));
                _collection.Add(new ShopifyCountry("Korea, Democratic People's Republic Of", "KP"));
                _collection.Add(new ShopifyCountry("Kosovo", "XK"));
                _collection.Add(new ShopifyCountry("Kuwait", "KW"));
                _collection.Add(new ShopifyCountry("Kyrgyzstan", "KG"));
                _collection.Add(new ShopifyCountry("Lao People's Democratic Republic", "LA"));
                _collection.Add(new ShopifyCountry("Latvia", "LV"));
                _collection.Add(new ShopifyCountry("Lebanon", "LB"));
                _collection.Add(new ShopifyCountry("Lesotho", "LS"));
                _collection.Add(new ShopifyCountry("Liberia", "LR"));
                _collection.Add(new ShopifyCountry("Libyan Arab Jamahiriya", "LY"));
                _collection.Add(new ShopifyCountry("Liechtenstein", "LI"));
                _collection.Add(new ShopifyCountry("Lithuania", "LT"));
                _collection.Add(new ShopifyCountry("Luxembourg", "LU"));
                _collection.Add(new ShopifyCountry("Macao", "MO"));
                _collection.Add(new ShopifyCountry("Madagascar", "MG"));
                _collection.Add(new ShopifyCountry("Malawi", "MW"));
                _collection.Add(new ShopifyCountry("Malaysia", "MY"));
                _collection.Add(new ShopifyCountry("Maldives", "MV"));
                _collection.Add(new ShopifyCountry("Mali", "ML"));
                _collection.Add(new ShopifyCountry("Malta", "MT"));
                _collection.Add(new ShopifyCountry("Martinique", "MQ"));
                _collection.Add(new ShopifyCountry("Mauritania", "MR"));
                _collection.Add(new ShopifyCountry("Mauritius", "MU"));
                _collection.Add(new ShopifyCountry("Mayotte", "YT"));
                _collection.Add(new ShopifyCountry("Mexico", "MX"));
                _collection.Add(new ShopifyCountry("Moldova, Republic of", "MD"));
                _collection.Add(new ShopifyCountry("Monaco", "MC"));
                _collection.Add(new ShopifyCountry("Mongolia", "MN"));
                _collection.Add(new ShopifyCountry("Montenegro", "ME"));
                _collection.Add(new ShopifyCountry("Montserrat", "MS"));
                _collection.Add(new ShopifyCountry("Morocco", "MA"));
                _collection.Add(new ShopifyCountry("Mozambique", "MZ"));
                _collection.Add(new ShopifyCountry("Myanmar", "MM"));
                _collection.Add(new ShopifyCountry("Namibia", "NA"));
                _collection.Add(new ShopifyCountry("Nauru", "NR"));
                _collection.Add(new ShopifyCountry("Nepal", "NP"));
                _collection.Add(new ShopifyCountry("Netherlands", "NL"));
                _collection.Add(new ShopifyCountry("Netherlands Antilles", "AN"));
                _collection.Add(new ShopifyCountry("New Caledonia", "NC"));
                _collection.Add(new ShopifyCountry("New Zealand", "NZ"));
                _collection.Add(new ShopifyCountry("Nicaragua", "NI"));
                _collection.Add(new ShopifyCountry("Niger", "NE"));
                _collection.Add(new ShopifyCountry("Nigeria", "NG"));
                _collection.Add(new ShopifyCountry("Niue", "NU"));
                _collection.Add(new ShopifyCountry("Norfolk Island", "NF"));
                _collection.Add(new ShopifyCountry("North Macedonia", "MK"));
                _collection.Add(new ShopifyCountry("Norway", "NO"));
                _collection.Add(new ShopifyCountry("Oman", "OM"));
                _collection.Add(new ShopifyCountry("Pakistan", "PK"));
                _collection.Add(new ShopifyCountry("Palestinian Territory, Occupied", "PS"));
                _collection.Add(new ShopifyCountry("Panama", "PA"));
                _collection.Add(new ShopifyCountry("Papua New Guinea", "PG"));
                _collection.Add(new ShopifyCountry("Paraguay", "PY"));
                _collection.Add(new ShopifyCountry("Peru", "PE"));
                _collection.Add(new ShopifyCountry("Philippines", "PH"));
                _collection.Add(new ShopifyCountry("Pitcairn", "PN"));
                _collection.Add(new ShopifyCountry("Poland", "PL"));
                _collection.Add(new ShopifyCountry("Portugal", "PT"));
                _collection.Add(new ShopifyCountry("Qatar", "QA"));
                _collection.Add(new ShopifyCountry("Republic of Cameroon", "CM"));
                _collection.Add(new ShopifyCountry("Rest of World", "*"));
                _collection.Add(new ShopifyCountry("Reunion", "RE"));
                _collection.Add(new ShopifyCountry("Romania", "RO"));
                _collection.Add(new ShopifyCountry("Russia", "RU"));
                _collection.Add(new ShopifyCountry("Rwanda", "RW"));
                _collection.Add(new ShopifyCountry("Saint Barthélemy", "BL"));
                _collection.Add(new ShopifyCountry("Saint Helena", "SH"));
                _collection.Add(new ShopifyCountry("Saint Kitts And Nevis", "KN"));
                _collection.Add(new ShopifyCountry("Saint Lucia", "LC"));
                _collection.Add(new ShopifyCountry("Saint Martin", "MF"));
                _collection.Add(new ShopifyCountry("Saint Pierre And Miquelon", "PM"));
                _collection.Add(new ShopifyCountry("Samoa", "WS"));
                _collection.Add(new ShopifyCountry("San Marino", "SM"));
                _collection.Add(new ShopifyCountry("Sao Tome And Principe", "ST"));
                _collection.Add(new ShopifyCountry("Saudi Arabia", "SA"));
                _collection.Add(new ShopifyCountry("Senegal", "SN"));
                _collection.Add(new ShopifyCountry("Serbia", "RS"));
                _collection.Add(new ShopifyCountry("Seychelles", "SC"));
                _collection.Add(new ShopifyCountry("Sierra Leone", "SL"));
                _collection.Add(new ShopifyCountry("Singapore", "SG"));
                _collection.Add(new ShopifyCountry("Sint Maarten", "SX"));
                _collection.Add(new ShopifyCountry("Slovakia", "SK"));
                _collection.Add(new ShopifyCountry("Slovenia", "SI"));
                _collection.Add(new ShopifyCountry("Solomon Islands", "SB"));
                _collection.Add(new ShopifyCountry("Somalia", "SO"));
                _collection.Add(new ShopifyCountry("South Africa", "ZA"));
                _collection.Add(new ShopifyCountry("South Georgia And The South Sandwich Islands", "GS"));
                _collection.Add(new ShopifyCountry("South Korea", "KR"));
                _collection.Add(new ShopifyCountry("South Sudan", "SS"));
                _collection.Add(new ShopifyCountry("Spain", "ES"));
                _collection.Add(new ShopifyCountry("Sri Lanka", "LK"));
                _collection.Add(new ShopifyCountry("St. Vincent", "VC"));
                _collection.Add(new ShopifyCountry("Sudan", "SD"));
                _collection.Add(new ShopifyCountry("Suriname", "SR"));
                _collection.Add(new ShopifyCountry("Svalbard And Jan Mayen", "SJ"));
                _collection.Add(new ShopifyCountry("Sweden", "SE"));
                _collection.Add(new ShopifyCountry("Switzerland", "CH"));
                _collection.Add(new ShopifyCountry("Syria", "SY"));
                _collection.Add(new ShopifyCountry("Taiwan", "TW"));
                _collection.Add(new ShopifyCountry("Tajikistan", "TJ"));
                _collection.Add(new ShopifyCountry("Tanzania, United Republic Of", "TZ"));
                _collection.Add(new ShopifyCountry("Thailand", "TH"));
                _collection.Add(new ShopifyCountry("Timor Leste", "TL"));
                _collection.Add(new ShopifyCountry("Togo", "TG"));
                _collection.Add(new ShopifyCountry("Tokelau", "TK"));
                _collection.Add(new ShopifyCountry("Tonga", "TO"));
                _collection.Add(new ShopifyCountry("Trinidad and Tobago", "TT"));
                _collection.Add(new ShopifyCountry("Tunisia", "TN"));
                _collection.Add(new ShopifyCountry("Turkey", "TR"));
                _collection.Add(new ShopifyCountry("Turkmenistan", "TM"));
                _collection.Add(new ShopifyCountry("Turks and Caicos Islands", "TC"));
                _collection.Add(new ShopifyCountry("Tuvalu", "TV"));
                _collection.Add(new ShopifyCountry("Uganda", "UG"));
                _collection.Add(new ShopifyCountry("Ukraine", "UA"));
                _collection.Add(new ShopifyCountry("United Arab Emirates", "AE"));
                _collection.Add(new ShopifyCountry("United Kingdom", "GB"));
                _collection.Add(new ShopifyCountry("United States", "US"));
                _collection.Add(new ShopifyCountry("United States Minor Outlying Islands", "UM"));
                _collection.Add(new ShopifyCountry("Uruguay", "UY"));
                _collection.Add(new ShopifyCountry("Uzbekistan", "UZ"));
                _collection.Add(new ShopifyCountry("Vanuatu", "VU"));
                _collection.Add(new ShopifyCountry("Venezuela", "VE"));
                _collection.Add(new ShopifyCountry("Vietnam", "VN"));
                _collection.Add(new ShopifyCountry("Virgin Islands, British", "VG"));
                _collection.Add(new ShopifyCountry("Wallis And Futuna", "WF"));
                _collection.Add(new ShopifyCountry("Western Sahara", "EH"));
                _collection.Add(new ShopifyCountry("Yemen", "YE"));
                _collection.Add(new ShopifyCountry("Zambia", "ZM"));
                _collection.Add(new ShopifyCountry("Zimbabwe", "ZW"));
            }
            return _collection;
        }
    }
    #endregion
}

/// <summary>
/// Representation of an ShopifyCountries-1 Country
/// </summary>
public class ShopifyCountry
{
    public ShopifyCountry(string name, string alpha2)
    {
        this.Name = name;
        this.Alpha2 = alpha2;
    }

    public string Name { get; private set; }

    public string Alpha2 { get; private set; }
    
}