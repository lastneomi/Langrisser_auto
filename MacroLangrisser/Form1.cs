using MacroLangrisser.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

//참고 사이트. 감사합니다.
//https://hl4rny.tistory.com/243
//https://tctt.tistory.com/163
//https://itfresh.tistory.com/15
//https://lesomnus.tistory.com/10#footnote_10_1

namespace MacroLangrisser
{

    public partial class Form1 : Form
    {
        // 핸들을 잡기위한 Dll Import
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);

        //화면 조작 Dll Import
        [DllImport("user32.dll")]
        public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        //키 조작 Dll Import
        [DllImport("User32.Dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        //캡쳐 Dll Import
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);
        [DllImport("user32.dll")]
        static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        //전역 변수
        int runStopFlag = 0;
        string blueStackClassName = "BlueStacks";
        string childBlueStackName = "BlueStacks Android PluginAndroid";
        int title_Y = 42; //제목표시줄 높이 42

        public static string Image_file_origin = Application.StartupPath + @"\Capt_origin.bmp";
        public static string Image_file_cut = Application.StartupPath + @"\Capt_cut.bmp";
        public static string Image_file_con = Application.StartupPath + @"\Capt_con.bmp";
        public static Bitmap Image_temp;

        // PostMessage 를 위한 Message Value
        public enum WMessages : int
        {
            WM_LBUTTONDOWN = 0x201, //Left mousebutton down
            WM_LBUTTONUP = 0x202,  //Left mousebutton up
            WM_LBUTTONDBLCLK = 0x203, //Left mousebutton doubleclick
            WM_RBUTTONDOWN = 0x204, //Right mousebutton down
            WM_RBUTTONUP = 0x205,   //Right mousebutton up
            WM_RBUTTONDBLCLK = 0x206, //Right mousebutton doubleclick
            WM_KEYDOWN = 0x100,  //Key down
            WM_KEYUP = 0x101,   //Key up
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105,
            WM_CHAR = 0x102,
            WM_COMMAND = 0x111
        }

        // 이미지 정보 형식 셋팅
        class statusList
        {
            public Bitmap imgName;
            public int returnCount;
            public string Des;
        }

        class Library
        {
            // 이미지 비교
            public static int imgComp(Bitmap A, statusList b)
            {
                    using (Bitmap B = new Bitmap(b.imgName))
                    {
                        for (int i = 0; i < A.Width; i++)
                        {
                            for (int j = 0; j < B.Height; j++)
                            {
                                if (A.GetPixel(i, j).ToString() != B.GetPixel(i, j).ToString())
                                {
                                    return 0;
                                }
                            }
                        }
                    }
                return 1;
            }

            // 이미지 리스트
            public static statusList[] imgList = new statusList[]
            {
            new statusList { imgName = Resources.case_1, returnCount = 0, Des = "사건" },
            new statusList { imgName = Resources.posion, returnCount = 1, Des = "경험치포션" },
            new statusList { imgName = Resources.hammer, returnCount = 2, Des = "망치" },
            new statusList { imgName = Resources.money, returnCount = 3, Des = "돈" },
            new statusList { imgName = Resources.key, returnCount = 4, Des = "열쇠" },
            new statusList { imgName = Resources.gear, returnCount = 5, Des = "장비" },
            new statusList { imgName = Resources.move, returnCount = 6, Des = "돌입" },
            new statusList { imgName = Resources.attack, returnCount = 7, Des = "공격" },
            new statusList { imgName = Resources.auto, returnCount = 8, Des = "자동" },
            new statusList { imgName = Resources.talk, returnCount = 9, Des = "대화_스킵" },
            new statusList { imgName = Resources.stamina, returnCount = 10, Des = "스태미너 부족" },
            new statusList { imgName = Resources.end, returnCount = 11, Des = "정산" },
            new statusList { imgName = Resources.treasure, returnCount = 12, Des = "정산_보물상자" },
            new statusList { imgName = Resources.open_box, returnCount = 13, Des = "정산_열린상자" },
            new statusList { imgName = Resources.caselist, returnCount = 14, Des = "사건_리스트" },
            new statusList { imgName = Resources._2p, returnCount = 15, Des = "2P 입장" },
            new statusList { imgName = Resources._3p, returnCount = 16, Des = "3P 입장" },
            new statusList { imgName = Resources.fight, returnCount = 17, Des = "전투 시작" },
            new statusList { imgName = Resources.confirm, returnCount = 18, Des = "다시 초대 확인" },
            new statusList { imgName = Resources.point, returnCount = 19, Des = "포인트 정산" },
            new statusList { imgName = Resources.talk_skip, returnCount = 20, Des = "대화_스킵" },
            };
        }

        //delay 함수.
        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }
        
