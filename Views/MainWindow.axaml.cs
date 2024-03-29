using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using DynamicData.Binding;
using Mp3Player.ViewModels;
using ReactiveUI;

namespace Mp3Player.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private List<DataGridRow> _musics = new List<DataGridRow>();
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.ShowOpenFileDialog.RegisterHandler(DoShowDialog)));
            this.WhenActivated(d => d(ViewModel!.ShowNewPlaylistWindow.RegisterHandler(DoShowNewPlaylistWindowAsync)));
            this.WhenActivated(d => d(ViewModel!.ShowOpenFolderDialog.RegisterHandler(DoShowOpenFolderDialog)));
            this.WhenActivated(d => d(ViewModel!.ShowEqualizerDialog.RegisterHandler(DoShowEqualizerDialog)));
            this.FindControl<Canvas>("DragWindowCanvas").PointerPressed += (sender, args) =>
            {
              PlatformImpl?.BeginMoveDrag(args);  
            };
            this.WhenAnyValue(x => x.WindowState)
                .Subscribe(windowState =>
                {
                    var appBorder = this.FindControl<Border>("AppBorder");
                    if (windowState == WindowState.Maximized)
                    {
                    
                        appBorder.CornerRadius = CornerRadius.Parse("0");
                    }
                    else
                    {
                        appBorder.CornerRadius = CornerRadius.Parse("20");
                    }
                });


        }

        private async Task DoShowNewPlaylistWindowAsync(InteractionContext<NewPlaylistWindowViewModel, PlaylistViewModel> interaction)
        {
            var newPlaylistWindow = new NewPlaylistWindow();
            newPlaylistWindow.DataContext = interaction.Input;

            var result = await newPlaylistWindow.ShowDialog<PlaylistViewModel>(this);
            interaction.SetOutput(result);
        }
        private void DataGrid_OnDoubleTapped(object? sender, RoutedEventArgs e)
        {
            var datagrid = sender as DataGrid;

            if (e.Source.GetType() == typeof(Border))
            {
                datagrid.SelectedItem = null;
                datagrid.SelectedIndex = -1;
                return;
            };
            ((MainWindowViewModel) this.DataContext).DoubleClickMusic();
        }

        private async Task DoShowDialog(InteractionContext<Unit, string[]> interactionContext)
        {
            var dialog = new OpenFileDialog();
            dialog.AllowMultiple = true;
            string[] files = await dialog.ShowAsync(this);

            interactionContext.SetOutput(files);
        }

        private async Task DoShowOpenFolderDialog(InteractionContext<Unit, string> interactionContext)
        {
            var dialog = new OpenFolderDialog();
            dialog.Title = "Choose folder to open";
            var folder = await dialog.ShowAsync(this);
            interactionContext.SetOutput(folder);
        }

        private async Task DoShowEqualizerDialog(InteractionContext<EqualizerWindowViewModel, Unit> interactionContext)
        {
            var dialog = new EqualizerWindow();
            dialog.DataContext = interactionContext.Input;
            await dialog.ShowDialog(this);
            interactionContext.SetOutput(Unit.Default);
        }
        private async void MenuItem_ExitOnClick(object? sender, RoutedEventArgs e)
        {   
            this.Close();
        }
        private void Control_OnContextRequested(object? sender, ContextRequestedEventArgs e)
        {
            //((MainWindowViewModel) this.DataContext).ShowContextMenu();
            //e.Handled = true;
        }

        private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine(e);  
        }

        private void DataGrid_OnLoadingRow(object? sender, DataGridRowEventArgs e)
        {
            _musics.Add(e.Row);
        }

        private void DataGrid_OnTapped(object? sender, RoutedEventArgs e)
        {
            if (e.Source.GetType() == typeof(Border))
            {
                var datagrid = sender as DataGrid;
                datagrid.SelectedIndex = -1;
                datagrid.SelectedItem = null;
                return;;
            }
        }

        private void Playlists_OnContextRequested(object? sender, ContextRequestedEventArgs e)
        {
            ((MainWindowViewModel) DataContext).ShowPlaylistsContextMenu();
        }

        private void ExitBorder_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            
            this.Close();
        }

        private void MinimizeBorder_OnPointerPressed(object? sender, PointerReleasedEventArgs e)
        {
            //var border = sender as Border;
            //Console.WriteLine(border.Name);
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            WindowState = WindowState.Minimized;

        }
    }
}