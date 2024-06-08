using SeaDrop;
namespace TestApp
{
    public class Shooting : Scene
    {
        public static Chara player = new Chara();
        public static Chara enemy = new Chara();
        public static int max, move, etick;
        public static List<Sprite> Shots = new List<Sprite>();
        public static List<Sprite> EShots = new List<Sprite>();

        public override void Enable()
        {
            player.texture = new Texture(@"Skin\player.png");
            player.x = -64;
            player.y = 288;
            player.width = player.texture.Width;
            player.height = player.texture.Height;
            player.size = 8;

            enemy.texture = new Texture(@"Skin\enemy.png");
            enemy.x = 0;
            enemy.y = 50;
            enemy.width = enemy.texture.Width;
            enemy.height = enemy.texture.Height;
            enemy.size = 8;

            Init();

            max = 10;
            move = 1;

            Shots = new List<Sprite>();
            EShots = new List<Sprite>();
            base.Enable();
        }

        public static void Init()
        {
            player.damage = 0;
            player.dtick = 0;
            player.hp = 5;

            enemy.damage = 0;
            enemy.dtick = 0;
            enemy.hp = 20;
            etick = 0;
        }

        public override void Disable()
        {
            base.Disable();
        }

        public override void Draw()
        {
            Drawing.Text(0, 0, FPS.AverageValue, 0x00ff00);
            Drawing.Text(0, 20, FPS.AverageProcess, 0x00ffff);
            enemy.Draw();
            player.Draw();
            Drawing.Text(enemy.x + enemy.width, enemy.y + enemy.height - 20, enemy.hp);
            Drawing.Text(player.x + player.width, player.y + player.height - 20, player.hp);
            try
            {
                foreach (Sprite s in Shots)
                {
                    if (s != null) Drawing.Box(s.x, s.y, 8, 8, 0xffff00);
                }
                foreach (Sprite s in EShots)
                {
                    if (s != null) Drawing.Box(s.x, s.y, 8, 8, 0xff0000);
                }
            }
            catch { }
            base.Draw();
        }