        // X_Y 좌표 생성
        public int MakeLParam(int LoWord, int HiWord)
        {
            return (int)((HiWord << 16) | (LoWord & 0xFFFF));
        }

        public void clickSend(int x, int y)
        {
            IntPtr hw1 = FindWindow(null, blueStackClassName);
            IntPtr hw2 = FindWindowEx(hw1, IntPtr.Zero, null, childBlueStackName);

            PostMessage(hw2, (int)WMessages.WM_LBUTTONDOWN, 1, MakeLParam(x, y - title_Y ));
            Delay(100);
            PostMessage(hw2, (int)WMessages.WM_LBUTTONUP, 0, MakeLParam(x, y - title_Y));
            Delay(500);
        }

        //이미지 저장 함수
        public void Capt_file(int x, int y, int x_Width, int y_Height)
        {
            try
            {
                IntPtr hwnd = FindWindow(null, blueStackClassName);

                SetWindowPos((int)hwnd, 0, 0, 0, 1570, 920, 0x10);
                Graphics gfxWin = Graphics.FromHwnd(hwnd);
                Rectangle rc = Rectangle.Round(gfxWin.VisibleClipBounds);

                Bitmap bmp = new Bitmap(
                    rc.Width, rc.Height,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);


                Graphics gfxBmp = Graphics.FromImage(bmp);
                IntPtr hdcBitmap = gfxBmp.GetHdc();
                bool succeeded = PrintWindow(hwnd, hdcBitmap, 1);
                gfxBmp.ReleaseHdc(hdcBitmap);
                if (!succeeded)
                {
                    gfxBmp.FillRectangle(
                        new SolidBrush(Color.Gray),
                        new Rectangle(Point.Empty, bmp.Size));
                }

                gfxBmp.Dispose();
                bmp.Save(Image_file_origin);

                var bitmap2 = bmp.Clone(new Rectangle(x, y, x_Width, y_Height), PixelFormat.Format32bppArgb);
                var bitmap3 = bmp.Clone(new Rectangle(x, y, x_Width, y_Height), PixelFormat.Format4bppIndexed);

                bitmap2.Save(Image_file_cut);
                bitmap2.Dispose();
                bitmap2 = null;
                bmp.Dispose();
                bmp = null;

                bitmap3.Save(Image_file_con);
                bitmap3.Dispose();
                bitmap3 = null;

                textBox1.Text += "이미지 파일 저장 완료. \r\n";
            }
            catch (Exception e)
            {
                textBox1.Text += "[Error]이미지 저장 실패 \r\n";
            }
        }

