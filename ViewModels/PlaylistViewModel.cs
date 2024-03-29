using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mp3Player.Models;
using ReactiveUI;

namespace Mp3Player.ViewModels;

public class PlaylistViewModel : ViewModelBase
{
    public Playlist _playlist;
    private string _name;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private string _search;
    public string Search
    {
        get => _search;
        set
        {
            this.RaiseAndSetIfChanged(ref _search, value);
            if (Musics != null)
            {
                var musics = Musics.Where(m => m.Title.ToLower().Contains(Search.ToLower()) || m.PlaylistFile.FilePath.ToLower().Contains(Search.ToLower()));
                FilteredMusics = new ObservableCollection<MusicViewModel>();
                foreach (var music in musics)
                {
                    FilteredMusics.Add(music);
                }
            }
        }
    }
    
    private ViewModelBase _content;

    public ViewModelBase Content
    {
        get => _content;
        set
        {
            this.RaiseAndSetIfChanged(ref _content, value);
            //((MusicsDataGridViewModel) Content).Playlist = this;

        }
    }
    private ObservableCollection<MusicViewModel> _musics;

    public ObservableCollection<MusicViewModel> Musics
    {
        get => _musics;
        set
        {
            if (Musics != null)
            {
                Musics.CollectionChanged += (sender, args) =>
                {
                    FilteredMusics = new ObservableCollection<MusicViewModel>();
                    foreach (var music in Musics)
                    {
                        FilteredMusics.Add(music);
                    }
                };
            }
            
            this.RaiseAndSetIfChanged(ref _musics, value);
            FilteredMusics = new ObservableCollection<MusicViewModel>();
            foreach (var music in Musics)
            {
                FilteredMusics.Add(music);
            }
        }
    }

    private ObservableCollection<MusicViewModel> _filteredMusics;

    public ObservableCollection<MusicViewModel> FilteredMusics
    {
        get => _filteredMusics;
        set => this.RaiseAndSetIfChanged(ref _filteredMusics, value);
    }

    public PlaylistViewModel(Playlist playlist)
    {
        _playlist = playlist;
        Musics = new ObservableCollection<MusicViewModel>();
        Musics.CollectionChanged += (sender, args) =>
        {
            FilteredMusics = new ObservableCollection<MusicViewModel>();
            foreach (var music in Musics)
            {
                FilteredMusics.Add(music);
            }
        };
        Name = playlist.Name;
    }

    public void Create()
    {
        _playlist.Create();
    }

    public void Save()
    {
        _playlist.PlaylistFiles = new List<PlaylistFile>();
        foreach (var music in Musics)
        {
            _playlist.PlaylistFiles.Add(music.PlaylistFile);
        }
        _playlist.Save();
    }
    public void Delete()
    {
        _playlist.Delete();
    }

    public void RemoveFile(PlaylistFile file)
    {
        _playlist.RemoveFile(file);
    }
    public void Rename()
    {
        _playlist.Rename();
    }
    
}