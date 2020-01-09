using System.Collections.ObjectModel;
using System.Linq;

// Quick and dirty but flexible solution
// https://stackoverflow.com/a/35295634

public static class ISO3166
{
    /// <summary>
    /// Obtain ISO3166-1 Country based on its alpha3 code.
    /// </summary>
    /// <param name="alpha3"></param>
    /// <returns></returns>
    public static ISO3166Country FromAlpha3(string alpha3)
    {
        return Collection.FirstOrDefault(p => p.Alpha3 == alpha3);
    }
    
    /// <summary>
    /// Obtain ISO3166-1 Country based on its alpha2 code.
    /// </summary>
    /// <param name="alpha2"></param>
    /// <returns></returns>
    public static ISO3166Country FromAlpha2(string alpha2)
    {
        return Collection.FirstOrDefault(p => p.Alpha2 == alpha2);
    }
    
    /// <summary>
    /// Obtain ISO3166-1 Country based on its full name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static ISO3166Country FromName(string name)
    {
        return Collection.FirstOrDefault(p => p.Name == name);
    }

    #region Build Collection

    private static Collection<ISO3166Country> _collection;

    public static Collection<ISO3166Country> Collection
    {
        get
        {
            if (_collection == null)
            {
                _collection = new Collection<ISO3166Country>();
                _collection.Add(new ISO3166Country("Afghanistan", "AF", "AFG", 4));
                _collection.Add(new ISO3166Country("Åland Islands", "AX", "ALA", 248));
                _collection.Add(new ISO3166Country("Albania", "AL", "ALB", 8));
                _collection.Add(new ISO3166Country("Algeria", "DZ", "DZA", 12));
                _collection.Add(new ISO3166Country("American Samoa", "AS", "ASM", 16));
                _collection.Add(new ISO3166Country("Andorra", "AD", "AND", 20));
                _collection.Add(new ISO3166Country("Angola", "AO", "AGO", 24));
                _collection.Add(new ISO3166Country("Anguilla", "AI", "AIA", 660));
                _collection.Add(new ISO3166Country("Antarctica", "AQ", "ATA", 10));
                _collection.Add(new ISO3166Country("Antigua and Barbuda", "AG", "ATG", 28));
                _collection.Add(new ISO3166Country("Argentina", "AR", "ARG", 32));
                _collection.Add(new ISO3166Country("Armenia", "AM", "ARM", 51));
                _collection.Add(new ISO3166Country("Aruba", "AW", "ABW", 533));
                _collection.Add(new ISO3166Country("Australia", "AU", "AUS", 36));
                _collection.Add(new ISO3166Country("Austria", "AT", "AUT", 40));
                _collection.Add(new ISO3166Country("Azerbaijan", "AZ", "AZE", 31));
                _collection.Add(new ISO3166Country("Bahamas", "BS", "BHS", 44));
                _collection.Add(new ISO3166Country("Bahrain", "BH", "BHR", 48));
                _collection.Add(new ISO3166Country("Bangladesh", "BD", "BGD", 50));
                _collection.Add(new ISO3166Country("Barbados", "BB", "BRB", 52));
                _collection.Add(new ISO3166Country("Belarus", "BY", "BLR", 112));
                _collection.Add(new ISO3166Country("Belgium", "BE", "BEL", 56));
                _collection.Add(new ISO3166Country("Belize", "BZ", "BLZ", 84));
                _collection.Add(new ISO3166Country("Benin", "BJ", "BEN", 204));
                _collection.Add(new ISO3166Country("Bermuda", "BM", "BMU", 60));
                _collection.Add(new ISO3166Country("Bhutan", "BT", "BTN", 64));
                _collection.Add(new ISO3166Country("Bolivia (Plurinational State of)", "BO", "BOL", 68));
                _collection.Add(new ISO3166Country("Bonaire, Sint Eustatius and Saba", "BQ", "BES", 535));
                _collection.Add(new ISO3166Country("Bosnia and Herzegovina", "BA", "BIH", 70));
                _collection.Add(new ISO3166Country("Botswana", "BW", "BWA", 72));
                _collection.Add(new ISO3166Country("Bouvet Island", "BV", "BVT", 74));
                _collection.Add(new ISO3166Country("Brazil", "BR", "BRA", 76));
                _collection.Add(new ISO3166Country("British Indian Ocean Territory", "IO", "IOT", 86));
                _collection.Add(new ISO3166Country("Brunei Darussalam", "BN", "BRN", 96));
                _collection.Add(new ISO3166Country("Bulgaria", "BG", "BGR", 100));
                _collection.Add(new ISO3166Country("Burkina Faso", "BF", "BFA", 854));
                _collection.Add(new ISO3166Country("Burundi", "BI", "BDI", 108));
                _collection.Add(new ISO3166Country("Cabo Verde", "CV", "CPV", 132));
                _collection.Add(new ISO3166Country("Cambodia", "KH", "KHM", 116));
                _collection.Add(new ISO3166Country("Cameroon", "CM", "CMR", 120));
                _collection.Add(new ISO3166Country("Canada", "CA", "CAN", 124));
                _collection.Add(new ISO3166Country("Cayman Islands", "KY", "CYM", 136));
                _collection.Add(new ISO3166Country("Central African Republic", "CF", "CAF", 140));
                _collection.Add(new ISO3166Country("Chad", "TD", "TCD", 148));
                _collection.Add(new ISO3166Country("Chile", "CL", "CHL", 152));
                _collection.Add(new ISO3166Country("China", "CN", "CHN", 156));
                _collection.Add(new ISO3166Country("Christmas Island", "CX", "CXR", 162));
                _collection.Add(new ISO3166Country("Cocos (Keeling) Islands", "CC", "CCK", 166));
                _collection.Add(new ISO3166Country("Colombia", "CO", "COL", 170));
                _collection.Add(new ISO3166Country("Comoros", "KM", "COM", 174));
                _collection.Add(new ISO3166Country("Congo", "CG", "COG", 178));
                _collection.Add(new ISO3166Country("Congo (Democratic Republic of the)", "CD", "COD", 180));
                _collection.Add(new ISO3166Country("Cook Islands", "CK", "COK", 184));
                _collection.Add(new ISO3166Country("Costa Rica", "CR", "CRI", 188));
                _collection.Add(new ISO3166Country("Côte d'Ivoire", "CI", "CIV", 384));
                _collection.Add(new ISO3166Country("Croatia", "HR", "HRV", 191));
                _collection.Add(new ISO3166Country("Cuba", "CU", "CUB", 192));
                _collection.Add(new ISO3166Country("Curaçao", "CW", "CUW", 531));
                _collection.Add(new ISO3166Country("Cyprus", "CY", "CYP", 196));
                _collection.Add(new ISO3166Country("Czech Republic", "CZ", "CZE", 203));
                _collection.Add(new ISO3166Country("Denmark", "DK", "DNK", 208));
                _collection.Add(new ISO3166Country("Djibouti", "DJ", "DJI", 262));
                _collection.Add(new ISO3166Country("Dominica", "DM", "DMA", 212));
                _collection.Add(new ISO3166Country("Dominican Republic", "DO", "DOM", 214));
                _collection.Add(new ISO3166Country("Ecuador", "EC", "ECU", 218));
                _collection.Add(new ISO3166Country("Egypt", "EG", "EGY", 818));
                _collection.Add(new ISO3166Country("El Salvador", "SV", "SLV", 222));
                _collection.Add(new ISO3166Country("Equatorial Guinea", "GQ", "GNQ", 226));
                _collection.Add(new ISO3166Country("Eritrea", "ER", "ERI", 232));
                _collection.Add(new ISO3166Country("Estonia", "EE", "EST", 233));
                _collection.Add(new ISO3166Country("Ethiopia", "ET", "ETH", 231));
                _collection.Add(new ISO3166Country("Falkland Islands (Malvinas)", "FK", "FLK", 238));
                _collection.Add(new ISO3166Country("Faroe Islands", "FO", "FRO", 234));
                _collection.Add(new ISO3166Country("Fiji", "FJ", "FJI", 242));
                _collection.Add(new ISO3166Country("Finland", "FI", "FIN", 246));
                _collection.Add(new ISO3166Country("France", "FR", "FRA", 250));
                _collection.Add(new ISO3166Country("French Guiana", "GF", "GUF", 254));
                _collection.Add(new ISO3166Country("French Polynesia", "PF", "PYF", 258));
                _collection.Add(new ISO3166Country("French Southern Territories", "TF", "ATF", 260));
                _collection.Add(new ISO3166Country("Gabon", "GA", "GAB", 266));
                _collection.Add(new ISO3166Country("Gambia", "GM", "GMB", 270));
                _collection.Add(new ISO3166Country("Georgia", "GE", "GEO", 268));
                _collection.Add(new ISO3166Country("Germany", "DE", "DEU", 276));
                _collection.Add(new ISO3166Country("Ghana", "GH", "GHA", 288));
                _collection.Add(new ISO3166Country("Gibraltar", "GI", "GIB", 292));
                _collection.Add(new ISO3166Country("Greece", "GR", "GRC", 300));
                _collection.Add(new ISO3166Country("Greenland", "GL", "GRL", 304));
                _collection.Add(new ISO3166Country("Grenada", "GD", "GRD", 308));
                _collection.Add(new ISO3166Country("Guadeloupe", "GP", "GLP", 312));
                _collection.Add(new ISO3166Country("Guam", "GU", "GUM", 316));
                _collection.Add(new ISO3166Country("Guatemala", "GT", "GTM", 320));
                _collection.Add(new ISO3166Country("Guernsey", "GG", "GGY", 831));
                _collection.Add(new ISO3166Country("Guinea", "GN", "GIN", 324));
                _collection.Add(new ISO3166Country("Guinea-Bissau", "GW", "GNB", 624));
                _collection.Add(new ISO3166Country("Guyana", "GY", "GUY", 328));
                _collection.Add(new ISO3166Country("Haiti", "HT", "HTI", 332));
                _collection.Add(new ISO3166Country("Heard Island and McDonald Islands", "HM", "HMD", 334));
                _collection.Add(new ISO3166Country("Holy See", "VA", "VAT", 336));
                _collection.Add(new ISO3166Country("Honduras", "HN", "HND", 340));
                _collection.Add(new ISO3166Country("Hong Kong", "HK", "HKG", 344));
                _collection.Add(new ISO3166Country("Hungary", "HU", "HUN", 348));
                _collection.Add(new ISO3166Country("Iceland", "IS", "ISL", 352));
                _collection.Add(new ISO3166Country("India", "IN", "IND", 356));
                _collection.Add(new ISO3166Country("Indonesia", "ID", "IDN", 360));
                _collection.Add(new ISO3166Country("Iran (Islamic Republic of)", "IR", "IRN", 364));
                _collection.Add(new ISO3166Country("Iraq", "IQ", "IRQ", 368));
                _collection.Add(new ISO3166Country("Ireland", "IE", "IRL", 372));
                _collection.Add(new ISO3166Country("Isle of Man", "IM", "IMN", 833));
                _collection.Add(new ISO3166Country("Israel", "IL", "ISR", 376));
                _collection.Add(new ISO3166Country("Italy", "IT", "ITA", 380));
                _collection.Add(new ISO3166Country("Jamaica", "JM", "JAM", 388));
                _collection.Add(new ISO3166Country("Japan", "JP", "JPN", 392));
                _collection.Add(new ISO3166Country("Jersey", "JE", "JEY", 832));
                _collection.Add(new ISO3166Country("Jordan", "JO", "JOR", 400));
                _collection.Add(new ISO3166Country("Kazakhstan", "KZ", "KAZ", 398));
                _collection.Add(new ISO3166Country("Kenya", "KE", "KEN", 404));
                _collection.Add(new ISO3166Country("Kiribati", "KI", "KIR", 296));
                _collection.Add(new ISO3166Country("Korea (Democratic People's Republic of)", "KP", "PRK", 408));
                _collection.Add(new ISO3166Country("Korea (Republic of)", "KR", "KOR", 410));
                _collection.Add(new ISO3166Country("Kuwait", "KW", "KWT", 414));
                _collection.Add(new ISO3166Country("Kyrgyzstan", "KG", "KGZ", 417));
                _collection.Add(new ISO3166Country("Lao People's Democratic Republic", "LA", "LAO", 418));
                _collection.Add(new ISO3166Country("Latvia", "LV", "LVA", 428));
                _collection.Add(new ISO3166Country("Lebanon", "LB", "LBN", 422));
                _collection.Add(new ISO3166Country("Lesotho", "LS", "LSO", 426));
                _collection.Add(new ISO3166Country("Liberia", "LR", "LBR", 430));
                _collection.Add(new ISO3166Country("Libya", "LY", "LBY", 434));
                _collection.Add(new ISO3166Country("Liechtenstein", "LI", "LIE", 438));
                _collection.Add(new ISO3166Country("Lithuania", "LT", "LTU", 440));
                _collection.Add(new ISO3166Country("Luxembourg", "LU", "LUX", 442));
                _collection.Add(new ISO3166Country("Macao", "MO", "MAC", 446));
                _collection.Add(new ISO3166Country("Macedonia (the former Yugoslav Republic of)", "MK", "MKD", 807));
                _collection.Add(new ISO3166Country("Madagascar", "MG", "MDG", 450));
                _collection.Add(new ISO3166Country("Malawi", "MW", "MWI", 454));
                _collection.Add(new ISO3166Country("Malaysia", "MY", "MYS", 458));
                _collection.Add(new ISO3166Country("Maldives", "MV", "MDV", 462));
                _collection.Add(new ISO3166Country("Mali", "ML", "MLI", 466));
                _collection.Add(new ISO3166Country("Malta", "MT", "MLT", 470));
                _collection.Add(new ISO3166Country("Marshall Islands", "MH", "MHL", 584));
                _collection.Add(new ISO3166Country("Martinique", "MQ", "MTQ", 474));
                _collection.Add(new ISO3166Country("Mauritania", "MR", "MRT", 478));
                _collection.Add(new ISO3166Country("Mauritius", "MU", "MUS", 480));
                _collection.Add(new ISO3166Country("Mayotte", "YT", "MYT", 175));
                _collection.Add(new ISO3166Country("Mexico", "MX", "MEX", 484));
                _collection.Add(new ISO3166Country("Micronesia (Federated States of)", "FM", "FSM", 583));
                _collection.Add(new ISO3166Country("Moldova (Republic of)", "MD", "MDA", 498));
                _collection.Add(new ISO3166Country("Monaco", "MC", "MCO", 492));
                _collection.Add(new ISO3166Country("Mongolia", "MN", "MNG", 496));
                _collection.Add(new ISO3166Country("Montenegro", "ME", "MNE", 499));
                _collection.Add(new ISO3166Country("Montserrat", "MS", "MSR", 500));
                _collection.Add(new ISO3166Country("Morocco", "MA", "MAR", 504));
                _collection.Add(new ISO3166Country("Mozambique", "MZ", "MOZ", 508));
                _collection.Add(new ISO3166Country("Myanmar", "MM", "MMR", 104));
                _collection.Add(new ISO3166Country("Namibia", "NA", "NAM", 516));
                _collection.Add(new ISO3166Country("Nauru", "NR", "NRU", 520));
                _collection.Add(new ISO3166Country("Nepal", "NP", "NPL", 524));
                _collection.Add(new ISO3166Country("Netherlands", "NL", "NLD", 528));
                _collection.Add(new ISO3166Country("New Caledonia", "NC", "NCL", 540));
                _collection.Add(new ISO3166Country("New Zealand", "NZ", "NZL", 554));
                _collection.Add(new ISO3166Country("Nicaragua", "NI", "NIC", 558));
                _collection.Add(new ISO3166Country("Niger", "NE", "NER", 562));
                _collection.Add(new ISO3166Country("Nigeria", "NG", "NGA", 566));
                _collection.Add(new ISO3166Country("Niue", "NU", "NIU", 570));
                _collection.Add(new ISO3166Country("Norfolk Island", "NF", "NFK", 574));
                _collection.Add(new ISO3166Country("Northern Mariana Islands", "MP", "MNP", 580));
                _collection.Add(new ISO3166Country("Norway", "NO", "NOR", 578));
                _collection.Add(new ISO3166Country("Oman", "OM", "OMN", 512));
                _collection.Add(new ISO3166Country("Pakistan", "PK", "PAK", 586));
                _collection.Add(new ISO3166Country("Palau", "PW", "PLW", 585));
                _collection.Add(new ISO3166Country("Palestine, State of", "PS", "PSE", 275));
                _collection.Add(new ISO3166Country("Panama", "PA", "PAN", 591));
                _collection.Add(new ISO3166Country("Papua New Guinea", "PG", "PNG", 598));
                _collection.Add(new ISO3166Country("Paraguay", "PY", "PRY", 600));
                _collection.Add(new ISO3166Country("Peru", "PE", "PER", 604));
                _collection.Add(new ISO3166Country("Philippines", "PH", "PHL", 608));
                _collection.Add(new ISO3166Country("Pitcairn", "PN", "PCN", 612));
                _collection.Add(new ISO3166Country("Poland", "PL", "POL", 616));
                _collection.Add(new ISO3166Country("Portugal", "PT", "PRT", 620));
                _collection.Add(new ISO3166Country("Puerto Rico", "PR", "PRI", 630));
                _collection.Add(new ISO3166Country("Qatar", "QA", "QAT", 634));
                _collection.Add(new ISO3166Country("Réunion", "RE", "REU", 638));
                _collection.Add(new ISO3166Country("Romania", "RO", "ROU", 642));
                _collection.Add(new ISO3166Country("Russian Federation", "RU", "RUS", 643));
                _collection.Add(new ISO3166Country("Rwanda", "RW", "RWA", 646));
                _collection.Add(new ISO3166Country("Saint Barthélemy", "BL", "BLM", 652));
                _collection.Add(new ISO3166Country("Saint Helena, Ascension and Tristan da Cunha", "SH", "SHN", 654));
                _collection.Add(new ISO3166Country("Saint Kitts and Nevis", "KN", "KNA", 659));
                _collection.Add(new ISO3166Country("Saint Lucia", "LC", "LCA", 662));
                _collection.Add(new ISO3166Country("Saint Martin (French part)", "MF", "MAF", 663));
                _collection.Add(new ISO3166Country("Saint Pierre and Miquelon", "PM", "SPM", 666));
                _collection.Add(new ISO3166Country("Saint Vincent and the Grenadines", "VC", "VCT", 670));
                _collection.Add(new ISO3166Country("Samoa", "WS", "WSM", 882));
                _collection.Add(new ISO3166Country("San Marino", "SM", "SMR", 674));
                _collection.Add(new ISO3166Country("Sao Tome and Principe", "ST", "STP", 678));
                _collection.Add(new ISO3166Country("Saudi Arabia", "SA", "SAU", 682));
                _collection.Add(new ISO3166Country("Senegal", "SN", "SEN", 686));
                _collection.Add(new ISO3166Country("Serbia", "RS", "SRB", 688));
                _collection.Add(new ISO3166Country("Seychelles", "SC", "SYC", 690));
                _collection.Add(new ISO3166Country("Sierra Leone", "SL", "SLE", 694));
                _collection.Add(new ISO3166Country("Singapore", "SG", "SGP", 702));
                _collection.Add(new ISO3166Country("Sint Maarten (Dutch part)", "SX", "SXM", 534));
                _collection.Add(new ISO3166Country("Slovakia", "SK", "SVK", 703));
                _collection.Add(new ISO3166Country("Slovenia", "SI", "SVN", 705));
                _collection.Add(new ISO3166Country("Solomon Islands", "SB", "SLB", 90));
                _collection.Add(new ISO3166Country("Somalia", "SO", "SOM", 706));
                _collection.Add(new ISO3166Country("South Africa", "ZA", "ZAF", 710));
                _collection.Add(new ISO3166Country("South Georgia and the South Sandwich Islands", "GS", "SGS", 239));
                _collection.Add(new ISO3166Country("South Sudan", "SS", "SSD", 728));
                _collection.Add(new ISO3166Country("Spain", "ES", "ESP", 724));
                _collection.Add(new ISO3166Country("Sri Lanka", "LK", "LKA", 144));
                _collection.Add(new ISO3166Country("Sudan", "SD", "SDN", 729));
                _collection.Add(new ISO3166Country("Suriname", "SR", "SUR", 740));
                _collection.Add(new ISO3166Country("Svalbard and Jan Mayen", "SJ", "SJM", 744));
                _collection.Add(new ISO3166Country("Swaziland", "SZ", "SWZ", 748));
                _collection.Add(new ISO3166Country("Sweden", "SE", "SWE", 752));
                _collection.Add(new ISO3166Country("Switzerland", "CH", "CHE", 756));
                _collection.Add(new ISO3166Country("Syrian Arab Republic", "SY", "SYR", 760));
                _collection.Add(new ISO3166Country("Taiwan, Province of China[a]", "TW", "TWN", 158));
                _collection.Add(new ISO3166Country("Tajikistan", "TJ", "TJK", 762));
                _collection.Add(new ISO3166Country("Tanzania, United Republic of", "TZ", "TZA", 834));
                _collection.Add(new ISO3166Country("Thailand", "TH", "THA", 764));
                _collection.Add(new ISO3166Country("Timor-Leste", "TL", "TLS", 626));
                _collection.Add(new ISO3166Country("Togo", "TG", "TGO", 768));
                _collection.Add(new ISO3166Country("Tokelau", "TK", "TKL", 772));
                _collection.Add(new ISO3166Country("Tonga", "TO", "TON", 776));
                _collection.Add(new ISO3166Country("Trinidad and Tobago", "TT", "TTO", 780));
                _collection.Add(new ISO3166Country("Tunisia", "TN", "TUN", 788));
                _collection.Add(new ISO3166Country("Turkey", "TR", "TUR", 792));
                _collection.Add(new ISO3166Country("Turkmenistan", "TM", "TKM", 795));
                _collection.Add(new ISO3166Country("Turks and Caicos Islands", "TC", "TCA", 796));
                _collection.Add(new ISO3166Country("Tuvalu", "TV", "TUV", 798));
                _collection.Add(new ISO3166Country("Uganda", "UG", "UGA", 800));
                _collection.Add(new ISO3166Country("Ukraine", "UA", "UKR", 804));
                _collection.Add(new ISO3166Country("United Arab Emirates", "AE", "ARE", 784));
                _collection.Add(new ISO3166Country("United Kingdom of Great Britain and Northern Ireland", "GB", "GBR", 826));
                _collection.Add(new ISO3166Country("United States of America", "US", "USA", 840));
                _collection.Add(new ISO3166Country("United States Minor Outlying Islands", "UM", "UMI", 581));
                _collection.Add(new ISO3166Country("Uruguay", "UY", "URY", 858));
                _collection.Add(new ISO3166Country("Uzbekistan", "UZ", "UZB", 860));
                _collection.Add(new ISO3166Country("Vanuatu", "VU", "VUT", 548));
                _collection.Add(new ISO3166Country("Venezuela (Bolivarian Republic of)", "VE", "VEN", 862));
                _collection.Add(new ISO3166Country("Viet Nam", "VN", "VNM", 704));
                _collection.Add(new ISO3166Country("Virgin Islands (British)", "VG", "VGB", 92));
                _collection.Add(new ISO3166Country("Virgin Islands (U.S.)", "VI", "VIR", 850));
                _collection.Add(new ISO3166Country("Wallis and Futuna", "WF", "WLF", 876));
                _collection.Add(new ISO3166Country("Western Sahara", "EH", "ESH", 732));
                _collection.Add(new ISO3166Country("Yemen", "YE", "YEM", 887));
                _collection.Add(new ISO3166Country("Zambia", "ZM", "ZMB", 894));
                _collection.Add(new ISO3166Country("Zimbabwe", "ZW", "ZWE", 716));
            }
            return _collection;
        }
    }
    #endregion
}

/// <summary>
/// Representation of an ISO3166-1 Country
/// </summary>
public class ISO3166Country
{
    public ISO3166Country(string name, string alpha2, string alpha3, int numericCode)
    {
        this.Name = name;
        this.Alpha2 = alpha2;
        this.Alpha3 = alpha3;
        this.NumericCode = numericCode;
    }

    public string Name { get; private set; }

    public string Alpha2 { get; private set; }

    public string Alpha3 { get; private set; }

    public int NumericCode { get; private set; }
}