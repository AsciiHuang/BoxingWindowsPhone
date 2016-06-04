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
using System.IO.IsolatedStorage;

namespace BOXING
{
    public partial class InfoOptionPage : PhoneApplicationPage
    {
        public InfoOptionPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            lblVersion.Text = "Version: 1.0.0.0\nCopyright © Ascii Huang. All Rights Reserved.";
            ReflashSoundButton();
        }

        private void OnResetScoresButtonClick(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings[Constants.SETTING_KEY_MAX_POWER] = 0;
            IsolatedStorageSettings.ApplicationSettings[Constants.SETTING_KEY_MAX_AGILE] = 0;
            IsolatedStorageSettings.ApplicationSettings[Constants.SETTING_KEY_MAX_SCORE] = 0;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        private void BtnSound_Click(object sender, RoutedEventArgs e)
        {
            Utility.SetCloseBackgroundMusic();
            ((MediaElement)App.Current.Resources["mediaElement"]).Stop();
            ReflashSoundButton();
        }

        private void BtnNoSound_Click(object sender, RoutedEventArgs e)
        {
            if (Microsoft.Xna.Framework.Media.MediaPlayer.State == Microsoft.Xna.Framework.Media.MediaState.Playing)
            {
                if (MessageBox.Show("The audio output is being used by another application. Do you want to stop it?", "BOXING", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
                    Utility.SetOpenBackgroundMusic();
                    //((MediaElement)App.Current.Resources["mediaElement"]).Play();
                    ((MediaElement)App.Current.Resources["mediaElement"]).Source = new Uri("Sound/bgm.mp3", UriKind.Relative);
                }
            }
            else
            {
                Utility.SetOpenBackgroundMusic();
                //((MediaElement)App.Current.Resources["mediaElement"]).Play();
                ((MediaElement)App.Current.Resources["mediaElement"]).Source = new Uri("Sound/bgm.mp3", UriKind.Relative);
            }
            ReflashSoundButton();
        }

        private void ReflashSoundButton()
        {
            if (Utility.IsPlayBackgoundMusic)
            {
                BtnSound.Visibility = Visibility.Visible;
                BtnNoSound.Visibility = Visibility.Collapsed;
            }
            else
            {
                BtnNoSound.Visibility = Visibility.Visible;
                BtnSound.Visibility = Visibility.Collapsed;
            }
        }
    }
}