using System; using System.Collections.Generic; using LongThan.Core; using LongThan.Tests;
public static class XCheck { public static void Run() {
  var g = TestData.MakeGame(1); var o = new List<string>();
  var plan = new (int t, Key k, bool v)[] { (0,Key.Right,true),(60,Key.Jump,true),(80,Key.Jump,false),(130,Key.Dash,true),(160,Key.Dash,false),(200,Key.Right,false),(210,Key.Attack,true),(212,Key.Attack,false),(300,Key.Right,true),(340,Key.Jump,true),(345,Key.Jump,false),(360,Key.Jump,true),(365,Key.Jump,false) };
  for (int i=0;i<600;i++){ foreach(var p in plan) if(p.t==i){ g.input.held[(int)p.k]=p.v; if(p.v) g.input.pressed[(int)p.k]=true; else g.input.released[(int)p.k]=true; } Flow.Step(g); if(i%50==49) o.Add($"[{i},{Math.Round(g.P.x,2)},{Math.Round(g.P.y,2)}]"); }
  Console.WriteLine("XCHECK ["+string.Join(",",o)+"]"); } }
