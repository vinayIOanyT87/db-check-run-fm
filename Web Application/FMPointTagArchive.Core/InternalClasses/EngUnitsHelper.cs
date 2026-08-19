
namespace FMPointTagArchive.Core.InternalClasses
{
    public class EngUnitsHelper
    {
        public static string GetUnitString(int EngineeringUnitIndex)
        {
            string returnValue = string.Empty;

            switch (EngineeringUnitIndex)
            {
                case 1: return "°C";
                case 2: return "°F";
                case 3: return "Kelvin";
                case 4: return "°R";
                case 5: return "msec";
                case 6: return "sec";
                case 7: return "min";
                case 8: return "hr";
                case 9: return "days";
                case 10: return "wks";
                case 11: return "mon";
                case 12: return "yrs";
                case 19: return "ft-in-8th";
                case 20: return "mm";
                case 21: return "cm";
                case 22: return "m";
                case 23: return "km";
                case 24: return "16th";
                case 25: return "in";
                case 26: return "ft";
                case 27: return "ft-in-16th";
                case 28: return "yd";
                case 29: return "mi";
                case 30: return "mm²";
                case 31: return "cm²";
                case 32: return "m²";
                case 33: return "km²";
                case 34: return "16th²";
                case 35: return "in²";
                case 36: return "ft²";
                case 37: return "yd²";
                case 38: return "mi²";
                case 40: return "cc";
                case 41: return "m³";
                case 42: return "l";
                case 43: return "in³";
                case 44: return "ft³";
                case 45: return "yd³";
                case 46: return "gal (US)";
                case 47: return "gal (UK)";
                case 48: return "bbl (Oil)";
                case 49: return "bbl (Liq)";
                case 50: return "kl";
                case 51: return "MsFt3";
                case 60: return "g";
                case 61: return "kg";
                case 62: return "ton (m)";
                case 63: return "oz";
                case 64: return "lb";
                case 65: return "ton (e)";
                case 66: return "ton (s)";
                case 67: return "ton (l)";
                case 68: return "Mlbs";
                case 70: return "Pa";
                case 71: return "kPa";
                case 72: return "kg/cm²";
                case 73: return "PSI";
                case 74: return "psig";
                case 75: return "psia";
                case 76: return "in H2O";
                case 77: return "ft H2O";
                case 78: return "in Hg";
                case 79: return "lb/ft²";
                case 80: return "torr";
                case 81: return "bar";
                case 82: return "mbar";
                case 83: return "mm Hg";
                case 84: return "mm H2O";
                case 85: return "g/cm²";
                case 86: return "atm";
                case 90: return "cc/min";
                case 91: return "cc/hr";
                case 92: return "m³/sec";
                case 93: return "m³/min";
                case 94: return "m³/hr";
                case 95: return "m³/day";
                case 96: return "l/sec";
                case 97: return "l/min";
                case 98: return "l/hr";
                case 99: return "Ml/day";
                case 100: return "in³/min";
                case 101: return "in³/hr";
                case 102: return "ft³/sec";
                case 103: return "ft³/min";
                case 104: return "ft³/hr";
                case 105: return "ft³/day";
                case 106: return "yd³/min";
                case 107: return "yd³/hr";
                case 108: return "gps (US)";
                case 109: return "gpm (US)";
                case 110: return "gph (US)";
                case 111: return "MGPD (US)";
                case 112: return "gps (UK)";
                case 113: return "gpm (UK)";
                case 114: return "gph (UK)";
                case 115: return "MGPD (UK)";
                case 116: return "BPM (Oil)";
                case 117: return "BPH (Oil)";
                case 118: return "BPD (Oil)";
                case 119: return "MBPD (Oil)";
                case 120: return "BPM (Liq)";
                case 121: return "BPH (Liq)";
                case 122: return "BPD (Liq)";
                case 123: return "MBPD (Liq)";
                case 124: return "kl/sec";
                case 125: return "kl/min";
                case 126: return "kl/hr";
                case 127: return "kl/day";
                case 130: return "lb/sec";
                case 131: return "lb/min";
                case 132: return "lb/hr";
                case 133: return "lb/day";
                case 134: return "ton(m)/min";
                case 135: return "ton(m)/hr";
                case 136: return "ton(m)/day";
                case 137: return "ton(s)/min";
                case 138: return "ton(s)/hr";
                case 139: return "ton(s)/day";
                case 140: return "ton(l)/min";
                case 141: return "ton(l)/hr";
                case 142: return "ton(l)/day";
                case 143: return "g/sec";
                case 144: return "g/min";
                case 145: return "g/hr";
                case 146: return "kg/sec";
                case 147: return "kg/min";
                case 148: return "kg/hr";
                case 149: return "kg/day";
                case 150: return "Mlbs/sec";
                case 151: return "Mlbs/min";
                case 152: return "Mlbs/hr";
                case 153: return "Mlbs/day";
                case 160: return "in/sec";
                case 161: return "ft/sec";
                case 162: return "ft/min";
                case 163: return "mm/sec";
                case 164: return "cm/sec";
                case 165: return "m/sec";
                case 166: return "m/min";
                case 167: return "MPH";
                case 168: return "m/hr";
                case 169: return "KPH";
                case 170: return "KNOT";
                case 171: return "mm/min";
                case 180: return "g/cm³";
                case 181: return "g/ml";
                case 182: return "g/l";
                case 183: return "kg/m³";
                case 184: return "kg/l";
                case 185: return "lb/in³";
                case 186: return "lb/ft³";
                case 187: return "lb/gal(US)";
                case 188: return "lb/gal(UK)";
                case 189: return "lb/bbl(o)";
                case 190: return "lb/bbl(l)";
                case 191: return "°API";
                case 192: return "sp gr";
                case 193: return "% Plato";
                case 194: return "°BRIX";
                case 195: return "°Ba (l)";
                case 196: return "°Ba (h)";
                case 197: return "°Tw";
                case 198: return "°Balling";
                case 199: return "ton(s)/yd³";
                case 200: return "BTU";
                case 201: return "cal";
                case 202: return "J";
                case 203: return "WH";
                case 204: return "kWH";
                case 210: return "BTU/sec";
                case 211: return "BTU/min";
                case 212: return "BTU/hr";
                case 213: return "cal/min";
                case 214: return "W";
                case 215: return "kW";
                case 216: return "kVA";
                case 217: return "hp";
                case 220: return "mV";
                case 221: return "V";
                case 222: return "mA";
                case 223: return "A";
                case 224: return "ohm";
                case 225: return "F";
                case 226: return "C";
                case 227: return "H";
                case 228: return "µS";
                case 229: return "S";
                case 230: return "mho";
                case 231: return "P.F.";
                case 232: return "RPM";
                case 233: return "Hz";
                case 234: return "%";
                case 235: return "PPM";
                case 236: return "%H";
                case 237: return "%O2";
                case 238: return "RH";
                case 239: return "pH";
                case 240: return "centp";
                case 241: return "%sol-wt";
                case 242: return "%sol-vol";
                case 243: return "%quality";
                case 244: return "bushel";
                case 245: return "pr vol";
                case 246: return "pr mass";
                case 247: return "ft³/lb";
                default:
                    break;
            }
            return returnValue;
        }


