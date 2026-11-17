// Chạy mọi hàm [Test] trong các lớp test; in kết quả. Mã thoát 1 nếu có test hỏng.
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

static class Runner
{
    static int Main()
    {
        string data = Path.Combine(AppContext.BaseDirectory, "../../../../../Assets/Resources/Data");
        LongThan.Tests.TestData.Dir = Path.GetFullPath(data);
        int pass = 0, fail = 0;
        foreach (var t in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetMethods().Any(m => m.GetCustomAttribute<TestAttribute>() != null)))
        {
            foreach (var m in t.GetMethods().Where(m => m.GetCustomAttribute<TestAttribute>() != null))
            {
                var o = Activator.CreateInstance(t);
                try
                {
                    foreach (var s in t.GetMethods().Where(x => x.GetCustomAttribute<SetUpAttribute>() != null)) s.Invoke(o, null);
                    m.Invoke(o, null); pass++; Console.WriteLine($"  ✓ {t.Name}.{m.Name}");
                }
                catch (TargetInvocationException e) { fail++; Console.WriteLine($"  ✗ {t.Name}.{m.Name}: {e.InnerException?.Message}"); }
            }
        }
        Console.WriteLine($"{pass} đạt, {fail} hỏng"); if (Environment.GetEnvironmentVariable("XCHECK") == "1") XCheck.Run();
        return fail == 0 ? 0 : 1;
    }
}
