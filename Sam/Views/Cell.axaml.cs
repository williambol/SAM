using System;
using Avalonia.Controls;
using Sam.ViewModels;

namespace Sam.Views;

public partial class Cell : UserControl
{
    public Cell()
    {
        InitializeComponent();
        
        CellViewModel? cellViewModel = (CellViewModel)DataContext;

        DataContextChanged += (s, e) =>
        {
            cellViewModel = (CellViewModel)DataContext;
        };
        
        SizeChanged += (sender, e) =>
        {
            if (cellViewModel is not null)
            {
                cellViewModel.FontSize = Math.Max(e.NewSize.Height - 30, 15);
                cellViewModel.PossibleValueFontSize = cellViewModel.FontSize / 2;
            }
        };
    }
}