        //비교용 이미지 캡쳐 함수
        public int Capt()
        {
            try
            {
                Clipboard.Clear();
                IntPtr hwnd = FindWindow(null, blueStackClassName);

                SetWindowPos((int)hwnd, 0, 0, 0, 1570, 920, 0x10);
                Graphics gfxWin = Graphics.FromHwnd(hwnd);
                Rectangle rc = Rectangle.Round(gfxWin.VisibleClipBounds);

                Image_temp = new Bitmap(
                    rc.Width, rc.Height,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Graphics gfxBmp = Graphics.FromImage(Image_temp);
                IntPtr hdcBitmap = gfxBmp.GetHdc();
                bool succeeded = PrintWindow(hwnd, hdcBitmap, 1);
                gfxBmp.ReleaseHdc(hdcBitmap);
                if (!succeeded)
                {
                    gfxBmp.FillRectangle(
                        new SolidBrush(Color.Gray),
                        new Rectangle(Point.Empty, Image_temp.Size));
                    textBox1.Text += "[Error]캡쳐 \r\n";
                    gfxBmp.Dispose();
                    return 0;
                }
                gfxBmp.Dispose();
                return 1;
            }
            catch (Exception e)
            {
                textBox1.Text += "[Error]캡쳐 \r\n";
                return 0;
            }
        }

        //이미지 잘라내기 함수
        public Bitmap cutImg(int x, int y, int x_size, int y_size)
        {
            try
            {
                Bitmap bmp = Image_temp.Clone(new Rectangle(x, y, x_size, y_size), PixelFormat.Format4bppIndexed);
                return bmp;
            }
            catch (Exception e)
            {
                textBox1.Text += "[Error]캡쳐 \r\n";
                return null;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Diagnostics.Process[] mProcess = System.Diagnostics.Process.GetProcessesByName(Application.ProductName);
            foreach (System.Diagnostics.Process p in mProcess)
                p.Kill();
        }

        //테스트
        private void btnCapImg_Click(object sender, EventArgs e)
        {
            //사건 열기
            //Capt_file(1485, 175, 35, 55);

            //처음 hammer //1026 184
            //Capt_file(1035, 190, 75, 70);

            //둘 key
            //Capt_file(1035, 338, 75, 70);

            //셋 money
            //Capt_file(1035, 486, 75, 70);

            //넷 gear
            //Capt_file(1035, 634, 75, 70);

            //사건 돌입
            //Capt_file(890, 650, 75, 45);

            //공격
            // Capt_file(1415, 830, 95, 50);

            //자동
            //Capt_file(1475, 340, 45, 25);

            //대화 팝업시 클릭(오른쪽)
            // Capt_file(910, 695, 70, 25);

            //대화 팝업시 클릭(왼쪽)
            //Capt_file(360, 700, 40, 15);

            //사건 정산
            //Capt_file(785, 235, 95, 50);

            //보물상자
            //Capt_file(710, 410, 100, 100);

            //열린 보물상자
            //Capt_file(685, 635, 70, 70);

            //사건 글씨
            //Capt_file(1135, 100, 40, 40);

            //2p
            //Capt_file(605, 360, 50, 40);

            //3p
            //Capt_file(1020, 360, 50, 40);

            //전투 시작
            //Capt_file(1200, 755, 55, 35);

            //다시 초대
            //Capt_file(875, 580, 50, 30);

            //스태미너 부족 (정지)
            // Capt_file(660, 295, 70, 50);

            //대화 자동(왼)
            //Capt_file(1145, 725, 50, 50);

            //스테미너 부족
            //Capt_file(850,800, 500,70);

            //자동 버튼
            //Capt_file(1475, 340, 45, 25);

            //대화 스킵
            //Capt_file(1150, 730, 60, 50);

            //포인트 정산
            // Capt_file(670, 160, 100, 40);

            //대사 스킵
            Capt_file(1100, 825, 40, 40);

        }

        private void btnCase_Click(object sender, EventArgs e)
        {
            Bitmap cutImgfile;
            runStopFlag = 1;
            btnCapImg.Enabled = false; btnCase.Enabled = false; btnSpecial.Enabled = false;

            //사건 시작
            while (runStopFlag == 1)
            {
                //블루스택 실행 여부 확인
                IntPtr Id = FindWindow(null, "BlueStacks");
                if ((int)Id == 0)
                {
                    textBox1.Text += "블루스택을 실행해주세요.\r\n";
                    break;
                }

                if(Capt() == 0)
                    break;
                cutImgfile = cutImg(1485, 175, 35, 55);

                //사건 열기
                var item = Library.imgList[0];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(1485, 175);

                    textBox1.Text += "사건 시작"
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }


                //사건 리스트 확인
                cutImgfile = cutImg(1135, 100, 40, 40);
                item = Library.imgList[14];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    textBox1.Text += "사건 리스트 오픈\r\n";

                    #region 경험치 포션 제외하고 동작

                    item = Library.imgList[1]; //경험치 포션 이미지

                    //첫번째 칸
                    if (Library.imgComp( cutImg(1035, 190, 75, 70) , item) != 1)
                    {
                        clickSend(1045, 200);

                        textBox1.Text += "1째 리스트 클릭"
                                      + DateTime.Now.ToString("HHmmss")
                                      + "\r\n";
                    }
                    //두번째 칸
                    else if (Library.imgComp(cutImg(1035, 338, 75, 70), item) != 1)
                    {
                        clickSend(1045, 348);

                        textBox1.Text += "2째 리스트 클릭"
                                      + DateTime.Now.ToString("HHmmss")
                                      + "\r\n";
                    }

                    //세번째 칸
                    else if (Library.imgComp(cutImg(1035, 486, 75, 70), item) != 1)
                    {
                        clickSend(1045, 496);

                        textBox1.Text += "3째 리스트 클릭"
                                      + DateTime.Now.ToString("HHmmss")
                                      + "\r\n";
                    }

                    //네번째 칸
                    else if (Library.imgComp(cutImg(1035, 634, 75, 70), item) != 1)
                    {
                        clickSend(1045, 644);

                        textBox1.Text += "4째 리스트 클릭"
                                      + DateTime.Now.ToString("HHmmss")
                                      + "\r\n";
                    }
                    //다섯째 칸
                    else if (Library.imgComp(cutImg(1035, 782, 75, 70), item) != 1)
                    {
                        clickSend(1045, 792);

                        textBox1.Text += "5째 리스트 클릭"
                                      + DateTime.Now.ToString("HHmmss")
                                      + "\r\n";
                    }
                    #endregion

                }


                //사건 돌입
                cutImgfile = cutImg(890, 650, 75, 45);
                item = Library.imgList[6];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(930, 670);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //공격
                cutImgfile = cutImg(1415, 830, 95, 50);
                item = Library.imgList[7];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(1465, 815);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //자동 버튼
                cutImgfile = cutImg(1475, 340, 45, 25);
                item = Library.imgList[8];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(1500, 350);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //대화 스킵
                cutImgfile = cutImg(1150, 730, 60, 50);
                item = Library.imgList[9];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(730, 800);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //대화 스킵2
                cutImgfile = cutImg(1000, 825, 40, 40);
                item = Library.imgList[20];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(730, 800);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //사건 정산
                cutImgfile = cutImg(785, 235, 95, 50);
                item = Library.imgList[11];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(785, 235);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //정산 보물상자
                cutImgfile = cutImg(710, 410, 100, 100);
                item = Library.imgList[12];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(710, 410);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //정산 열린 상자
                cutImgfile = cutImg(685, 635, 70, 70);
                item = Library.imgList[13];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(685, 635);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                Delay(2000);
            }

        }

