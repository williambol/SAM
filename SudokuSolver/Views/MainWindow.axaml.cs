using System;
using Avalonia.Controls;
using SudokuSolver.ViewModels;

namespace SudokuSolver.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        MainWindowViewModel? mainWindowViewModel = (MainWindowViewModel)DataContext;

        DataContextChanged += (s, e) =>
        {
            mainWindowViewModel = (MainWindowViewModel) DataContext;
        };
        
        SizeChanged += (sender, e) =>
        {
            if (mainWindowViewModel is not null)
            {
                var size = Math.Min(ClientSize.Width, ClientSize.Height);
                size = Math.Max(300, size);
                mainWindowViewModel.Board.Width = size;
                mainWindowViewModel.Board.Height = size;
            }
        };
    }
}