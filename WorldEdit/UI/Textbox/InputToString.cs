using OnixRuntime.Api.Inputs;

namespace WorldEdit.UI.Textbox;

class ThisShouldBeInJsonButIsnt
{
    public Dictionary<(InputKey, bool), string> GetDictionary()
    {
        Dictionary<(InputKey, bool), string> d = new Dictionary<(InputKey, bool), string>();

        d.Add((InputKey.Type.A, true),  "A");
        d.Add((InputKey.Type.A, false), "a");
        d.Add((InputKey.Type.B, true),  "B");
        d.Add((InputKey.Type.B, false), "b");
        d.Add((InputKey.Type.C, true),  "C");
        d.Add((InputKey.Type.C, false), "c");
        d.Add((InputKey.Type.D, true),  "D");
        d.Add((InputKey.Type.D, false), "d");
        d.Add((InputKey.Type.E, true),  "E");
        d.Add((InputKey.Type.E, false), "e");
        d.Add((InputKey.Type.F, true),  "F");
        d.Add((InputKey.Type.F, false), "f");
        d.Add((InputKey.Type.G, true),  "G");
        d.Add((InputKey.Type.G, false), "g");
        d.Add((InputKey.Type.H, true),  "H");
        d.Add((InputKey.Type.H, false), "h");
        d.Add((InputKey.Type.I, true),  "I");
        d.Add((InputKey.Type.I, false), "i");
        d.Add((InputKey.Type.J, true),  "J");
        d.Add((InputKey.Type.J, false), "j");
        d.Add((InputKey.Type.K, true),  "K");
        d.Add((InputKey.Type.K, false), "k");
        d.Add((InputKey.Type.L, true),  "L");
        d.Add((InputKey.Type.L, false), "l");
        d.Add((InputKey.Type.M, true),  "M");
        d.Add((InputKey.Type.M, false), "m");
        d.Add((InputKey.Type.N, true),  "N");
        d.Add((InputKey.Type.N, false), "n");
        d.Add((InputKey.Type.O, true),  "O");
        d.Add((InputKey.Type.O, false), "o");
        d.Add((InputKey.Type.P, true),  "P");
        d.Add((InputKey.Type.P, false), "p");
        d.Add((InputKey.Type.Q, true),  "Q");
        d.Add((InputKey.Type.Q, false), "q");
        d.Add((InputKey.Type.R, true),  "R");
        d.Add((InputKey.Type.R, false), "r");
        d.Add((InputKey.Type.S, true),  "S");
        d.Add((InputKey.Type.S, false), "s");
        d.Add((InputKey.Type.T, true),  "T");
        d.Add((InputKey.Type.T, false), "t");
        d.Add((InputKey.Type.U, true),  "U");
        d.Add((InputKey.Type.U, false), "u");
        d.Add((InputKey.Type.V, true),  "V");
        d.Add((InputKey.Type.V, false), "v");
        d.Add((InputKey.Type.W, true),  "W");
        d.Add((InputKey.Type.W, false), "w");
        d.Add((InputKey.Type.X, true),  "X");
        d.Add((InputKey.Type.X, false), "x");
        d.Add((InputKey.Type.Y, true),  "Y");
        d.Add((InputKey.Type.Y, false), "y");
        d.Add((InputKey.Type.Z, true),  "Z");
        d.Add((InputKey.Type.Z, false), "z");
        
        
        
        d.Add((InputKey.Type.Num1, false), "1");
        d.Add((InputKey.Type.Num1, true),  "!");
        d.Add((InputKey.Type.Num2, false), "2");
        d.Add((InputKey.Type.Num2, true),  "@");
        d.Add((InputKey.Type.Num3, false), "3");
        d.Add((InputKey.Type.Num3, true),  "#");
        d.Add((InputKey.Type.Num4, false), "4");
        d.Add((InputKey.Type.Num4, true),  "$");
        d.Add((InputKey.Type.Num5, false), "5");
        d.Add((InputKey.Type.Num5, true),  "%");
        d.Add((InputKey.Type.Num6, false), "6");
        d.Add((InputKey.Type.Num6, true),  "^");
        d.Add((InputKey.Type.Num7, false), "7");
        d.Add((InputKey.Type.Num7, true),  "&");
        d.Add((InputKey.Type.Num8, false), "8");
        d.Add((InputKey.Type.Num8, true),  "*");
        d.Add((InputKey.Type.Num9, false), "9");
        d.Add((InputKey.Type.Num9, true),  "(");
        d.Add((InputKey.Type.Num0, false), "0");
        d.Add((InputKey.Type.Num0, true),  ")");
        
        
        
        d.Add((InputKey.Type.Substract, false), "-");
        d.Add((InputKey.Type.Substract, true),  "_");
        d.Add((InputKey.Type., false), "=");
        d.Add((InputKey.Type.Equals, true),  "+");
        d.Add((InputKey.Type.LeftBracket, false), "[");
        d.Add((InputKey.Type.LeftBracket, true),  "{");
        d.Add((InputKey.Type.RightBracket, false), "]");
        d.Add((InputKey.Type.RightBracket, true),  "}");
        d.Add((InputKey.Type.Backslash, false), "\\");
        d.Add((InputKey.Type.Backslash, true),  "|");
        d.Add((InputKey.Type.Semicolon, false), ";");
        d.Add((InputKey.Type.Semicolon, true),  ":");
        d.Add((InputKey.Type.Quote, false), "'");
        d.Add((InputKey.Type.Quote, true),  "\"");
        d.Add((InputKey.Type.Comma, false), ",");
        d.Add((InputKey.Type.Comma, true),  "<");
        d.Add((InputKey.Type.Dot, false), ".");
        d.Add((InputKey.Type.Dot, true),  ">");
        d.Add((InputKey.Type.Slash, false), "/");
        d.Add((InputKey.Type.Slash, true),  "?");
        d.Add((InputKey.Type.Grave, false), "`");
        d.Add((InputKey.Type.Grave, true),  "~");
        
        return d;
    }
}
