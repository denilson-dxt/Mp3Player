using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Mp3Player.ViewModels;
using ReactiveUI;

namespace Mp3Player.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.ShowOpenFileDialog.RegisterHandler(DoShowDialog)));
        }
        
        private void InputElement_OnDoubleTapped(object? sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel) this.DataContext).DoubleClickMusic();
        }

        private async Task DoShowDialog(InteractionContext<Unit, string[]> interactionContext)
        {
            var dialog = new OpenFileDialog();
            dialog.AllowMultiple = true;
            string[] files = await dialog.ShowAsync(this);

            interactionContext.SetOutput(files);
        }
        private async void MenuItem_ExitOnClick(object? sender, RoutedEventArgs e)
        {   
            this.Close();
            
        }


        private void Control_OnContextRequested(object? sender, ContextRequestedEventArgs e)
        {
            ((MainWindowViewModel) this.DataContext).ShowContextMenu();
            //e.Handled = true;
        }

        private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}