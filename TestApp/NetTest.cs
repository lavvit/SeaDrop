using SeaDrop;

namespace TestApp
{
    public class NetTest : Scene
    {
        public override void Enable()
        {
            Tcp.OpenServer();
            Udp.OpenServer();

            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }

        public override void Draw()
        {
            Drawing.Text(100, 0, $"{Udp.HostIP} : {Udp.HostPort}");
            int m = 0;
            for (int i = Udp.Sends.Count > 20 ? Udp.Sends.Count - 20 : 0; i < Udp.Sends.Count; i++)
            {
                Drawing.Text(100, 20 + 20 * m, Udp.Sends[i]);
                m++;
            }

            Drawing.Text(400, 0, $"{Tcp.HostIP} : {Tcp.HostPort}");
            int n = 0;
            for (int i = Tcp.Sends.Count > 20 ? Tcp.Sends.Count - 20 : 0; i < Tcp.Sends.Count; i++)
            {
                Drawing.Text(400, 20 + 20 * n, Tcp.Sends[i]);
                n++;
            }

            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(EKey.Esc)) DXLib.End();

            if (Key.IsPushed(EKey.Q)) Udp.Sends.Clear();
            if (Key.IsPushed(EKey.W)) Tcp.Sends.Clear();

            if (Key.IsPushed(EKey.Space)) Udp.SendClient($"{Environment.UserName}({Environment.MachineName}) : {Udp.Sends.Count}");
            if (Key.IsPushed(EKey.Enter)) Tcp.SendClient($"{Environment.UserName}({Environment.MachineName}) : {Tcp.Sends.Count}");
            base.Update();
        }
    }
}
