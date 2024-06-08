using SeaDrop;
using static DxLibDLL.DX;

namespace TestApp
{
    public class MortorTest : Scene
    {
        public static Mortor Mortor = new Mortor();
        public override void Enable()
        {
            Mortor = new Mortor()
            {
                Max = 1,
                Min = 0,
                Loop = true,
            };
            base.Enable();
        }

        private void Timer_Looped(object? sender, EventArgs e)
        {
            //if (Graph != null && Graph.ReferencePoint++ >= ReferencePoint.BottomRight) { Graph.ReferencePoint = ReferencePoint.TopLeft; }
        }

        public override void Draw()
        {
            DrawString(20, 20, $"{FPS.Value}", 0x00ff00);
            DrawString(20, 40, $"{FPS.AverageValue}", 0x00ff00);
            DrawString(20, 80, $"{FPS.Process}", 0x00ff80);
            DrawString(20, 100, $"{FPS.AverageProcess}", 0x00ff80);

            DrawString(200, 200, $"Value:{Mortor.Value}", 0x00ff80);
            DrawString(200, 220, $"Speed:{Mortor.Speed}", 0x00ff80);
            if (Mortor.stopvalue.HasValue) DrawString(100, 240, $"Stop:{Mortor.stopvalue.Value}", 0x00ff80);
            for (int i = 0; i < Mortor.Actions.Count; i++)
            {
                var action = Mortor.Actions[i];
                DrawString(200, 240 + 20 * i, $"{action}", 0x00ff80);
            }

            DrawCircle(120, 240, 80, 0xffffff);
            DrawCircle(120, 240, 80, 0x0000ff, FALSE);
            double rad = 2 * Math.PI * Mortor.Value / (Mortor.Max.HasValue ? (double)Mortor.Max.Value : 1.0);
            var sin = 80.0 * Math.Sin(rad);
            var cos = 80.0 * -Math.Cos(rad);
            DrawLine(120, 240, 120 + (int)sin, 240 + (int)cos, 0xff0000, 4);

            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(EKey.Esc)) DXLib.End();
            Mortor.Update();
            int val = Key.IsPushing(EKey.LShift) ? 200 : 1000;
            if (Key.IsPushed(EKey.S))
            {
                Mortor.Start(val);
            }
            if (Key.IsPushed(EKey.D))
            {
                Mortor.Start(-val);
            }
            if (Key.IsPushed(EKey.X))
            {
                Mortor.Start(1000, val);
            }
            if (Key.IsPushed(EKey.C))
            {
                Mortor.Start(1000, -val);
            }
            if (Key.IsPushed(EKey.V))
            {
                Mortor.Start(1000, 0);
            }
            if (Key.IsPushed(EKey.A))
            {
                Mortor.Start(1000, 0.5, Key.IsPushing(EKey.LShift), EEasing.Circ);
            }
            if (Key.IsPushed(EKey.F))
            {
                Mortor.Stop();
            }
            if (Key.IsPushed(EKey.G))
            {
                Mortor.Stop(Key.IsPushing(EKey.LShift) ? -5 : 5);
            }
            if (Key.IsPushed(EKey.Z))
            {
                Mortor.Actions.Clear();
            }


            base.Update();
        }
    }
}
