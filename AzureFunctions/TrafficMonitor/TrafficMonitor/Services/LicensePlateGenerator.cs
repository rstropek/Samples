using System;
using System.Linq;

namespace TrafficMonitor.Services
{
    public class LicensePlateGenerator
    {
        private static string[] regionsAustria = new[] {
            "AM", "B", "BA", "BL", "BM", "BN", "BR", "BZ", "DL", "DO", "E", "EF", "EU", "FB", "FE", "FF", "FK", "FR", "G", "GB", "GD", "GF", "GM",
            "GR", "GS", "GU", "HA", "HB", "HE", "HL", "HO", "I", "IL", "IM", "JE", "JO", "JU", "K", "KB", "KF", "KI", "KL", "KO", "KR", "KS", "KU",
            "L", "LA", "LB", "LE", "LF", "LI", "LL", "LN", "LZ", "MA", "MD", "ME", "MI", "MU", "MZ", "ND", "NK", "OP", "OW", "P", "PE", "PL", "RA",
            "RE", "RI", "RO", "S", "SB", "SD", "SE", "SL", "SP", "SR", "SV", "SW", "SZ", "TA", "TU", "UU", "VB", "VI", "VK", "VL", "VO", "W", "WB",
            "WE", "WL", "WN", "WO", "WT", "WU", "WY", "WZ", "ZE", "ZT"
        };

