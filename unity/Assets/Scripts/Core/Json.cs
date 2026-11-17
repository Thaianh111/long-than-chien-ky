// Đọc JSON nhỏ gọn (không cần thư viện ngoài) và đổ vào lớp C# theo tên trường.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace LongThan.Core
{
    /// <summary>Bộ đọc JSON tối giản: object → Dictionary, mảng → List, số → double.</summary>
    public static class Json
    {
        public static object Parse(string s) { int i = 0; var v = Value(s, ref i); return v; }

        static void Ws(string s, ref int i) { while (i < s.Length && char.IsWhiteSpace(s[i])) i++; }
        static object Value(string s, ref int i)
        {
            Ws(s, ref i);
            char c = s[i];
            if (c == '{') return Obj(s, ref i);
            if (c == '[') return Arr(s, ref i);
            if (c == '"') return Str(s, ref i);
            if (s.Substring(i).StartsWith("true")) { i += 4; return true; }
            if (s.Substring(i).StartsWith("false")) { i += 5; return false; }
            if (s.Substring(i).StartsWith("null")) { i += 4; return null; }
            int st = i; while (i < s.Length && "+-0123456789.eE".IndexOf(s[i]) >= 0) i++;
            return double.Parse(s.Substring(st, i - st), CultureInfo.InvariantCulture);
        }
        static Dictionary<string, object> Obj(string s, ref int i)
        {
            var d = new Dictionary<string, object>(); i++;
            Ws(s, ref i); if (s[i] == '}') { i++; return d; }
            while (true)
            {
                Ws(s, ref i); var k = Str(s, ref i); Ws(s, ref i); i++; // ':'
                d[k] = Value(s, ref i); Ws(s, ref i);
                if (s[i] == ',') { i++; continue; }
                i++; return d; // '}'
            }
        }
        static List<object> Arr(string s, ref int i)
        {
            var l = new List<object>(); i++;
            Ws(s, ref i); if (s[i] == ']') { i++; return l; }
            while (true)
            {
                l.Add(Value(s, ref i)); Ws(s, ref i);
                if (s[i] == ',') { i++; continue; }
                i++; return l;
            }
        }
        static string Str(string s, ref int i)
        {
            var sb = new StringBuilder(); i++;
            while (s[i] != '"')
            {
                if (s[i] == '\\')
                {
                    i++; char e = s[i];
                    if (e == 'u') { sb.Append((char)System.Convert.ToInt32(s.Substring(i + 1, 4), 16)); i += 4; }
                    else sb.Append(e == 'n' ? '\n' : e == 't' ? '\t' : e);
                }
                else sb.Append(s[i]);
                i++;
            }
            i++; return sb.ToString();
        }

        /// <summary>Đổ dữ liệu JSON vào đối tượng có sẵn: trường public trùng tên khóa thì gán.</summary>
        public static T Bind<T>(object src) where T : new() => (T)Convert(src, typeof(T));

        static object Convert(object v, Type t)
        {
            if (v == null) return null;
            if (t == typeof(float)) return (float)(double)v;
            if (t == typeof(int)) return (int)(double)v;
            if (t == typeof(double)) return (double)v;
            if (t == typeof(bool)) return (bool)v;
            if (t == typeof(string)) return (string)v;
            if (t.IsArray)
            {
                var src = (List<object>)v; var et = t.GetElementType(); var arr = Array.CreateInstance(et, src.Count);
                for (int k = 0; k < src.Count; k++) arr.SetValue(Convert(src[k], et), k);
                return arr;
            }
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var vt = t.GetGenericArguments()[1]; var dict = (IDictionary)Activator.CreateInstance(t);
                foreach (var kv in (Dictionary<string, object>)v) dict[kv.Key] = Convert(kv.Value, vt);
                return dict;
            }
            var o = Activator.CreateInstance(t); var map = (Dictionary<string, object>)v;
            foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
                if (map.TryGetValue(f.Name, out var fv)) f.SetValue(o, Convert(fv, f.FieldType));
            return o;
        }
    }
}
