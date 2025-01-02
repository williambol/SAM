using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
using ReactiveUI;
using Sam.Models;

namespace Sam.ViewModels;

public class CellViewModel : ViewModelBase
{
    public Point Point { get; set; } = new Point
    {
        X = 0,
        Y = 0,
    };

    private int? _value = null;

    public int? Value
    {
        get => _value;
        set
        {
            this.RaiseAndSetIfChanged(ref _value, CheckValue(value));
            //UpdateAll();
        }
    }

    private bool _isFocused;
    public bool IsFocused
    {
        get => _isFocused;
        set => this.RaiseAndSetIfChanged(ref _isFocused, value);
    }
    
    private double _fontSize = 20;
    public double FontSize
    {
        get => _fontSize;
        set => this.RaiseAndSetIfChanged(ref _fontSize, value);
    }
    
    private double _possibleValueFontSize = 20;
    public double PossibleValueFontSize
    {
        get => _possibleValueFontSize;
        set => this.RaiseAndSetIfChanged(ref _possibleValueFontSize, value);
    }
    
    private List<int> _possibleValues = [1,2,3,4,5,6,7,8,9];
    public List<int> PossibleValues { get => _possibleValues; set => this.RaiseAndSetIfChanged(ref _possibleValues, value); }

    public List<CellViewModel> Row { get; set; } = [];
    
    public List<CellViewModel> Column { get; set; } = [];
    
    public List<CellViewModel> SubGrid { get; set; } = [];

    public List<CellViewModel> All => [..Row, ..Column, ..SubGrid];
    
    public List<CellViewModel> Others => All.Where(c => c != this && c.Value != null).Distinct().ToList();

    private int? CheckValue(int? value)
    {
        return value;
    }
    
    public void UpdateAll()
    {
        All.Distinct()
            .ToList()
            .ForEach(c => c.UpdatePossibleValues());
    }

    public void UpdatePossibleValues()
    {
        if (Value != null)
        {
            PossibleValues = [];
            return;
        }
        
        List<int> possibleValues = [1,2,3,4,5,6,7,8,9];

        List<int?> foundValues = Others.Select(c => c.Value).Where(c => c != null).Distinct().ToList();

        possibleValues = possibleValues.Where(v => !foundValues.Contains(v)).ToList();
        
        PossibleValues = possibleValues;
    }
    
    public void RemovePossibleValues(List<int> possibleValues)
    {
        if (Value != null)
        {
            PossibleValues = [];
            return;
        }
        
        List<int> currentPossibleValues = [..PossibleValues];
        currentPossibleValues = currentPossibleValues.Except(possibleValues).ToList();
        
        PossibleValues = currentPossibleValues;
    }
}