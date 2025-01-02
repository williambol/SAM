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
                var size = Math.Min(ClientSize.Width, ClientSize.Height - 40);
                size = Math.Max(280, size - 20);
                Console.WriteLine(size);
                mainWindowViewModel.Board.Width = size;
                mainWindowViewModel.Board.Height = size;
            }
        };
    }
}