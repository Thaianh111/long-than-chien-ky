// Giả lập vài thứ của NUnit để chạy test ngoài Unity (trong Unity dùng NUnit thật).
using System;
namespace NUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method)] public class TestAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Method)] public class SetUpAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class)] public class TestFixtureAttribute : Attribute { }
    public class AssertionException : Exception { public AssertionException(string m) : base(m) { } }
    public static class Assert
    {
        public static void AreEqual(object e, object a, string m = null) { if (!Equals(Convert.ToDouble(e), Convert.ToDouble(a)) && !Equals(e, a)) throw new AssertionException($"Mong {e}, nhận {a}. {m}"); }
        public static void AreEqual(float e, float a, float tol, string m = null) { if (Math.Abs(e - a) > tol) throw new AssertionException($"Mong {e}±{tol}, nhận {a}. {m}"); }
        public static void AreNotEqual(object e, object a, string m = null) { if (Equals(e, a)) throw new AssertionException($"Không mong {e}. {m}"); }
        public static void IsTrue(bool c, string m = null) { if (!c) throw new AssertionException("Mong đúng. " + m); }
        public static void IsFalse(bool c, string m = null) { if (c) throw new AssertionException("Mong sai. " + m); }
        public static void IsNotNull(object o, string m = null) { if (o == null) throw new AssertionException("Mong khác null. " + m); }
        public static void Greater(double a, double b, string m = null) { if (!(a > b)) throw new AssertionException($"Mong {a} > {b}. {m}"); }
        public static void GreaterOrEqual(double a, double b, string m = null) { if (!(a >= b)) throw new AssertionException($"Mong {a} >= {b}. {m}"); }
        public static void LessOrEqual(double a, double b, string m = null) { if (!(a <= b)) throw new AssertionException($"Mong {a} <= {b}. {m}"); }
    }
}
