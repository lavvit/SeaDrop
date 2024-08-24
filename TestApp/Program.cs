using SeaDrop;

namespace TestApp
{
    public class Program : Scene
    {
        public static void Main(string[] args)
        {
            DXLib.VSync = true;
            DXLib.MultiThreading = true;
            DXLib.Init(new Program(), 640, 480);
            //1280, 720
            //
            //Shooting
            //NetTest
            //MortorTest
        }

        Sound Sound = new();

        public override void Enable()
        {
            Input.Init();
            Sound = new("Amazing Mirage.ogg");
            Sound.Play();

            base.Enable();
        }

        public override void Draw()
        {
            Drawing.Text(400, 0, FPS.AverageValue);
            Drawing.Text(400, 20, FPS.AverageProcess);

            base.Draw();
        }
        public override void Update()
        {
            if (Key.IsPushed(EKey.Esc)) DXLib.End();

            base.Update();
        }
    }
}