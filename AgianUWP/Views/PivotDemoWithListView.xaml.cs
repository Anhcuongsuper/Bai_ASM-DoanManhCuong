using AgianUWP.DataAccessLibrary;
using AgianUWP.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AgainUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PivotDemoWithListView : Page
    {
        private bool _isPlaying = false;

        private int _currentIndex = 0;

        private ObservableCollection<Song> listSong;

        internal ObservableCollection<Song> ListSong { get => listSong; set => listSong = value; }

        public PivotDemoWithListView()
        {
            this.ListSong = new ObservableCollection<Song>();
            this.ListSong.Add(new Song()
            {
                name = "Buồn Của Anh",
                singer = "G-Dragon",
                thumbnail = "https://avatar-nct.nixcdn.com/song/2017/12/19/1/e/0/d/1513657073338_640.jpg",
                link = "https://c1-ex-swe.nixcdn.com/NhacCuaTui963/ChuaBaoGioSEESINGSHARE2-HaAnhTuan-5111026.mp3"
            });
            
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Song song = new Song()
            {
                name = this.SongName.Text,
                thumbnail = this.SongThumbnail.Text
            };
            this.ListSong.Add(song);
            this.SongName.Text = string.Empty;
            this.SongThumbnail.Text = string.Empty;
            MusicModel.Add(song);
        }

        private void Choosed_Song(object sender, TappedRoutedEventArgs e)
        {
            StackPanel panel = sender as StackPanel;
            Song choosed = panel.Tag as Song;
            _currentIndex = this.MyListSong.SelectedIndex;
            Uri mp3Link = new Uri(choosed.link);
            this.MyPlayer.Source = mp3Link;
            this.Song_Name.Text = this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
            Play_Song();
        }

        private void Play_Song()
        {
            _isPlaying = true;
            this.Player_Status.Text = "Now Playing: ";
            this.MyPlayer.Play();
            this.Play_Button.Symbol = Symbol.Pause;
        }

        private void Pause_Song()
        {
            _isPlaying = false;
            this.Player_Status.Text = "Paused: ";
            this.MyPlayer.Pause();
            this.Play_Button.Symbol = Symbol.Play;
        }

        private void Click_Play(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                Pause_Song();
            }
            else
            {
                Play_Song();
            }
        }

        private void Do_Next(object sender, RoutedEventArgs e)
        {
            Pause_Song();
            _currentIndex += 1;
            if (_currentIndex >= this.ListSong.Count)
            {
                _currentIndex = 0;
            }
            Uri mp3Link = new Uri(this.ListSong[_currentIndex].link);
            this.MyPlayer.Source = mp3Link;
            this.Song_Name.Text = this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
            this.MyListSong.SelectedIndex = _currentIndex;
            Play_Song();
        }

        private void Do_Previous(object sender, RoutedEventArgs e)
        {
            Pause_Song();
            _currentIndex -= 1;
            if (_currentIndex < 0)
            {
                _currentIndex = this.ListSong.Count - 1;
            }
            Uri mp3Link = new Uri(this.ListSong[_currentIndex].link);
            this.MyPlayer.Source = mp3Link;
            this.Song_Name.Text = this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
            this.MyListSong.SelectedIndex = _currentIndex;
            Play_Song();
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            string msg = String.Format("Current value: {0}", e.NewValue);
            this.time.Text = msg;

        }

        private void Slider_ValueChanged_1(object sender, RangeBaseValueChangedEventArgs e)
        {

        }
    }
}