        public static string EncodeFtInFractionasString(int iDenominator, double value)
        {
            var valueDouble = value;

				if(double.IsNaN(value))
				{
				    return "NaN";
			   }

            // Get Whole Feet to Integer
            var negative = (valueDouble < 0.00);
            if (negative)
            {
                valueDouble = -valueDouble;
            }

            var feet = (int)valueDouble;
            var fraction = valueDouble - feet;

            // Convert to Inches
            fraction *= 12.0000;
            var inch = (int)fraction;
            fraction -= inch;

            int factor = (iDenominator == 16) ? 16 : 8;

            // Convert to Fraction
            fraction *= factor;
            var fract = (int)(fraction + 0.500);

            if (fract >= factor)
            {
                inch++;
                fract = 0;

                if (inch >= 12)
                {
                    feet++;
                    inch = 0;
                }
            }

            if (negative)
            {
                if (iDenominator == 16)
                {
                    return "-" + feet.ToString("D2") + "-" + inch.ToString("D2") + "-" + fract.ToString("D2");
                }

                return "-" + feet.ToString("D2") + "-" + inch.ToString("D2") + "-" + fract.ToString("D1");
            }

            if (iDenominator == 16)
            {
                return feet.ToString("D2") + "-" + inch.ToString("D2") + "-" + fract.ToString("D2");
            }

            return feet.ToString("D2") + "-" + inch.ToString("D2") + "-" + fract.ToString("D1");
        }
    }
}
