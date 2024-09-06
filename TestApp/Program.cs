using SeaDrop;

namespace TestApp
{
    public class Program : Scene
    {
        public static void Main(string[] args)
        {
            DXLib.VSync = true;
            DXLib.MultiThreading = true;
            DXLib.Init(new Program());
            //1280, 720, 640, 480
            //
            //Shooting
            //NetTest
            //MortorTest
        }

        Animation animation = new();

        public override void Enable()
        {
            animation = new("Anim\\Anim.txt");
            base.Enable();
        }

        public override void Draw()
        {
            animation.Draw();
            Drawing.Text(400, 0, FPS.AverageValue);
            Drawing.Text(400, 20, FPS.AverageProcess);
            Drawing.Text(100, 20, animation.Timer);

            base.Draw();
        }
        public override void Update()
        {
            if (Key.IsPushed(EKey.Esc)) DXLib.End();
            if (Key.IsPushed(EKey.Space)) animation.Start();

            base.Update();
        }
    }
}