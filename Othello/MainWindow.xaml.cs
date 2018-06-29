using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Othello
{

    public partial class MainWindow : Window
    {
        public static Image[,] cellList = new Image[9,9];
        static BitmapImage blackSource = new BitmapImage(new Uri("pack://Application:,,,/Resources/black.png"));
        static BitmapImage whiteSource = new BitmapImage(new Uri("pack://Application:,,,/Resources/white.png"));
        static BitmapImage tipSource = new BitmapImage(new Uri("pack://Application:,,,/Resources/tip.png"));
        static int[,] board = new int[9, 9];
        static int color = 1;
        static int black=0,white=0,able = 0;
        static int win1 = 0, win2 = 0;
        static int blackstep = 0, whitestep = 0;
        static int PVE = 0;
        static int EVE = 0;
        const int inf = 0x3f3f3f3f;
        DispatcherTimer timer = new DispatcherTimer();
        
        int[,] value = new int[,] //估值表
        {
           {  0,  0,  0,  0,  0,  0,  0,  0,  0},
           {  0,120,-60, 10, 10, 10, 10,-60,120},
           {  0,-60,-80,  5,  5,  5,  5,-80,-60},
           {  0, 10,  5,  1,  1,  1,  1,  5, 10},
           {  0, 10,  5,  1,  1,  1,  1,  5, 10},
           {  0, 10,  5,  1,  1,  1,  1,  5, 10},
           {  0, 10,  5,  1,  1,  1,  1,  5, 10},
           {  0,-60,-80,  5,  5,  5,  5,-80,-60},
           {  0,120,-60, 10, 10, 10, 10,-60,120},
        };
        
        public MainWindow()
        {
            InitializeComponent();
            bkg.Source = new Uri("pack://SiteOfOrigin:,,,/bkgmov.avi",UriKind.RelativeOrAbsolute);
            bkg.Play();
            cellList[1, 1] = cell11; cellList[1, 2] = cell12; cellList[1, 3] = cell13; cellList[1, 4] = cell14; cellList[1, 5] = cell15; cellList[1, 6] = cell16; cellList[1, 7] = cell17; cellList[1, 8] = cell18;
            cellList[2, 1] = cell21; cellList[2, 2] = cell22; cellList[2, 3] = cell23; cellList[2, 4] = cell24; cellList[2, 5] = cell25; cellList[2, 6] = cell26; cellList[2, 7] = cell27; cellList[2, 8] = cell28;
            cellList[3, 1] = cell31; cellList[3, 2] = cell32; cellList[3, 3] = cell33; cellList[3, 4] = cell34; cellList[3, 5] = cell35; cellList[3, 6] = cell36; cellList[3, 7] = cell37; cellList[3, 8] = cell38;
            cellList[4, 1] = cell41; cellList[4, 2] = cell42; cellList[4, 3] = cell43; cellList[4, 4] = cell44; cellList[4, 5] = cell45; cellList[4, 6] = cell46; cellList[4, 7] = cell47; cellList[4, 8] = cell48;
            cellList[5, 1] = cell51; cellList[5, 2] = cell52; cellList[5, 3] = cell53; cellList[5, 4] = cell54; cellList[5, 5] = cell55; cellList[5, 6] = cell56; cellList[5, 7] = cell57; cellList[5, 8] = cell58;
            cellList[6, 1] = cell61; cellList[6, 2] = cell62; cellList[6, 3] = cell63; cellList[6, 4] = cell64; cellList[6, 5] = cell65; cellList[6, 6] = cell66; cellList[6, 7] = cell67; cellList[6, 8] = cell68;
            cellList[7, 1] = cell71; cellList[7, 2] = cell72; cellList[7, 3] = cell73; cellList[7, 4] = cell74; cellList[7, 5] = cell75; cellList[7, 6] = cell76; cellList[7, 7] = cell77; cellList[7, 8] = cell78;
            cellList[8, 1] = cell81; cellList[8, 2] = cell82; cellList[8, 3] = cell83; cellList[8, 4] = cell84; cellList[8, 5] = cell85; cellList[8, 6] = cell86; cellList[8, 7] = cell87; cellList[8, 8] = cell88;
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                    board[i, j] = 0;
        }
         int max(int a,int b)
        {
            return a > b ? a : b;
        }
         int min(int a, int b)
        {
            return a < b ? a : b;
        }
        int evaluate(int[,] board, int color, int x, int y)
        {

            int total = 0, actionPower = 0, space = 0;
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                {
                    if (board[i, j] == color)
                        total += value[x, y];
                    else if (board[i, j] == 2)
                        actionPower++;
                    else
                        space++;
                }
            return total + 3*(space/16) * actionPower;
        }
         void GradientInTimer(object sender,EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            if (grid.Effect is BlurEffect)
            {
                BlurEffect blur = (BlurEffect)grid.Effect;
                blur.Radius += 1;
                if (blur.Radius >= 10)
                {
                    timer.Stop();
                }
            }
            else timer.Stop();
        }
         void GradientOutTimer(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            if (grid.Effect is BlurEffect)
            {
                BlurEffect blur = (BlurEffect)grid.Effect;
                blur.Radius -= 1;
                if (blur.Radius <= 0)
                {
                    timer.Stop();
                    DropShadowEffect effect = new DropShadowEffect();
                    effect.ShadowDepth = 3;
                    effect.RenderingBias = RenderingBias.Quality;
                    grid.Effect = effect;
                }
            }
            else timer.Stop();
        }
         void printScreen()
        {
            for(int i=1;i<=8;i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    cellList[i, j].Cursor = Cursors.No;
                    cellList[i, j].Source = new BitmapImage(new Uri("pack://Application:,,,/Resources/init.png"));
                    if (board[i,j] == 1)
                        cellList[i, j].Source = blackSource;
                    else if (board[i,j] == 2)
                    {
                        cellList[i, j].Source = tipSource;
                        cellList[i, j].Cursor = Cursors.Hand;
                    }
                    else if (board[i,j] == -1)
                        cellList[i, j].Source = whiteSource;
                }
            }
            num1.Content = black + "";
            num2.Content = white + "";
            winCnt1.Content = win1 + "";
            winCnt2.Content = win2 + "";
            stepnum1.Content = blackstep + "";
            stepnum2.Content = whitestep + "";
            if(EVE!=0)tip.Content = (color == 1 ? "黑方" : "白方") + "电脑正在思考...";
            else if (PVE >= 1) tip.Content = "轮到你下了！";
            else tip.Content = "现在轮到" + (color == 1 ? " 黑方 " : " 白方 ") + "下棋";
        }
        void rev(int[,]board,int x, int y)
        {
            for (int i = x - 1; i > 0; i--)//up
            {
                if (board[i,y] == board[x,y])
                {
                     
                        for (int j = i+1; j < x; j++)
                            board[j,y] = board[x,y];
                    break;
                }
                if (board[i,y] != ~board[x,y] + 1) break;
            }
             
            for (int i = x + 1; i <= 8; i++)//down
            {
                if (board[i,y] == board[x,y])
                {
                     
                        for (int j = i-1; j > x; j--)
                            board[j,y] = board[x,y];
                    break;
                }
                if (board[i,y] != ~board[x,y] + 1) break;
            }
             
            for (int i = y - 1; i > 0; i--)//left
            {
                if (board[x,i] == board[x,y])
                {
                     
                        for (int j = i+1; j < y; j++)
                            board[x,j] = board[x,y];
                    break;
                }
                if (board[x,i] != ~board[x,y] + 1) break;
            }
             
            for (int i = y + 1; i <= 8; i++)//right
            {
                if (board[x,i] == board[x,y])
                {
                     
                        for (int j = i-1; j > y; j--)
                            board[x,j] = board[x,y];
                    break;
                }
                if (board[x,i] != ~board[x,y] + 1) break;
            }
             
            for (int k = 1; x - k > 0 && y - k > 0; k++)//from x-y to upleft;
            {
                if (board[x,y] == board[x - k,y - k])
                {
                     
                        for (int j = 1; j <= k; j++)
                            board[x - j,y - j] = board[x,y];
                    break;
                }
                if (board[x - k,y - k] != ~board[x,y] + 1) break;
            }
             
            for (int k = 1; x - k > 0 && y + k <= 8; k++)//from x-y to upright;
            {
                if (board[x,y] == board[x - k,y + k])
                {
                     
                        for (int j = 1; j <= k; j++)
                            board[x - j,y + j] = board[x,y];
                    break;
                }
                if (board[x - k,y + k] != ~board[x,y] + 1) break;
            }
             
            for (int k = 1; x + k <= 8 && y - k > 0; k++)//from x-y to downleft;
            {
                if (board[x,y] == board[x + k,y - k])
                {
                     
                        for (int j = 1; j <= k; j++)
                            board[x + j,y - j] = board[x,y];
                    break;
                }
                if (board[x + k,y - k] != ~board[x,y] + 1) break;
            }
             
            for (int k = 1; x + k <= 8 && y + k <= 8; k++)//from x-y to downright;
            {
                if (board[x,y] == board[x + k,y + k])
                {
                     
                        for (int j = 1; j <= k; j++)
                            board[x + j,y + j] = board[x,y];
                    break;
                }
                if (board[x + k,y + k] != ~board[x,y] + 1) break;
            }
            printScreen();
        }
        int tryRev(int x, int y)
        {
            int cnt = 0;
            for (int i = x - 1; i > 0; i--)//up
            {
                if (board[i,y] == board[x,y])
                {
                    cnt += x - i - 1;
                    break;
                }
                if (board[i,y] != ~board[x,y] + 1) break;
            }
            for (int i = x + 1; i <= 8; i++)//down
            {
                if (board[i,y] == board[x,y])
                {
                    cnt += i - x - 1;
                    break;
                }
                if (board[i,y] != ~board[x,y] + 1) break;
            }
            for (int i = y - 1; i > 0; i--)//left
            {
                if (board[x,i] == board[x,y])
                {
                    cnt += y - i - 1;
                    break;
                }
                if (board[x,i] != ~board[x,y] + 1) break ;
            }
            for (int i = y + 1; i <= 8; i++)//right
            {
                if (board[x,i] == board[x,y])
                {
                    cnt += i - y - 1;
                    break;
                }
                if (board[x,i] != ~board[x,y] + 1) break;
            }
            for (int k = 1; x - k > 0 && y - k > 0; k++)//from x-y to upleft;
            {
                if (board[x,y] == board[x - k,y - k])
                {
                    cnt += k - 1;
                    break;
                }
                if (board[x - k,y - k] != ~board[x,y] + 1) break; ;
            }
            for (int k = 1; x - k > 0 && y + k <= 8; k++)//from x-y to upright;
            {
                if (board[x,y] == board[x - k,y + k])
                {
                    cnt += k - 1;
                    break;
                }
                if (board[x - k,y + k] != ~board[x,y] + 1) break;
            }
            for (int k = 1; x + k <= 8 && y - k > 0; k++)//from x-y to downleft;
            {
                if (board[x,y] == board[x + k,y - k])
                {
                    cnt += k - 1;
                    break;
                }
                if (board[x + k,y - k] != ~board[x,y] + 1) break;
            }
            for (int k = 1; x + k <= 8 && y + k <= 8; k++)//from x-y to downright;
            {
                if (board[x,y] == board[x + k,y + k])
                {
                    cnt += k - 1;
                    break;
                }
                if (board[x + k,y + k] != ~board[x,y] + 1) break;
            }
            printScreen();
            return cnt;
        }
        void probRes(int [,]board,int color)
        {
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                    if (Math.Abs(board[i,j]) != 1) board[i,j] = 0;
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    if (board[i,j] == color)
                    {
                        int other = -color;
                        for (int k = 1; k < i - 1; k++)//up
                        {
                            int l = k + 1;
                            if (board[k,j]!=1&&board[k,j]!=-1)
                                while (board[l,j] == other) l++;
                            if (l == i)
                                board[k,j] = 2;
                        }
                        for (int k = i + 2; k <= 8; k++)//down
                        {
                            int l = k - 1;
                            if (board[k, j] != 1 && board[k, j] != -1)
                                while (board[l,j] == other) l--;
                            if (l == i) board[k,j] = 2;
                        }
                        for (int k = 1; k < j - 1; k++)//left
                        {
                            int l = k + 1;
                            if (board[i, k] != 1 && board[i, k] != -1)
                                while (board[i,l] == other) l++;
                            if (l == j) board[i,k] = 2;
                        }
                        for (int k = j + 2; k <= 8; k++)//right
                        {
                            int l = k - 1;
                            if (board[i, k] != 1 && board[i, k] != -1)
                                while (board[i,l] == other) l--;
                            if (l == j) board[i,k] = 2;
                        }
                        for (int k = 2; i - k > 0 && j - k > 0; k++)//upleft;
                        {
                            int l = k - 1;
                            if (board[i-k, j-k] != 1 && board[i-k, j-k] != -1)
                                while (board[i - l,j - l] == other) l--;
                            if (l == 0) board[i - k,j - k] = 2;
                        }
                        for (int k = 2; i - k > 0 && j + k <= 8; k++)//upright;
                        {
                            int l = k - 1;
                            if (board[i - k, j + k] != 1 && board[i - k, j + k] != -1)
                                while (board[i - l,j + l] == other) l--;
                            if (l == 0) board[i - k,j + k] = 2;
                        }
                        for (int k = 2; i + k <= 8 && j - k > 0; k++)//downleft;
                        {
                            int l = k - 1;
                            if (board[i + k, j - k] != 1 && board[i + k, j - k] != -1)
                                while (board[i + l,j - l] == other) l--;
                            if (l == 0) board[i + k,j - k] = 2;
                        }
                        for (int k = 2; i + k <= 8 && j + k <= 8; k++)//downright;
                        {
                            int l = k - 1;
                            if (board[i + k, j + k] != 1 && board[i + k, j + k] != -1)
                                while (board[i + l,j + l] == other) l--;
                            if (l == 0) board[i + k,j + k] = 2;
                        }
                    }
                }
            }
        }
        void statistic()
        {
            black = white = able = 0;
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                    if (board[i,j] == 1) black++;
                    else if (board[i,j] == -1) white++;
                    else if (board[i,j] == 2) able++;
        }

        private void newGameButton_AIMode_Easy_Click(object sender, RoutedEventArgs e)
        {
            PVE = 1;
            EVE = 0;
            oppositeLabel1.Content = "自己";
            oppositeLabel2.Content = "简单电脑";
            init();
        }
        private void newGameButton_AIMode_Medium_Click(object sender, RoutedEventArgs e)
        {
            PVE = 2;
            EVE = 0;
            oppositeLabel1.Content = "自己";
            oppositeLabel2.Content = "中等电脑";
            init();
        }
        private void newGameButton_AIMode_Hard_Click(object sender, RoutedEventArgs e)
        {
            PVE = 3;
            EVE = 0;
            oppositeLabel1.Content = "自己";
            oppositeLabel2.Content = "困难电脑";
            init();
        }
        void init()
        {
            timer.Stop();
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                    cellList[i, j].IsEnabled = true;
            black = white = able = blackstep = whitestep = 0;
            color = 1;
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                    board[i,j] = 0;
            journal.Items.Clear();
            
            board[4, 4] = -1;
            board[4, 5] = 1;
            board[5, 4] = 1;
            board[5, 5] = -1;
            /*
            board[4, 3] = 1;
            board[4, 4] = 1;
            board[4, 5] = 1;
            board[5, 4] = 1;
            board[5, 5] = -1;
            board[6, 5] = 1;
            //board[7, 5] = -1;
            board[7, 7] = -1;
            board[6, 6] = -1;
            board[6, 7] = 1;
            color=-1;
            */
            probRes(board,color); 
            statistic();
            printScreen();
            
            if (grid.Effect is BlurEffect)
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += new EventHandler(GradientOutTimer);
                timer.Interval = TimeSpan.FromSeconds(0.016);
                timer.Start();
            }
        }
        void gameOver()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(GradientInTimer);
            timer.Interval = TimeSpan.FromSeconds(0.016);
            BlurEffect effect = new BlurEffect();
            effect.Radius = 0;
            effect.RenderingBias = RenderingBias.Quality;
            grid.Effect = effect;
            timer.Start();
            if (black != white)
            {
                if (black > white)
                    win1++;
                else win2++;
            }
            printScreen();
            tip.Content =(black == white ? "旗鼓相当的对手！":black > white ? "黑方获胜！" : "白方获胜！");
            journal.Items.Add(new Label()
            {
                Content = (black == white ? "旗鼓相当的对手！" : black > white ? "本局结果：黑胜。" : "本局结果：白胜。"),
                Foreground = Brushes.White,
            });
            journal.SelectedIndex = journal.Items.Count - 1;
            journal.ScrollIntoView(journal.Items[journal.Items.Count - 1]);
            Label label1 = new Label();
            label1.Margin = new Thickness(276,251,0,0);
            label1.Foreground = tip.Foreground;
            label1.FontSize = tip.FontSize;
        }

        private void newGameButton_Click(object sender, RoutedEventArgs e)
        {
            oppositeLabel1.Content = "玩家1";
            oppositeLabel2.Content = "玩家2";
            PVE = 0;
            EVE = 0;
            init();
        }
        void AIThinking_Easy(object sender,EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                    cellList[i, j].IsEnabled = true;
            timer.Stop();
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    if (board[i,j] == 2)
                    {
                        board[i,j] = color;
                        journal.Items.Add(new Label() {
                        Content = (blackstep + whitestep + 1) + ". "+ (color == 1 ? "黑方" : "白方") + "在 " + (char)(j+'A' - 1) + i + " 处落子",
                        Foreground = Brushes.White,
                        });
                        journal.SelectedIndex = journal.Items.Count - 1;
                        journal.ScrollIntoView(journal.Items[journal.Items.Count - 1]);
                        if (color == 1)
                            cellList[i, j].Source = blackSource;
                        else cellList[i, j].Source = whiteSource;
                        if (color == 1) blackstep++;
                        else whitestep++;
                        color = -color;
                        rev(board,i, j);
                        probRes(board,color);
                        statistic();
                        printScreen();
                        if (able == 0)
                        {
                            color = -color;
                            probRes(board,color);
                            statistic();
                            if (able == 0)
                            {
                                gameOver();
                                return;
                            }
                            else
                            {
                                journal.Items.Add(new Label()
                                {
                                    Content = (color == 1 ? "白方" : "黑方") + "无棋可下。",
                                    Foreground = Brushes.White,
                                });
                                if (PVE >= 1 || EVE != 0) tip.Content = (color == 1 ? "黑方" : "白方") + "电脑正在思考...";
                                else tip.Content = "现在轮到 白方 下棋";
                                journal.SelectedIndex = journal.Items.Count - 1;
                                journal.ScrollIntoView(journal.Items[journal.Items.Count - 1]);
                                AIMode_Easy();
                                printScreen();
                            }
                        }
                        return;
                    }
                }
            }
        }
        const int DEEP = 1;
        int alphaBetaDfs(int[,]board,int x,int y,int color,int d,int alpha,int beta)
        {
            if (d == DEEP)
                return evaluate(board, color, x, y);
            probRes(board, color);
            int[,] b = new int[9, 9];
            for (int k = 1; k <= 8; k++)
                for (int l = 1; l <= 8; l++)
                    b[k, l] = board[k, l];
            int ans = d % 2 == 0 ? -inf : inf;
            for(int i=1;i<=8;i++)
               for(int j=1;j<=8;j++)
                {
                    if(board[i,j]==2)
                    {
                        if (d % 2 == 1)
                        {
                            int v = alphaBetaDfs(b, i, j, color, d + 1,alpha,beta);
                            if (v < alpha) return -inf;
                            beta = min(beta, v);
                            ans = min(ans, v);
                        }   
                        else
                        {
                            int v = alphaBetaDfs(b, i, j, color, d + 1,alpha,beta);
                            if (v > beta) return inf;
                            alpha = max(alpha, v);
                            ans = max(ans, v);
                        }
                    }
                }
            if (ans == inf || ans == -inf) ans = ~ans + 1;
            return ans;
        }
        void AIThinking_Hard(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                    cellList[i, j].IsEnabled = true;
            timer.Stop();
            int maxx = -inf;
            int x = 0;
            int y = 0;
            int alpha = -inf;
            int beta = inf;
            for(int i=1;i<=8;i++)
                for(int j=1;j<=8;j++)
                {
                    if(board[i,j]==2)
                    {
                        int[,] b = new int[9, 9];
                        for (int k = 1; k <= 8; k++)
                            for (int l = 1; l <= 8; l++)
                                b[k,l] = board[k, l];
                        b[i, j] = color;
                        rev(b, i, j);
                        probRes(b, -color);
                        int temp = alphaBetaDfs(b, i, j, -color, 1, alpha, beta);
                        if (temp >= maxx)
                        {
                            maxx = temp;
                            x = i;
                            y = j;
                        }
                        alpha = max(alpha, temp);
                        
                    }
                }
                board[x, y] = color;
                journal.Items.Add(new Label()
                {
                    Content = (blackstep + whitestep + 1) + ". " + (color == 1 ? "黑方" : "白方") + "在 " + (char)(y + 'A' - 1) + x + " 处落子",
                    Foreground = Brushes.White,
                });
                journal.SelectedIndex = journal.Items.Count - 1;
                journal.ScrollIntoView(journal.Items[journal.Items.Count - 1]);
                if (color == 1)
                    cellList[x, y].Source = blackSource;
                else cellList[x, y].Source = whiteSource;
                if (color == 1) blackstep++;
                else whitestep++;
                color = -color;
                rev(board,x, y);
                probRes(board,color); 
                statistic();
                printScreen();
                if (able == 0)
                {
                    color = -color;
                    probRes(board,color); ;
                    statistic();
                    if (able == 0)
                    {
                        gameOver();
                        return;
                    }
                    else
                    {
                        journal.Items.Add(new Label()
                        {
                            Content = (color == 1 ? "白方" : "黑方") + "无棋可下。",
                            Foreground = Brushes.White,
                        });
                        journal.SelectedIndex = journal.Items.Count - 1;
                        journal.ScrollIntoView(journal.Items[journal.Items.Count - 1]);
                        if (PVE >= 1 || EVE != 0) tip.Content = (color == 1 ? "黑方" : "白方") + "电脑正在思考...";
                        else tip.Content = "现在轮到 白方 下棋";
                        AIMode_Hard();
                        if (EVE == 2) AIMode_Medium();
                        else if (EVE == 3) AIMode_Hard();
                        printScreen();
                    }
                }
                if (EVE == 2) AIMode_Medium();
                else if (EVE == 3) AIMode_Hard();
            
        }

        void AIThinking_Medium(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                    cellList[i, j].IsEnabled = true;
            timer.Stop();
            bool ok = false;
            int maxx = -0x3f3f3f3f;
            int x=1, y=1;
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    if (board[i,j] == 2)
                    {
                        ok = true;
                        board[i, j] = color;
                        int cnt = tryRev(i, j);
                        board[i, j] = 2;
                        if(cnt > maxx)
                        {
                            maxx = cnt;
                            x = i;
                            y = j;
                        }
                    }
                }
            }
            if (ok)
            {
                board[x,y] = color;
                journal.Items.Add(new Label()
                {
                    Content = (blackstep + whitestep + 1) + ". " + (color == 1 ? "黑方" : "白方") + "在 " + (char)(y + 'A' - 1) + x + " 处落子",
                    Foreground = Brushes.White,
                });
                journal.SelectedIndex = journal.Items.Count - 1;
                journal.ScrollIntoView(journal.Items[journal.Items.Count - 1]);
                if (color == 1)
                    cellList[x, y].Source = blackSource;
                else cellList[x, y].Source = whiteSource;
                if (color == 1) blackstep++;
                else whitestep++;
                color = -color;
                rev(board,x, y);
                probRes(board,color);
                statistic();
                printScreen();
                if (able == 0)
                {
                    color = -color;
                    probRes(board,color);;
                    statistic();
                    if (able == 0)
                    {
                        gameOver();
                        return;
                    }
                    else
                    {
                        journal.Items.Add(new Label()
                        {
                            Content = (color == 1 ? "白方" : "黑方") + "无棋可下。",
                            Foreground = Brushes.White,
                        });
                        journal.SelectedIndex = journal.Items.Count - 1;
                        journal.ScrollIntoView(journal.Items[journal.Items.Count - 1]);
                        if (PVE >= 1 || EVE != 0) tip.Content = (color == 1 ? "黑方" : "白方") + "电脑正在思考...";
                        else tip.Content = "现在轮到 白方 下棋";
                        AIMode_Medium();
                        printScreen();
                        if (EVE == 1) AIMode_Medium();
                        else if (EVE == 2) AIMode_Hard();
                    }
                }
                if (EVE == 1) AIMode_Medium();
                else if (EVE == 2) AIMode_Hard();
            }
            return;
        }

        private void bkg_MediaEnded(object sender, RoutedEventArgs e)
        {
            bkg.Stop();
            bkg.Play();
        }

        private void AIModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AIModeButton.Content.Equals("收起"))
            {
                AIModeButton1.Visibility = AIModeButton2.Visibility = AIModeButton3.Visibility = Visibility.Visible;
                AIModeButton.Content = "收起";
            }
            else
            {
                AIModeButton1.Visibility = AIModeButton2.Visibility = AIModeButton3.Visibility = Visibility.Collapsed;
                AIModeButton.Content = "AI博弈";
            }
        }

        private void AIModeButton1_Click(object sender, RoutedEventArgs e)
        {
            AIModeButton1.Visibility = AIModeButton2.Visibility = AIModeButton3.Visibility = Visibility.Collapsed;
            AIModeButton.Content = "AI博弈";
            oppositeLabel1.Content = "中等电脑";
            oppositeLabel2.Content = "中等电脑";
            EVE = 1;
            init();
            AIMode_Medium();
        }

        private void AIModeButton2_Click(object sender, RoutedEventArgs e)
        {
            AIModeButton1.Visibility = AIModeButton2.Visibility = AIModeButton3.Visibility = Visibility.Collapsed;
            AIModeButton.Content = "AI博弈";
            oppositeLabel1.Content = "中等电脑";
            oppositeLabel2.Content = "困难电脑";
            EVE = 2;
            init();
            AIMode_Medium();
        }

        private void AIModeButton3_Click(object sender, RoutedEventArgs e)
        {
            AIModeButton1.Visibility = AIModeButton2.Visibility = AIModeButton3.Visibility = Visibility.Collapsed;
            AIModeButton.Content = "AI博弈";
            oppositeLabel1.Content = "困难电脑";
            oppositeLabel2.Content = "困难电脑";
            EVE = 3;
            init();
            AIMode_Hard();
        }

        void AIMode_Easy()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(AIThinking_Easy);
            if (EVE == 0)
                timer.Interval = TimeSpan.FromSeconds(0.5);
            tip.Content = (color == 1 ? "黑方" : "白方") + "电脑正在思考...";
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                    cellList[i, j].IsEnabled = false;
            timer.Start();
        }
        void AIMode_Medium()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(AIThinking_Medium);
            if(EVE == 0)
                timer.Interval = TimeSpan.FromSeconds(0.5);
            tip.Content = (color == 1 ? "黑方" : "白方") + "电脑正在思考...";
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                    cellList[i, j].IsEnabled = false;
            timer.Start();
        }
        void AIMode_Hard()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(AIThinking_Hard);
            if (EVE == 0)
                timer.Interval = TimeSpan.FromSeconds(0.1);
            tip.Content = (color == 1 ? "黑方" : "白方") + "电脑正在思考...";
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                    cellList[i, j].IsEnabled = false;
            timer.Start();
        }
        private void cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image cell = (Image)sender;
            int x = cell.Name[4]-'0';
            int y = cell.Name[5]-'0';
            if (board[x,y] != 2)return;
            journal.Items.Add(new Label()
            {
                Content = (blackstep + whitestep + 1) + ". " + (color == 1 ? "黑方" : "白方") + "在 " + (char)('A' + y - 1) + x + " 处落子",
                Foreground = Brushes.White,
            });
            journal.SelectedIndex = journal.Items.Count - 1;
            journal.ScrollIntoView(journal.Items[journal.Items.Count - 1]);
            if (color == 1) blackstep++;
            else whitestep++;
            board[x,y] = color;
            color = -color;
            rev(board,x, y);
            probRes(board,color);
            statistic();
            printScreen();
            if (able == 0)
            {
                color = -color;
                probRes(board,color);
                statistic();
                if (able == 0)
                {
                    gameOver();
                    return;
                }
                else
                {
                    journal.Items.Add(new Label()
                    {
                        Content = (color == 1 ? "白方" : "黑方") + "无棋可下。",
                        Foreground = Brushes.White,
                    });
                    journal.SelectedIndex = journal.Items.Count - 1;
                    journal.ScrollIntoView(journal.Items[journal.Items.Count - 1]);
                    printScreen();
                }
            }
            else
            {
                if (PVE == 1)
                {
                    AIMode_Easy();
                }
                else if(PVE == 2)
                {
                    AIMode_Medium();
                }
                else if (PVE == 3)
                {
                    AIMode_Hard();
                }
            }
        }
    }
}
