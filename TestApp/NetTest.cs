using SeaDrop;

namespace TestApp
{
    public class NetTest : Scene
    {
        public static string URL = "http://www.spicats.com";
        public static string NetState = "";
        public static List<string> NetData = new List<string>();
        public override void Enable()
        {
            var net = Internet.Connect(URL);
            NetState = net.State;
            NetData.Clear();
            if (net.Data != null) foreach (var line in net.Data.Result.Split('\n'))
            {
                NetData.Add(line.Trim());
            }

            Tcp.OpenServer();

            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }

        public override void Draw()
        {
            Drawing.Text(20, 0, NetState);
            for (int i = 0; i < NetData.Count; i++)
            {
                Drawing.Text(20, 40 + 20 * i, NetData[i]);
            }

            Drawing.Text(400, 0, $"{Tcp.HostIP} : {Tcp.HostPort}");
            int n = 0;
            for (int i = Tcp.Sends.Count > 34 ? Tcp.Sends.Count - 34 : 0; i < Tcp.Sends.Count; i++)
            {
                Drawing.Text(400, 20 + 20 * n, Tcp.Sends[i]);
                n++;
            }

            base.Draw();
        }

        public override void Update()
        {
            if (Key.IsPushed(EKey.Esc)) DXLib.End();
            if (Key.IsPushed(EKey.Enter)) Tcp.SendClient($"{Environment.UserName}({Environment.MachineName}) : {Tcp.Sends.Count}");
            base.Update();
        }
    }
}
