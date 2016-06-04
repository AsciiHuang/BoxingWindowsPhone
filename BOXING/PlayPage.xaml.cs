using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using System.Diagnostics;
using System.IO.IsolatedStorage;

namespace BOXING
{
    public partial class PlayPage : PhoneApplicationPage
    {
        private BitmapImage mBackgroundOrigin = null; // 在 Silverlight 內於 Code 中修改 Source 有 bug
        private BitmapImage mBackgroundScore = null; // 所以改用兩個 Image 隱藏與顯示比較安全 *-*
        private int mWaitSecond = 2;
        private DispatcherTimer mFightWaiter = null; // 負責數 321 Fight 用的
        private Accelerometer mAcc = null;
        private DispatcherTimer mScoreWaiter = null; // 負責每秒檢查是否已經出擊完畢用的
        private List<AccData> mMagnitudeList = null;
        private int mTagIndex = 0;
        private Boolean mIsFindTag = false;
        private DateTime mStartTime = DateTime.Now;
        Double mDiffMilliSeconds = 0.0;

        public PlayPage()
        {
            InitializeComponent();
            //mBackgroundOrigin = new BitmapImage(new Uri("/Image/bg_game.jpg", UriKind.Relative));
            //mBackgroundScore = new BitmapImage(new Uri("/Image/bg_score.jpg", UriKind.Relative));
            mAcc = new Accelerometer();
            mAcc.ReadingChanged += OnAccelerometerReadingChanged;
            mMagnitudeList = new List<AccData>();
            if (mFightWaiter == null)
            {
                mFightWaiter = new DispatcherTimer();
                mFightWaiter.Interval = TimeSpan.FromSeconds(1.0);
                mFightWaiter.Tick += new EventHandler(OnFightWaiterTimerTick);
            }
            if (mScoreWaiter == null)
            {
                mScoreWaiter = new DispatcherTimer();
                mScoreWaiter.Interval = TimeSpan.FromSeconds(1.0);
                mScoreWaiter.Tick += new EventHandler(OnScoreWaiterTimerTick);
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            mAcc.Stop();
            mWaitSecond = 2;
            mIsFindTag = false;
            mMagnitudeList.Clear();
            imgBackground.Visibility = Visibility.Visible;
            imgScore.Visibility = Visibility.Collapsed;
            panelScore.Visibility = Visibility.Collapsed;
            mFightWaiter.Start();
            mScoreWaiter.Stop();
            lblStatus.Text = "Ready：3";
            lblStatus.Visibility = Visibility.Visible;
        }

        private void OnFightWaiterTimerTick(object sender, EventArgs e)
        {
            if (mWaitSecond < 0)
            {
                mFightWaiter.Stop();
                lblStatus.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (mWaitSecond < 1)
                {
                    StartSensor();
                    lblStatus.Text = "Fight";
                }
                else
                {
                    lblStatus.Text = String.Format("Ready：{0}", mWaitSecond);
                }
                --mWaitSecond;
            }
        }

        private void OnScoreWaiterTimerTick(object sender, EventArgs e)
        {
            if (mIsFindTag)
            {
                mScoreWaiter.Stop();
                ShowAnim();
            }
            else
            {
                Double dblPreX = 0.0;
                for (int i = 0; i < mMagnitudeList.Count; ++i)
                {
                    if (mMagnitudeList[i].mX < 0.0 && dblPreX > 0.0)
                    {
                        // 找到 tag 了
                        mDiffMilliSeconds = (mMagnitudeList[i].mTime - mStartTime).TotalMilliseconds;
                        mIsFindTag = true;
                        mTagIndex = i;
                        break;
                    }
                    dblPreX = mMagnitudeList[i].mX;
                }
            }
        }

        private void StartSensor()
        {
            try
            {
                mAcc.Start();
                mScoreWaiter.Start();
                mStartTime = DateTime.Now;
            }
            catch (Exception)
            {
            }
        }

        private void OnAccelerometerReadingChanged(object sender, AccelerometerReadingEventArgs args)
        {
            Double Magnitude = Math.Sqrt(args.X * args.X + args.Y * args.Y + args.Z * args.Z);
            if (Magnitude >= 1.5)
            {
                mMagnitudeList.Add(new AccData(args.X, args.Y, args.Z, Magnitude, args.Timestamp));
            }
        }

        private void ShowAnim()
        {
            // 計算並秀動畫、秀成績
            VisibleAnim.Begin();
        }

        private void VisibleAnim_Completed(object sender, EventArgs e)
        {
            imgScore.Visibility = Visibility.Visible;
            ShowScore();
        }

        private void ShowScore()
        {
            String strScoreDetail = "";
            Double dblAvgX = 0.0, dblAvgG = 0.0, dblTtlG = 0.0, dblTtlX = 0.0;
            int nAgile = 0, nPower = 0, nScore = 0;

            #region 力量
            int nValidCount = 0;
            for (int i = 0; i < mTagIndex; ++i)
            {
                // 找出最大的 G 與 X、時間
                //if (mMagnitudeList[i].mG > dblMaxG)
                //{
                //    dblMaxG = mMagnitudeList[i].mG;
                //}
                //if (mMagnitudeList[i].mX > dblMaxX)
                //{
                //    dblMaxX = mMagnitudeList[i].mX;
                //}

                // 上面的測試結果會每次都很強…所以只取最大的 X 就好了…
                //if (mMagnitudeList[i].mX > dblMaxX)
                //{
                //    dblMaxX = mMagnitudeList[i].mX;
                //    dblMaxG = mMagnitudeList[i].mG;
                //}

                // 想想還是求平均好了 XD
                if (mMagnitudeList[i].mX >= 1.5)
                {
                    ++nValidCount;
                    dblTtlX += mMagnitudeList[i].mX;
                    dblTtlG += mMagnitudeList[i].mG;
                }
            }

            dblAvgX = dblTtlX / nValidCount;
            dblAvgG = dblTtlG / nValidCount;

            //nPower = (int)((Math.Pow(dblMaxX, 2.0) * Math.Pow(dblMaxG, 2.0)) * 10);
            nPower = (int)((Math.Pow(dblAvgX, 2.0) * Math.Pow(dblAvgG, 2.0)) * 10);
            #endregion

            #region 敏捷
            if (mDiffMilliSeconds >= 3000.0 || mDiffMilliSeconds < 0)
            {
                // 超過 3 秒，敏捷分數就為零
                nAgile = 0;
            }
            else if (mDiffMilliSeconds <= 500.0)
            {
                // 500 毫秒內就算滿分 50 分
                nAgile = 50;
            }
            else
            {
                nAgile = (int)((3000.0 - (Double)mDiffMilliSeconds) / 60); // 60 = 3000 / 50
            }
            #endregion

            #region 成績
            if (nAgile > 0)
            {
                nScore = nPower + (int)(Math.Sqrt(nAgile) + 0.99);
            }
            else
            {
                nScore = nPower;
            }
            #endregion

            strScoreDetail = String.Format("Power : {0}\nAgile : {1}\nScore : {2}", nPower, nAgile, nScore);
            lblScore.Text = strScoreDetail;

            panelScore.Visibility = Visibility.Visible;

            RefreshHonor(nPower, nAgile, nScore);
        }

        private void RefreshHonor(int nPower, int nAgile, int nScore)
        {
            int nOriPower = 0, nOriAgile = 0, nOriScore = 0;

            nOriPower = Utility.GetKeyValue(Constants.SETTING_KEY_MAX_POWER);
            nOriAgile = Utility.GetKeyValue(Constants.SETTING_KEY_MAX_AGILE);
            nOriScore = Utility.GetKeyValue(Constants.SETTING_KEY_MAX_SCORE);

            Boolean bNew = false;
            if (nPower > nOriPower)
            {
                IsolatedStorageSettings.ApplicationSettings[Constants.SETTING_KEY_MAX_POWER] = nPower;
                bNew = true;
            }
            if (nAgile > nOriAgile)
            {
                IsolatedStorageSettings.ApplicationSettings[Constants.SETTING_KEY_MAX_AGILE] = nAgile;
                bNew = true;
            }
            if (nScore > nOriScore)
            {
                IsolatedStorageSettings.ApplicationSettings[Constants.SETTING_KEY_MAX_SCORE] = nScore;
            }
            if (bNew)
            {
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        private void OnAgainButtonClick(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            OnExit();
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void OnExit()
        {
            if (mFightWaiter != null)
            {
                mFightWaiter.Stop();
            }
            if (mScoreWaiter != null)
            {
                mScoreWaiter.Stop();
            }
            if (mAcc != null)
            {
                mAcc.Stop();
            }
        }
    }
}