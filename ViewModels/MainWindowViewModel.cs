﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using ReactiveUI;

namespace Mp3Player.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Uri _actualFileUri;

        public Uri ActualFileUri
        {
            get => _actualFileUri;
            set => this.RaiseAndSetIfChanged(ref _actualFileUri, value);
        }

        private string _title;

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }
        
        private Timer _actualTime = new Timer();

        public Timer ActualTime
        {
            get => _actualTime;
            set => this.RaiseAndSetIfChanged(ref _actualTime, value);
        }

        private Timer _audioLength = new Timer();

        public Timer AudioLength
        {
            get => _audioLength;
            set => this.RaiseAndSetIfChanged(ref _audioLength, value);
        }
        
        private float _position;
        public float Position
        {
            get => _position;
            set => this.RaiseAndSetIfChanged(ref _position, value);
        }
        
        private LibVLC _libVlc;
        private Media _media;
        public Media ActualMedia
        {
            get => _media;
            set
            {
                this.RaiseAndSetIfChanged(ref _media, value);
            }
        }
        private MediaPlayer _mediaP;
        public MediaPlayer _mediaPlayer
        {
            get => _mediaP;
            set => this.RaiseAndSetIfChanged(ref _mediaP, value);
        }

        private ObservableCollection<MusicViewModel> _musics;

        public ObservableCollection<MusicViewModel> Musics
        {
            get => _musics;
            set => this.RaiseAndSetIfChanged(ref _musics, value);
        }

        private MusicViewModel _selectedMusic;

        public MusicViewModel SelectedMusic
        {
            get => _selectedMusic;
            set
            {
                if(SelectedMusic != null)
                    SelectedMusic.MPlayer.Stop();
                this.RaiseAndSetIfChanged(ref _selectedMusic, value);
                SelectedMusic.MPlayer.PositionChanged += (sender, args) =>
                {
                    Position = ((SelectedMusic.MPlayer.Position * 100) / 1);
                    var time = TimeSpan.FromMilliseconds(SelectedMusic.MPlayer.Time);
                    ActualTime.SetActual(time);
                };
                SelectedMusic.MPlayer.EndReached += (sender, args) => NextCommand.Execute();
            }
        }

        

        public MainWindowViewModel()
        {
            _libVlc = new LibVLC(enableDebugLogs:true);
            Musics = new ObservableCollection<MusicViewModel>()
            {
                new MusicViewModel("/home/denny/Music/a.mp3", _libVlc),
                new MusicViewModel("/home/denny/Music/VEDZ.m4a", _libVlc),
                new MusicViewModel("/home/denny/Music/VEDZ.m4a", _libVlc),
                new MusicViewModel("/home/denny/Music/VEDZ.m4a", _libVlc)


            };
            
             //ActualFileUri = new Uri("/home/denny/Music/a.mp3");
             PlayCommand = ReactiveCommand.CreateFromTask(async () =>
             {
                 if (SelectedMusic == null)
                 {
                     SelectedMusic = Musics.First();
                     //SelectedMusic?.Play();
                     SelectedMusic.MPlayer.Play();
                 }
                 else
                 {
                     SelectedMusic.MPlayer.Pause();
                 }
                _mediaPlayer?.Dispose();
                /*ActualMedia = new Media(_libVlc, ActualFileUri);
                 _mediaPlayer = new MediaPlayer(ActualMedia);
                 _mediaPlayer.Play(ActualMedia);
                 _mediaPlayer.PositionChanged += (sender, args) =>
                 {
                    Position = ((_mediaPlayer.Position * 100) / 1);
                    var time = TimeSpan.FromMilliseconds(_mediaPlayer.Time);
                    ActualTime.SetActual(time);
                 };
                 ActualMedia.MetaChanged += (sender, args) => Title = _media.Meta(MetadataType.Title);

                 _mediaPlayer.LengthChanged += (sender, args) => AudioLength.SetActual(TimeSpan.FromMilliseconds(_mediaPlayer.Length));*/
            });
            PauseCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _mediaPlayer?.Pause();
            });
            StopCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _mediaPlayer?.Stop();
                Position = 0;
            });
            RestartCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _mediaPlayer?.Stop();
                _mediaPlayer.Position = 0;

            });
            SelectMusicCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                //_mediaPlayer = SelectedMusic.MPlayer;
                //_mediaPlayer.Play();
                //SelectedMusic.MPlayer.Play(SelectedMusic.MPlayer.Media);
                SelectedMusic.Play();
            });
            PreviousCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                int index = Musics.ToList().IndexOf(SelectedMusic);
                if ((index - 1) <= Musics.Count)
                {
                    SelectedMusic = Musics[index-1];
                    SelectedMusic.Play();
                }
                
            });
            NextCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                int index = Musics.ToList().IndexOf(SelectedMusic);
                if ((index + 1) <= Musics.Count)
                {
                    SelectedMusic = Musics[index+1];
                    SelectedMusic.Play();
                }
            });
            OpenFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                string[] files = (await ShowOpenFileDialog.Handle(Unit.Default));
                if (files == null) return;
                List<MusicViewModel> musics = new List<MusicViewModel>();
                foreach (var file in files)
                {
                    musics.Add(new MusicViewModel(file, _libVlc));
                }

                foreach (var music in musics)
                {
                    Musics.Add(music);
                }

                SelectedMusic = musics.First();
                SelectedMusic.Play();
            });
            ShowOpenFileDialog = new Interaction<Unit, string[]>();
            
        }
        private async Task PlayNext()
        {
            
        }
        public ReactiveCommand<Unit, Unit> PlayCommand { get; set; }
        public ReactiveCommand<Unit, Unit> PauseCommand { get; set; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; set; }
        public ReactiveCommand<Unit, Unit> RestartCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SelectMusicCommand { get; set; }
        public ReactiveCommand<Unit, Unit> PreviousCommand { get; set; }
        public ReactiveCommand<Unit, Unit> NextCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenFileCommand { get; set; }
        public Interaction<Unit, string[]> ShowOpenFileDialog { get; set; }
        public void DoubleClickMusic()
        {
            SelectMusicCommand.Execute();
        }
    }
}