        public override void Update()
        {
            // 顔を歪めているかどうかで処理を分岐
            if (player.hp == 0)
            {
                player.texture.Color = System.Drawing.Color.DarkRed;
            }
            else if (player.damage == 1)
            {
                double damaged = 255 * (player.dtick / 30.0);
                // 顔を歪めている場合はダメージ時のグラフィックを描画する
                player.texture.Color = System.Drawing.Color.FromArgb(255, 255, (int)damaged, (int)damaged);

                // 顔を歪めている時間を測るカウンターに１を加算する
                player.dtick++;

                // もし顔を歪め初めて ３０ フレーム経過していたら顔の歪んだ状態から
                // 元に戻してあげる
                if (player.dtick == 30)
                {
                    // 『歪んでいない』を表す０を代入
                    player.damage = 0;
                }
            }
            else
            {
                player.texture.Color = System.Drawing.Color.White;
                if (Key.IsPushing(EKey.Left)) player.x -= 3;
                if (Key.IsPushing(EKey.Right)) player.x += 3;
                if (Key.IsPushing(EKey.Up)) player.y -= 3;
                if (Key.IsPushing(EKey.Down)) player.y += 3;

                // スペースキーを押していて、且弾が撃ち出されていなかったら弾を発射する
                if (Key.IsPushed(EKey.Space) && Shots.Count < max)
                {
                    int Bw = 40, Bh = 40, Sw = 8, Sh = 8;

                    Shots.Add(new Sprite()
                    {
                        x = (Bw - Sw) / 2 + player.x,
                        y = (Bh - Sh) / 2 + player.y,
                        width = 8,
                        height = 8,
                    });
                }
                if (player.x < 0) player.x = 0;
                if (player.x > 640 - 64) player.x = 640 - 64;
                if (player.y < 0) player.y = 0;
                if (player.y > 480 - 64) player.y = 480 - 64;
            }

            // 自機の弾の移動ルーチン( 存在状態を保持している変数の内容が１(存在する)の場合のみ行う )
            for (int i = Shots.Count - 1; i >= 0; i--)
            {
                var s = Shots[i];
                // 弾を１６ドット上に移動させる
                s.y -= 16;

                // 画面外に出てしまった場合は存在状態を保持している変数に０(存在しない)を代入する
                if (s.y < -80)
                {
                    Shots.RemoveAt(i);
                }
            }

            // 顔を歪めているかどうかで処理を分岐
            if (enemy.hp == 0)
            {
                enemy.texture.Color = System.Drawing.Color.DarkRed;
            }
            else if (enemy.damage == 1)
            {
                double damaged = 255 * (enemy.dtick / 30.0);
                // 顔を歪めている場合はダメージ時のグラフィックを描画する
                enemy.texture.Color = System.Drawing.Color.FromArgb(255, 255, (int)damaged, (int)damaged);

                // 顔を歪めている時間を測るカウンターに１を加算する
                enemy.dtick++;

                // もし顔を歪め初めて ３０ フレーム経過していたら顔の歪んだ状態から
                // 元に戻してあげる
                if (enemy.dtick == 30)
                {
                    // 『歪んでいない』を表す０を代入
                    enemy.damage = 0;
                }
            }
            else
            {
                enemy.texture.Color = System.Drawing.Color.White;
                if (move == 1) enemy.x += 3;
                if (move == 0) enemy.x -= 3;

                if (enemy.x > 576)
                {
                    enemy.x = 576;
                    move = 0;
                }

                if (enemy.x < 0)
                {
                    enemy.x = 0;
                    move = 1;
                }

                // 弾を撃つタイミングを計測するためのカウンターに１を足す
                if (player.hp > 0) etick++;

                // もしカウンター変数が６０だった場合は弾を撃つ処理を行う
                if (etick == 60)
                {
                    double esx = (enemy.width - enemy.size) / 2 + enemy.x;
                    double esy = (enemy.height - enemy.size) / 2 + enemy.y;
                    // 弾の移動速度を設定する
                    double sb, sbx, sby, bx, by, sx, sy;

                    sx = esx + enemy.size / 2;
                    sy = esy + enemy.size / 2;

                    bx = player.x + player.width / 2;
                    by = player.y + player.height / 2;

                    sbx = bx - sx;
                    sby = by - sy;

                    // 平方根を求めるのに標準関数の sqrt を使う、
                    // これを使うには math.h をインクルードする必要がある
                    sb = Math.Sqrt(sbx * sbx + sby * sby);

                    // もし既に弾が『飛んでいない』状態だった場合のみ発射処理を行う
                    // 弾の発射位置を設定する
                    // 弾の状態を保持する変数に『飛んでいる』を示す１を代入する
                    EShots.Add(new Sprite()
                    {
                        x = esx,
                        y = esy,
                        width = enemy.size,
                        height = enemy.size,
                        momentx = sbx / sb * 8,
                        momenty = sby / sb * 8
                    });

                    // 弾を打つタイミングを計測するための変数に０を代入
                    etick = 0;
                }
            }

            // 敵の弾の状態が『飛んでいる』場合のみ弾の移動処理を行う
            for (int i = EShots.Count - 1; i >= 0; i--)
            {
                var s = EShots[i];
                // 弾を移動させる
                s.x += s.momentx;
                s.y += s.momenty;

                // もし弾が画面からはみ出てしまった場合は弾の状態を『飛んでいない』
                // を表す０にする
                if (s.y > 480 || s.y < 0 ||
                    s.x > 640 || s.x < 0)
                    EShots.RemoveAt(i);
            }

            if (enemy.hp > 0)
            {
                // 弾と敵の当たり判定、弾の数だけ繰り返す
                for (int i = Shots.Count - 1; i >= 0; i--)
                {
                    var s = Shots[i];
                    // 四角君との当たり判定
                    if (((s.x > enemy.x && s.x < enemy.x + enemy.width) ||
                        (enemy.x > s.x && enemy.x < s.x + s.width)) &&
                        ((s.y > enemy.y && s.y < enemy.y + enemy.height) ||
                        (enemy.y > s.y && enemy.y < s.y + s.height)))
                    {
                        // 接触している場合は当たった弾の存在を消す
                        Shots.RemoveAt(i);

                        // 四角君の顔を歪めているかどうかを保持する変数に『歪めている』を表す１を代入
                        enemy.damage = 1;

                        // 四角君の顔を歪めている時間を測るカウンタ変数に０を代入
                        enemy.dtick = 0;
                        if (enemy.hp-- <= 0) enemy.hp = 0;
                    }
                }
            }
            if (player.hp > 0)
            {
                // 弾と自機の当たり判定、弾の数だけ繰り返す
                for (int i = EShots.Count - 1; i >= 0; i--)
                {
                    var s = EShots[i];
                    // 自機との当たり判定
                    if (((s.x > player.x && s.x < player.x + player.width) ||
                        (player.x > s.x && player.x < s.x + s.width)) &&
                        ((s.y > player.y && s.y < player.y + player.height) ||
                        (player.y > s.y && player.y < s.y + s.height)))
                    {
                        // 接触している場合は当たった弾の存在を消す
                        EShots.RemoveAt(i);

                        // 自機の顔を歪めているかどうかを保持する変数に『歪めている』を表す１を代入
                        player.damage = 1;

                        // 自機の顔を歪めている時間を測るカウンタ変数に０を代入
                        player.dtick = 0;
                        if (player.hp-- <= 0) player.hp = 0;
                    }
                }
            }

            Thread.Sleep(8);
            if (Key.IsPushed(EKey.Q)) Init();
            if (Key.IsPushed(EKey.Esc)) DXLib.End();
            base.Update();
        }
    }

    public class Chara : Sprite
    {
        public int size;
        public int damage, dtick;

        public int hp;
    }
}
