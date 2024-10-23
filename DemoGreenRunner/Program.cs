using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Net.Http.Headers;
using System.Xml.Linq;
using System.Drawing;
using static Game.Program;
using System.Net.NetworkInformation;
using System.Numerics;

namespace Game
{
    //Program chính
    public class Program
    {
        static int width = 50, height = 27;
        public static object consoleLock = new object();
        private static bool isPause;
        //private static bool isResume;
        //private static bool score;
        //private static string highestScore;
        //private static string level;
        static Point user = new Point(0, 0);
        static Point head_user = new Point(0, 0);
        static Point fish = new Point(0, 0);
        static Point plastic_bag = new Point(0, 0);
        static Point glass_bottle = new Point(0, 0);
        static Point block = new Point(0, 0);
        static int score = 0;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            Map.DrawMap(width, height);


            Thread t1 = new Thread(() =>
            {
                User.Info_User();
            });
            t1.Start();
            t1.Priority = ThreadPriority.Highest;
            Thread t2 = new Thread(() =>
            {
                Block.BlockImage();
            });
            t2.Start();
            t2.Priority = ThreadPriority.Lowest;
            Thread t3 = new Thread(() =>
            {

                Trash.Fish();
            });
            t3.Start();
            t3.Priority = ThreadPriority.Lowest;
            Thread t4 = new Thread(() =>
            {

                Trash.Plastic_bag();
            });
            t4.Start();
            t4.Priority = ThreadPriority.Lowest;
            Thread t5 = new Thread(() =>
            {

                Trash.Glass_Bottle();
            });
            t5.Start();
            t5.Priority = ThreadPriority.Lowest;

            SaveScore("ham", score);
            if (Console.KeyAvailable)
            {
                // Đọc phím được nhấn
                var keyInfo = Console.ReadKey(true);

                // Kiểm tra nếu phím là "Esc"
                if (keyInfo.Key == ConsoleKey.Escape)
                {

                    Console.Clear();

                    // Thoát vòng lặp => kết thúc game
                    ScoreBoard();
                }
            }
        }




        static bool isHit(Point head_user)
        {
            foreach (var trash in trash)
            {
                if (head_user.IsEqual(trash)) return true;
            }
            return false;
        }

        static void Restart()
        {
            score = 0;

        }
        //Kiểm tra thức ăn có trùng với vật cản động không
        //static bool IsCollideBlocks(Point food)
        //{
        //    foreach (HazardBlock block in blocks)
        //    {
        //        if (food.IsEqual(block.Block)) return true;

        //    }
        //    return false;
        //}
        static List<Point> trash = new List<Point>(); //Mảng chứa tọa độ các vật cản