        private static string[] regionsGermany = new[] {
            "A", "AA", "AB", "ABG", "ABI", "AC", "AE", "AH", "AIB", "AIC", "AK", "ALF", "ALZ", "AM", "AN", "ANA", "ANG", "ANK", "AÖ", "AP", "APD",
            "ARN", "ART", "AS", "ASL", "ASZ", "AT", "AU", "AUR", "AW", "AZ", "AZE", "B", "BA", "BAD", "BAR", "BB", "BBG", "BBL", "BC", "BCH", "BD",
            "BE", "BED", "BER", "BF", "BG", "BGL", "BH", "BI", "BID", "BIN", "BIR", "BIT", "BIW", "BK", "BKS", "BL", "BLB", "BLK", "BM", "BN", "BNA",
            "BO", "BÖ", "BOH", "BOR", "BOT", "BRA", "BRB", "BRG", "BRK", "BRL", "BRV", "BS", "BT", "BTF", "BÜD", "BÜS", "BÜZ", "BW", "BWL", "BYL", "BZ",
            "C", "CA", "CAS", "CB", "CE", "CHA", "CLP", "CLZ", "CO", "COC", "COE", "CUX", "CW", "D", "DA", "DAH", "DAN", "DAU", "DB", "DBR", "DD", "DE",
            "DEG", "DEL", "DGF", "DH", "DI", "DIL", "DIN", "DIZ", "DKB", "DL", "DLG", "DM", "DN", "DO", "DON", "DU", "DÜW", "DW", "DZ", "E", "EA", "EB",
            "EBE", "EBN", "EBS", "ECK", "ED", "EE", "EF", "EG", "EI", "EIC", "EIL", "EIN", "EIS", "EL", "EM", "EMD", "EMS", "EN", "ER", "ERB", "ERH", "ERK",
            "ERZ", "ES", "ESB", "ESW", "EU", "EW", "F", "FB", "FD", "FDB", "FDS", "FEU", "FF", "FFB", "FG", "FI", "FKB", "FL", "FLÖ", "FN", "FO", "FOR",
            "FR", "FRG", "FRI", "FRW", "FS", "FT", "FTL", "FÜ", "FÜS", "G", "GA", "GAN", "GAP", "GC", "GD", "GDB", "GE", "GEL", "GEO", "GER", "GF", "GG",
            "GHA", "GHC", "GI", "GK", "GL", "GM", "GMN", "GNT", "GÖ", "GOA", "GOH", "GP", "GR", "GRA", "GRH", "GRI", "GRM", "GRZ", "GS", "GT", "GTH", "GÜ",
            "GUB", "GUN", "GVM", "GW", "GZ", "H", "HA", "HAB", "HAL", "HAM", "HAS", "HB", "HBN", "HBS", "HC", "HCH", "HD", "HDH", "HDL", "HE", "HEB",
            "HEF", "HEI", "HEL", "HER", "HET", "HF", "HG", "HGN", "HGW", "HH", "HHM", "HI", "HIG", "HIP", "HK", "HL", "HM", "HMÜ", "HN", "HO", "HOG", "HOH",
            "HOL", "HOM", "HOR", "HOT", "HP", "HR", "HRO", "HS", "HSK", "HST", "HU", "HV", "HVL", "HWI", "HX", "HY", "HZ", "IGB", "IK", "IL", "ILL", "IN",
            "IZ", "J", "JE", "JL", "JÜL", "K", "KA", "KB", "KC", "KE", "KEH", "KEL", "KEM", "KF", "KG", "KH", "KI", "KIB", "KK", "KL", "KLE", "KLZ", "KM",
            "KN", "KO", "KÖN", "KÖT", "KR", "KRU", "KS", "KT", "KU", "KÜN", "KUS", "KY", "KYF", "L", "LA", "LAU", "LB", "LBS", "LBZ", "LD", "LDK", "LDS",
            "LEO", "LER", "LEV", "LG", "LH", "LI", "LIB", "LIF", "LIP", "LL", "LM", "LÖ", "LÖB", "LOS", "LP", "LR", "LRO", "LSA", "LSN", "LSZ", "LU", "LÜN",
            "LUP", "LWL", "M", "MA", "MAB", "MAI", "MAK", "MAL", "MB", "MC", "MD", "ME", "MEI", "MEK", "MER", "MET", "MG", "MGH", "MGN", "MH", "MHL", "MI",
            "MIL", "MK", "MKK", "ML", "MM", "MN", "MO", "MOD", "MOL", "MON", "MOS", "MQ", "MR", "MS", "MSE", "MSH", "MSP", "MST", "MTL", "MTK", "MÜ",
            "MÜB", "MÜR", "MVL", "MW", "MY", "MYK", "MZ", "MZG", "N", "NAB", "NAI", "NB", "ND", "NDH", "NE", "NEA", "NEB", "NEC", "NEN", "NES", "NEW", "NF",
            "NH", "NI", "NK", "NL", "NM", "NMB", "NMS", "NÖ", "NOH", "NOL", "NOM", "NOR", "NP", "NR", "NRW", "NU", "NVP", "NW", "NWM", "NY", "NZ", "OA",
            "OAL", "OB", "OBG", "OC", "OCH", "OD", "OE", "OF", "OG", "OH", "OHA", "OHV", "OHZ", "OK", "OL", "OPR", "OR", "OS", "OSL", "OVI", "OVL", "OVL",
            "OZ", "P", "PA", "PAF", "PAN", "PAR", "PB", "PCH", "PE", "PEG", "PF", "PI", "PIR", "PL", "PLÖ", "PM", "PN", "PR", "PRÜ", "PS", "PW", "PZ",
            "QFT", "QLB", "R", "RA", "RC", "RD", "RDG", "RE", "REG", "REH", "RG", "RH", "RI", "RID", "RIE", "RL", "RM", "RO", "ROD", "ROF", "ROK", "ROL",
            "ROS", "ROT", "ROW", "RP", "RPL", "RS", "RSL", "RT", "RU", "RÜD", "RÜG", "RV", "RW", "RZ", "S", "SAB", "SAD", "SAL", "SAN", "SAW", "SB", "SBG",
            "SBK", "SC", "SCZ", "SDH", "SDL", "SDT", "SE", "SEB", "SEE", "SEF", "SEL", "SFB", "SFT", "SG", "SGH", "SH", "SHA", "SHG", "SHK", "SHL", "SI", "SIG",
            "SIM", "SK", "SL", "SLE", "SLF", "SLK", "SLN", "SLS", "SLÜ", "SLZ", "SM", "SN", "SO", "SOB", "SOG", "SOK", "SÖM", "SON", "SP", "SPB", "SPN", "SR", "SRB",
            "SRO", "ST", "STA", "STB", "STD", "STE", "STL", "SU", "SUL", "SÜW", "SW", "SWA", "SZ", "SZB", "TBB", "TDO", "TE", "TET", "TF", "TG", "THL", "THW", "TIR",
            "TO", "TÖL", "TP", "TR", "TS", "TÜ", "TUT", "UE", "UEM", "UFF", "UH", "UL", "UM", "UN", "USI", "V", "VAI", "VB", "VEC", "VER", "VG", "VIB", "VIE", "VK",
            "VOH", "VR", "VS", "W", "WA", "WAF", "WAK", "WAN", "WAT", "WB", "WBS", "WDA", "WE", "WEL", "WEN", "WER", "WES", "WF", "WHV", "WI", "WIL", "WIS",
            "WIT", "WK", "WL", "WLG", "WM", "WMS", "WN", "WND", "WO", "WOB", "WOH", "WOL", "WOR", "WOS", "WR", "WRN", "WS", "WSF", "WST", "WSW", "WT", "WTM", "WÜ",
            "WUG", "WÜM", "WUN", "WUR", "WW", "WZ", "WIZ", "WZL", "Z", "ZE", "ZEL", "ZI", "ZP", "ZR", "ZW", "ZZ"
        };

        private static Random Random = new Random();

        private static int GetRandomNumericPart() => Random.Next(10, 1000);

        private static string GetRandomTextPart() =>
            new string(Enumerable.Range(0, Random.Next(2, 4))
                .Select(_ => Convert.ToChar(Convert.ToByte('A') + Random.Next(0, 26)))
                .ToArray());

        public string GetRandomAustrianLicensePlate() =>
            $"{regionsAustria[Random.Next(0, regionsAustria.Length)]}{GetRandomNumericPart()}{GetRandomTextPart()}";

        public string GetRandomGermanLicensePlate() =>
            $"{regionsGermany[Random.Next(0, regionsGermany.Length)]}{GetRandomTextPart()}{GetRandomNumericPart()}";
    }
}