        private void btnSpecial_Click(object sender, EventArgs e)
        {
            Bitmap cutImgfile;
            runStopFlag = 1;
            btnCapImg.Enabled = false; btnCase.Enabled = false; btnSpecial.Enabled = false;

            while (runStopFlag == 1)
            {
                //블루스택 실행 여부 확인
                IntPtr Id = FindWindow(null, "BlueStacks");
                if ((int)Id == 0)
                {
                    textBox1.Text += "블루스택을 실행해주세요.\r\n";
                    break;
                }

                if (Capt() == 0)
                    break;
                //2p 입장
                cutImgfile = cutImg(605, 360, 50, 40);
                var item = Library.imgList[15];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    //3p 입장
                    cutImgfile = cutImg(1020, 360, 50, 40);
                    item = Library.imgList[16];
                    if (Library.imgComp(cutImgfile, item) == 1)
                    {
                        clickSend(1280, 770);

                        textBox1.Text += "비경 시작"
                                      + DateTime.Now.ToString("HHmmss")
                                      + "\r\n";
                    }
                }

                //공격
                cutImgfile = cutImg(1415, 830, 95, 50);
                item = Library.imgList[7];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(1465, 815);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }


                //자동 버튼
                cutImgfile = cutImg(1475, 340, 45, 25);
                item = Library.imgList[8];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(1500, 350);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //대화 스킵
                cutImgfile = cutImg(1150, 730, 60, 50);
                item = Library.imgList[9];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(730, 800);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //대화 스킵2
                cutImgfile = cutImg(1100, 825, 40, 40);
                item = Library.imgList[20];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(730, 800);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //사건 정산
                cutImgfile = cutImg(785, 235, 95, 50);
                item = Library.imgList[11];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(785, 235);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //포인트 정산
                cutImgfile = cutImg(670, 160, 100, 40);
                item = Library.imgList[19];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(1400, 600);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //정산 보물상자
                cutImgfile = cutImg(710, 410, 100, 100);
                item = Library.imgList[12];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(710, 410);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //정산 열린 상자
                cutImgfile = cutImg(685, 635, 70, 70);
                item = Library.imgList[13];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(685, 635);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //다시 초대 확인
                cutImgfile = cutImg(875, 580, 50, 30);
                item = Library.imgList[18];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    clickSend(905, 595);

                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";
                }

                //스태미나 부족
                cutImgfile = cutImg(850, 800, 500, 70);
                item = Library.imgList[10];
                if (Library.imgComp(cutImgfile, item) == 1)
                {
                    textBox1.Text += item.Des
                                  + DateTime.Now.ToString("HHmmss")
                                  + "\r\n";

                    SystemSounds.Beep.Play();
                    Delay(1000);
                    SystemSounds.Beep.Play();
                    Delay(1000);
                    SystemSounds.Beep.Play();
                    Delay(1000);
                    break;
                }
                
                Delay(2000);
            }

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            runStopFlag = 0;
            textBox1.Text += "정지\r\n";

            btnCapImg.Enabled = true;            btnCase.Enabled = true;            btnSpecial.Enabled = true;
        }

    }
}