        #region ScoreBoard
        static void countScore(Point head_user)
        {
            if (isHit(head_user))
                score += 10;

            else if (isHit(head_user))
                score -= 15;

        }
        public static void SaveScore(string playerName, int score)
        {
            string filePath = "scores.txt";
            string scoreEntry = $"{playerName},{score}";
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(scoreEntry);
            }
        }


        static void ScoreBoard()
        {
            string filePath = "scores.txt";

            // Đọc tất cả dòng từ file
            string[] lines = File.ReadAllLines(filePath);

            // Tạo mảng để lưu điểm và tên
            (string playerName, int score)[] scores = new (string playerName, int score)[lines.Length];

            // Lặp qua từng dòng và thêm vào mảng
            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length == 2 && int.TryParse(parts[1], out int nscore))
                {
                    scores[i] = (parts[0], nscore); // Gán vào mảng
                }
            }

            //Sắp xếp mảng theo điểm giảm dần(bubble sort)
            for (int i = 0; i < scores.Length - 1; i++)
            {
                for (int j = 0; j < scores.Length - i - 1; j++)
                {
                    if (scores[j].score < scores[j + 1].score)
                    {
                        // Hoán đổi vị trí
                        var temp = scores[j];
                        scores[j] = scores[j + 1];
                        scores[j + 1] = temp;
                    }
                }
            }


            // Hiển thị bảng xếp hạng
            Console.WriteLine("Leaderboard:");
            for (int i = 0; i < scores.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {scores[i].playerName} - {scores[i].score} points");
            }
        }
        #endregion

        public class Map
        {
            public static void DrawMap(int width, int height)
            {
                for (int i = 0; i <= height; i++)
                {
                    for (int j = 0; j <= width * 2; j++)
                    {
                        // Khung ngoài
                        if (i == 0)//khung trên
                        {
                            Console.Write("▄");
                        }
                        else if (i == height)//khung dưới
                        {
                            Console.Write("▀");
                        }
                        else if (j == 0)//khung trái
                        {
                            Console.Write("▌");
                        }
                        else if (j == width * 2)//khung phải
                        {
                            Console.Write("▐");
                        }
                        // Đường line chia ngang 1
                        else if (i == height / 3)
                        {
                            Console.Write("─");
                        }
                        // Đường line chia ngang 2
                        else if (i == 2 * height / 3)
                        {
                            Console.Write("─");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                    Console.WriteLine();
                }
            }

            static void NameGame()//Logo của game khi mới hiện lên
            {
                int count = 0;
                string[] Name = new string[]
                {
@"▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒",
@"▒▒▒▒▒██████▒██████▒███████▒███████▒██▒▒▒▒█▒▒▒▒▒",
@"▒▒▒▒▒█▒▒▒▒█▒█▒▒▒▒█▒█▒▒▒▒▒▒▒█▒▒▒▒▒▒▒███▒▒▒█▒▒▒▒▒",
@"▒▒▒▒▒█▒▒▒▒█▒██████▒█▒▒▒▒▒▒▒█▒▒▒▒▒▒▒█▒██▒▒█▒▒▒▒▒",
@"▒▒▒▒▒█▒▒▒▒▒▒██▒▒▒▒▒███████▒███████▒█▒▒█▒▒█▒▒▒▒▒",
@"▒▒▒▒▒█▒▒███▒█▒██▒▒▒█▒▒▒▒▒▒▒█▒▒▒▒▒▒▒█▒▒██▒█▒▒▒▒▒",
@"▒▒▒▒▒█▒▒▒▒█▒█▒▒██▒▒█▒▒▒▒▒▒▒█▒▒▒▒▒▒▒█▒▒▒███▒▒▒▒▒",
@"▒▒▒▒▒██████▒█▒▒▒██▒███████▒███████▒█▒▒▒▒██▒▒▒▒▒",
@"▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒",
@"▒▒██████▒█▒▒▒▒█▒██▒▒▒▒█▒██▒▒▒▒█▒██████▒██████▒▒",
@"▒▒█▒▒▒▒█▒█▒▒▒▒█▒███▒▒▒█▒███▒▒▒█▒█▒▒▒▒▒▒█▒▒▒▒█▒▒",
@"▒▒██████▒█▒▒▒▒█▒█▒██▒▒█▒█▒██▒▒█▒█▒▒▒▒▒▒██████▒▒",
@"▒▒██▒▒▒▒▒█▒▒▒▒█▒█▒▒█▒▒█▒█▒▒█▒▒█▒██████▒██▒▒▒▒▒▒",
@"▒▒█▒██▒▒▒█▒▒▒▒█▒█▒▒██▒█▒█▒▒██▒█▒█▒▒▒▒▒▒█▒██▒▒▒▒",
@"▒▒█▒▒██▒▒█▒▒▒▒█▒█▒▒▒███▒█▒▒▒███▒█▒▒▒▒▒▒█▒▒██▒▒▒",
@"▒▒█▒▒▒██▒██████▒█▒▒▒▒██▒█▒▒▒▒██▒██████▒█▒▒▒██▒▒",
@"▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒",
                };
                foreach (string line in Name)
                {
                    Console.SetCursorPosition((Console.BufferWidth - line.Length) / 2, (Console.BufferHeight - Name.Length) / 2 + count);
                    Console.WriteLine(line);
                    count++;
                }
                string HuongDan = "Nhấn ENTER để sang trang HƯỚNG DẪN CHƠI GAME!";
                Console.SetCursorPosition(Console.BufferWidth / 2 - 23, 24);
                Console.WriteLine(HuongDan);
                Console.ReadKey();
                Console.Clear();
                string a = "HƯỚNG DẪN CHƠI";
                string b = "RÁC HỮU CƠ: Ấn phím 1_xương cá";
                string xuongca =
                    @"
  ▄▄    ▄   ▄
▄███▄▄▄█▄▄▄█▀
 ▀██   ▀▄  ▀█
                ";
                string c = "RÁC TÁI CHẾ ĐƯỢC: Ấn phím 2_chai nhựa";
                string chainhua =
                    @"
 █▀█
█▀ ▀█        
█   █
█   █
▀▀▀▀▀
                ";
                string d = "RÁC THẢI CÒN LẠI: Ấn phím 3_túi nilon";
                string tuinilon =
                    @"
 █ █   ▐ █
█  █  ▄█ ▐▌
█   ▀▀▀   █
▀█▄▄    ▄█
  ▀▀▀▀▀▀
                ";
                string e = "Né nếu gặp VẬT CẢN";
                string vatcan =
                    @"
████████
████████
████████ 
████████
████████
                ";
                string f = "CÁCH TÍNH ĐIỂM";
                string g = "Phân loại rác đúng: +10 điểm";
                string i = "Phân loại rác sai: -15 điểm";
                string k = "Nếu điểm số của bạn < 0 thì end game";
                string l = "Nếu bạn đụng chướng ngại vật thì end game";
                Console.SetCursorPosition(1, 1);
                Console.WriteLine(a);
                Console.SetCursorPosition(1, 3);
                Console.WriteLine(b);
                Console.SetCursorPosition(3, 4);
                Console.WriteLine(xuongca);
                Console.SetCursorPosition(1, 9);
                Console.WriteLine(c);
                Console.SetCursorPosition(10, 10);
                Console.WriteLine(chainhua);
                Console.SetCursorPosition(3, 16);
                Console.WriteLine(d);
                Console.SetCursorPosition(3, 17);
                Console.WriteLine(tuinilon);
                Console.SetCursorPosition(1, 24);
                Console.WriteLine(e);
                Console.SetCursorPosition(3, 25);
                Console.WriteLine(vatcan);
                Console.SetCursorPosition(Console.BufferWidth / 2 - 10, 1);
                Console.WriteLine(f);
                Console.SetCursorPosition(Console.BufferWidth / 2 - 10, 3);
                Console.WriteLine(g);
                Console.SetCursorPosition(Console.BufferWidth / 2 - 10, 5);
                Console.WriteLine(i);
                Console.SetCursorPosition(Console.BufferWidth / 2 - 10, 7);
                Console.WriteLine(k);
                Console.SetCursorPosition(Console.BufferWidth / 2 - 10, 9);
                Console.WriteLine(l);
                Console.ReadKey();
                Console.Clear();
            }
            static void GameOver()//Màn hình hiện khi thua
            {
                int count = 0;
                string[] gameover = new string[]
                {
                @"▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒",
                @"▒G A M E  O V E R !▒",
                @"▒     ┏━━━━━┓      ▒",
                @"▒     ┃Enter┃      ▒",
                @"▒     ┗━━━━━┛      ▒",
                @"▒     RESTART      ▒",
                @"▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒"
                };
                foreach (string line in gameover)
                {
                    Console.SetCursorPosition((Console.BufferWidth - line.Length) / 2, (Console.BufferHeight - gameover.Length) / 2 + count);
                    Console.WriteLine(line);
                    count++;
                }
            }
            static void InputNameBox()
            {
                int count = 0;
                string[] savebox = new string[]
                {
                @"▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒",
                @"▒      USER ACCOUNT       ▒",
                @"▒┏━━━━━━━━━━━━━━━━━━━━━━━┓▒",
                @"▒┃NAME:                  ┃▒",
                @"▒┗━━━━━━━━━━━━━━━━━━━━━━━┛▒",
                @"▒  Nhấn <Enter> hoàn tất  ▒",
                @"▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒"
                };

                foreach (string line in savebox)
                {
                    Console.SetCursorPosition((Console.BufferWidth - line.Length) / 2, (Console.BufferHeight - savebox.Length) / 2 + count);
                    Console.WriteLine(line);
                    count++;
                }
                Console.SetCursorPosition(53, 14);
                Console.ReadLine();
                string s = Console.ReadLine();
                do
                {
                    s = Console.ReadLine();
                    // Kiểm tra xem chuỗi nhập vào có chỉ toàn khoảng trắng không
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        Console.SetCursorPosition(Console.BufferWidth / 2 - 20, 20);
                        Console.WriteLine("Tên không được để trống. Vui lòng nhập lại!!!");
                    }
                } while (string.IsNullOrWhiteSpace(s));
            }
            static void SaveBox()
            {
                int count = 0;
                string[] savebox = new string[]
                {
                "▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒",
                "▒        SAVE BOX         ▒",
                "▒┏━━━━━━━━━━━━━━━━━━━━━━━┓▒",
                "▒┃ NAME:                 ┃▒",
                "▒┗━━━━━━━━━━━━━━━━━━━━━━━┛▒",
                "▒  Nhấn <Enter> hoàn tất  ▒",
                "▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒"
                };
                foreach (string line in savebox)
                {
                    Console.SetCursorPosition((Console.BufferWidth - line.Length) / 2, (Console.BufferHeight - savebox.Length) / 2 + count);
                    Console.WriteLine(line);
                    count++;
                }

            }//lạ lạ 
            public static int MaximumLengthOfPlayersName { get; private set; }

            static void BangXepHang()
            {
                int count = 0;
                string[] bangxephang = new string[]
                {
                @"┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓",
                @"┃                                      RANK                                          ┃",
                @"┃━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┃",
                @"┃                                                                                    ┃",
                @"┃                                                                                    ┃",
                @"┃                                                                                    ┃",
                @"┃                                                                                    ┃",
                @"┃                                                                                    ┃",
                @"┃                                                                                    ┃",
                @"┃                                                                                    ┃",
                @"┃                                                                                    ┃",
                @"┃                                                                                    ┃",
                @"┃                                                                                    ┃",
                @"┃                                                                                    ┃",
                @"┃                                                                                    ┃",
                @"┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛",
                };
                foreach (string line in bangxephang)
                {
                    Console.SetCursorPosition((Console.BufferWidth - line.Length) / 2, (Console.BufferHeight - bangxephang.Length) / 2 + count);
                    Console.WriteLine(line);
                    count++;
                }
            }

        }






        // Người chơi
        public class User
        {
            public static void Info_User()
            {
                string[] output = new string[]
                {   @" ██▄▄▄▄▄▄▄▄▄▄█▀ ",
                    @" ██          █▄ ",
                    @"▄▀█  ▐▌  █   ██ ",
                    @"▐▄▄▌   ▄▄    █  ",
                    @"  █         █   ",
                    @"   ▀▀█▄▄█▀▀     ",
                };

                Random rnd = new Random();
                int[] lane = { 1, 10, 19 }; // thay doi Y, giu nguyen X
                user.X = 3;
                user.Y = lane[rnd.Next(0, lane.Length)];
                // In đầu ra ban đầu
                Method.Print(ref user, output);
                while (true)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true); // true để không in ra phím  
                    int truonghop = 0;

                    if (keyInfo.KeyChar == 'w')
                        truonghop = 1; // Di chuyển lên  
                    if (keyInfo.KeyChar == 's')
                        truonghop = 2; // Di chuyển xuống
                    if (keyInfo.KeyChar == 'q')
                        truonghop = 3;

                    Method.Clear(ref user, output); // Xóa đầu ra cũ 
                                                    // Duy chuyển và in lại  
                    if (truonghop == 1 && user.Y > 2)  // Nếu không vượt quá giới hạn trên  
                    {
                        user.Y -= 9;

                    }
                    if (truonghop == 2 && user.Y < 18) // Nếu không vượt quá giới hạn dưới  
                    {
                        user.Y += 9;

                    }
                    if (truonghop == 3)
                    {
                        break;

                    }
                    //Point headuser = new Point(user.X + 5, user.Y);
                    //isHit(headuser);
                    //countScore(headuser);
                    //// In lại đầu ra mới  
                    Method.Print(ref user, output);
                    //if (Method.Phan_Loai_Rac() == 1)
                    //    Method.Clear(ref fish, output);


                }


            }
        }
        //Các loại rác
        //Các loại rác
        public class Trash
        {

            //Rác cá
            public static void Fish()
            {

                string[] output = new string[]
                        {
@"     ▄▄   ▄  ▄ ",
@"   ▄███▄▄█▄▄█▀ ",
@"    ▀██  ▀▄ ▀█ ",
                        };
                Random rnd = new Random();
                int[] lane = { 2, 11, 19 }; // thay doi Y, giu nguyen X
                fish.X = width * 2 - output[0].Length;
                fish.Y = lane[rnd.Next(0, lane.Length)]; ;
                Method.Move(ref fish, output);

            }
            public static void Plastic_bag()
            {
                string[] output = new string[]
                        {
                                @"  █ █   ▐ █  ",
                                @"  █  █ ▄█ ▐▌ ",
                                @"  █  ▀▀▀  █ ",
                                @"  ▀█▄▄  ▄█  ",

                        };
                Random rnd = new Random();
                int[] lane = { 2, 11, 19 }; // thay doi Y, giu nguyen X
                plastic_bag.X = width * 2 - output[0].Length;
                plastic_bag.Y = lane[rnd.Next(0, lane.Length)]; ;
                Method.Move(ref plastic_bag, output);
            }
            public static void Glass_Bottle()
            {
                string[] output = new string[]
                            {
            @"   █▀█  ",
            @"  █▀ ▀█ ",
            @"  █   █ ",
            @"  █   █ ",
            @"  ▀▀▀▀▀ ",
                            };
                Random rnd = new Random();
                int[] lane = { 2, 11, 19 }; // thay doi Y, giu nguyen X
                glass_bottle.X = width * 2 - output[0].Length;
                glass_bottle.Y = lane[rnd.Next(0, lane.Length)]; ;
                Method.Move(ref glass_bottle, output);
            }
        }

        // Các hàm xử lý
        public class Method
        {
            //In ra màn hình
            public static void Print(ref Point point, string[] output)
            {
                lock (consoleLock)
                {
                    for (int i = 0; i < output.Length; i++)
                    {
                        Console.SetCursorPosition(point.X, point.Y + i);// Thiết lập vị trí con trỏ  
                        Console.WriteLine(output[i]);
                    }
                }
            }
            // Xóa những hình cũ
            public static void Clear(ref Point point, string[] output)
            {
                lock (consoleLock)
                {
                    for (int i = 0; i < output.Length; i++)
                    {
                        for (int k = 0; k < output[i].Length; k++)
                        {
                            Console.SetCursorPosition(point.X + k, point.Y + i);// Thiết lập vị trí con trỏ  
                            Console.WriteLine(' ');

                        }
                    }
                }

            }
            // Di chuyển
            public static void Move(ref Point point, string[] print)
            {
                Random rnd = new Random();
                int[] lane = { 2, 11, 19 }; // thay doi Y, giu nguyen X
                int xspawn = point.X;
                while (point.X > 0)
                {
                    lock (consoleLock)
                    {
                        Method.Clear(ref point, print); // Xóa vật thể 
                        Method.Print(ref point, print);  // In lại vật thể ở vị trí mới
                        point.X--;
                        trash.Add(point);
                        Thread.Sleep(10); // Tạm dừng để điều chỉnh tốc độ di chuyển

                        if (point.X == 1)
                        {
                            Method.Clear(ref point, print);
                            point.X = xspawn;
                            point.Y = lane[rnd.Next(0, lane.Length)];
                            Method.Print(ref point, print);  // Tạo lại hình ảnh block
                            Method.Clear(ref point, print);

                        }

                    }

                }

            }
            //Check va chạm với người dùng với rác và chướng ngọai vật 
            public static bool hitBox(Point head_user, Point input)
            {
                head_user.X = user.X + 5;
                if (head_user.X == input.X)
                    return true;
                else return false;

            }
            //check để cho không in trùng tọa đọ cnv và rác 
            public static bool isEqual(Point fish, Point plastic_bag, Point glass_bottle, Point block)
            {
                if (
                   fish.X == plastic_bag.X && fish.Y == plastic_bag.Y ||
                   fish.X == glass_bottle.X && fish.Y == glass_bottle.Y ||
                   fish.X == block.X && fish.Y == block.Y ||
                   plastic_bag.X == glass_bottle.X && plastic_bag.Y == glass_bottle.Y ||
                   plastic_bag.X == block.X && plastic_bag.Y == block.Y ||
                   glass_bottle.X == block.X && glass_bottle.Y == block.Y
                 )
                    return false; // false là trùng
                else return true; // true không trùng
            }
            public static int Phan_Loai_Rac()
            {
                int truonghop = 0;
                ConsoleKeyInfo keyInfo = Console.ReadKey(true); // true để không in ra phím
                if (keyInfo.KeyChar == '1')
                    truonghop = 1;
                if (keyInfo.KeyChar == '2')
                    truonghop = 2;
                if (keyInfo.KeyChar == '3')
                    truonghop = 3;
                return truonghop;

            }
            public static void Nguoi_Cham_Chuong_Ngoai_Vat()
            {
                //if (isEqual(head_user, block))
                Console.WriteLine("Game Over");

            }
        }
        public class Point
        {
            private int x;
            private int y;
            public int X { get => x; set => x = value; }
            public int Y { get => y; set => y = value; }

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public bool IsEqual(Point other)
            {
                return this.X == other.X && this.Y == other.Y;
            }
        }
        public class Block
        {
            public static void BlockImage()
            {

                string[] BlockImage =
                {   @" ████████ ",
                    @" ████████ ",
                    @" ████████ ",
                    @" ████████ ",
                    @" ████████ ",
                    @" ████████ "
                };
                Random rnd = new Random();
                int[] lane = { 2, 11, 19 }; // thay doi Y, giu nguyen X
                block.X = width * 2 - BlockImage[0].Length;
                block.Y = lane[rnd.Next(0, lane.Length)];
                Method.Move(ref block, BlockImage);
            }
        }
    }
}




