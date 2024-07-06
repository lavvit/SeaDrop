using SeaDrop;

namespace TestApp
{
    public class Program : Scene
    {
        public static void Main(string[] args)
        {
            DXLib.VSync = true;
            DXLib.MultiThreading = true;
            DXLib.Init(new NetTest(), 640, 480);
            //1280, 720
            //Program
            //Shooting
            //
            //MortorTest
        }

        public static int Speaker = 1;

        public override void Enable()
        {
            Input.Init();
            base.Enable();
        }

        public override void Draw()
        {
            Drawing.Text(400, 0, FPS.AverageValue);
            Drawing.Text(400, 20, FPS.AverageProcess);

            Drawing.Text(100, 200, $"Speaker:{Speaker}");
            Input.Draw(100, 240);

            base.Draw();
        }
        public override void Update()
        {
            if (Key.IsPushed(EKey.Esc)) DXLib.End();
            if (Key.IsPushed(EKey.F1)) Speaker--;
            if (Key.IsPushed(EKey.F2)) Speaker++;


            var str = Input.Enter();
            if (str != null)
            {
                Voicevox.Play(str, Speaker);
                Input.Init();
            }

            base.Update();
        }
    }
}