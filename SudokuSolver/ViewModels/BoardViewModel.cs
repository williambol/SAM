using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using SudokuSolver.Models;

namespace SudokuSolver.ViewModels;

public class BoardViewModel : ViewModelBase
{
    /*private readonly List<int?> _seed = //easy
    [
        6   , 5   , null,    null, null, 3   ,    2   , 4   , 7   ,
        4   , null, null,    null, null, 7   ,    null, null, null,
        9   , null, 7   ,    2   , 5   , null,    null, null, 3   ,
        
        2   , null, null,    3   , null, null,    null, 8   , 1   ,
        null, null, 1   ,    7   , null, 6   ,    4   , null, 9   ,
        7   , null, 3   ,    8   , 9   , null,    5   , 2   , null,
        
        null, 6   , null,    null, null, 9   ,    3   , null, null,
        3   , 7   , 4   ,    null, null, null,    9   , null, null,
        1   , null, 9   ,    null, null, null,    null, null, 4   ,
    ];*/
    
    /*private readonly List<int?> _seed = //medium
    [
        6   , null, 9   ,    8   , null, null   ,    7   , 5   , 2   ,
        5   , null, 1,    6, 7, null   ,    4, 8, 9,
        null   , 2, null   ,    null   , 4   , null,    null, null, null   ,
        
        null   , null, null,    4   , null, null,    null, 7   , null   ,
        null, null, 5   ,    null   , null, null   ,    9   , null, 4   ,
        3   , null, 4   ,    9   , null   , null,    1   , 6   , null,
        
        2, null   , 8,    3, 9, 6   ,    5   , null, 7,
        null   , null   , 3   ,    2, null, null,    null   , 9, 1,
        null   , null, 6   ,    null, null, 7,    null, null, null   ,
    ];*/
    
    /*private readonly List<int?> _seed = //hard
    [
        null   , null, 4   ,    9   , null, null   ,    5   , null   , 6   ,
        null   , 1, null,    null, null, 3   ,    null, 9, null,
        null   , 9, 2   ,    null   , null   , null,    3, null, 7   ,
        
        3   , null, null,    6   , 7, null,    null, null   , 8   ,
        null, null, null   ,    null   , null, 1   ,    2   , null, null   ,
        null   , null, null   ,    null   , null   , null,    null   , 3   , null,
        
        null, null   , 6,    1, null, null   ,    8   , null, null,
        1   , null   , 8   ,    null, null, 6,    null   , 5, 4,
        null   , null, 3   ,    null, 8, 4,    null, null, 9   ,
    ];*/
    
    /*private readonly List<int?> _seed = //expert
    [
        null, null, null,    null, null, null,    null, 7, null,
        null, null, null,    null, 1, 7,    null, 2, null,
        2, 1, 7,    null, null, null,    8, 6, 3,
        
        null, 8, 6,    null, null, 2,    7, null, null,
        null, null, 5,    7, null, null,    null, null, null,
        4, 7, null,    1, null, null,    null, 9, 6,
        
        null, 5, 9,    null, null, null,    6, 3, null,
        6, null, 3,    9, null, 8,    null, null, null,
        null, null, null,    6, null, null,    2, null, 9,
    ];*/
    
    private readonly List<int?> _seed = //master
    [
        null, 8, 7,    null, null, 3,    9, null, null,
        null, 5, null,    null, null, null,    null, null, 3,
        null, null, null,    null, 6, 5,    null, 2, 7,

        3, null, null,    null, null, null,    null, null, 1,
        null, null, null,    null, null, 8,    null, null, null,
        null, 9, null,    null, 7, 4,    null, null, null,

        4, 7, null,    null, null, null,    null, null, 2,
        null, 6, 2,    4, null, 7,    5, null, 8,
        null, 3, null,    null, 1, null,    null, 6, null,
    ];
    
    private double _width = 300;
    public double Width 
    {
        get => _width;
        set => this.RaiseAndSetIfChanged(ref _width, value);
    }
    
    private double _height = 300;
    public double Height
    {
        get => _height;
        set => this.RaiseAndSetIfChanged(ref _height, value);
    }
    
    private string _title = "Hello Board!";
    public string Title 
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }
    
    private ObservableCollection<CellViewModel> _cells = new ObservableCollection<CellViewModel>();
    public ObservableCollection<CellViewModel> Cells 
    {
        get => _cells;
        set => this.RaiseAndSetIfChanged(ref _cells, value);
    }
    
    public ReactiveCommand<Unit, Unit> Solve { get; }

    private async Task RunSolve()
    {
        bool didFindValues;

        do
        {
            CalculatePossibleValues();
            
            didFindValues = await SolveSinglePossibleValuesAsync();
        }
        while(didFindValues);
    }

    private async Task<bool> SolveSinglePossibleValuesAsync()
    {
        var foundValues = Cells.Where(c => c.PossibleValues.Count == 1)
            .ToList();

        if (foundValues.Count >= 1)
        {
            foundValues.ForEach(c => c.Value = c.PossibleValues.First());
            return true;
        }

        return await SolveUniquePossibleValuesAsync();
    }
    
    private async Task<bool> SolveUniquePossibleValuesAsync()
    {
        bool foundValue = false;
        
        Cells
            .ToList()
            .ForEach(c =>
            {
                var uniquePossibleValues = c.PossibleValues
                    .Where(v =>
                        c.Row
                            .Where(rc => rc != c) //find all other cells in this cells row.
                            .All(rc => !rc.PossibleValues.Contains(v))//Check that no other cells in this row contain can be this possible value. Or the cell being inspected is the only cell in this row that can be that possible value.
                    )
                    .ToList(); //Check 

                if (uniquePossibleValues.Count >= 1)
                {
                    c.Value = uniquePossibleValues[0];
                    foundValue = true;
                    return;
                }
                
                uniquePossibleValues = c.PossibleValues
                    .Where(v =>
                            c.Column
                                .Where(rc => rc != c) //find all other cells in this cells row.
                                .All(rc => !rc.PossibleValues.Contains(v))//Check that no other cells in this row contain can be this possible value. Or the cell being inspected is the only cell in this row that can be that possible value.
                    )
                    .ToList(); //Check 

                if (uniquePossibleValues.Count >= 1)
                {
                    c.Value = uniquePossibleValues[0];
                    foundValue = true;
                    return;
                }
                
                uniquePossibleValues = c.PossibleValues
                    .Where(v =>
                            c.SubGrid
                                .Where(rc => rc != c) //find all other cells in this cells row.
                                .All(rc => !rc.PossibleValues.Contains(v))//Check that no other cells in this row contain can be this possible value. Or the cell being inspected is the only cell in this row that can be that possible value.
                    )
                    .ToList(); //Check 

                if (uniquePossibleValues.Count >= 1)
                {
                    c.Value = uniquePossibleValues[0];
                    foundValue = true;
                }
            });

        return foundValue;
    }

    private void CalculatePossibleValues()
    {
        _cells.ToList().ForEach(c => c.UpdatePossibleValues());
    }
    
    public BoardViewModel()
    {
        Solve = ReactiveCommand.CreateFromTask(RunSolve);
        
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                _cells.Add(new CellViewModel
                {
                    Point = new Point
                    {
                        X = j,
                        Y = i
                    },
                    Value = _seed[i * 9 + j],
                });
            }
        }

        for (int i = 0; i < 9; i++)
        {
            List<CellViewModel> row = _cells
                .Where(c => c.Point.X == i)
                .ToList();
            
            row.ForEach(c => c.Row = row);
            
            List<CellViewModel> column = _cells
                .Where(c => c.Point.Y == i)
                .ToList();
            
            column.ForEach(c => c.Column = column);
            
            //i   => 0, 1, 2, 3, 4, 5, 6, 7, 8
            //i%3 => 0, 1, 2, 0, 1, 2, 0, 1, 2
            //i/3 => 0, 0, 0, 1, 1, 1, 2, 2, 2
            //x   => 0, 3, 6, 0, 3, 6, 0, 3, 6 = (i%3)*3
            //y   => 0, 0, 0, 3, 3, 3, 6, 6, 6 = i/3*3
            int xLower = i % 3 * 3;
            int xUpper = xLower + 2;

            int yLower = i / 3 * 3;
            int yUpper = yLower + 2;
            
            List<CellViewModel> subGrid = _cells
                .Where(c => c.Point.X >= xLower && c.Point.X <= xUpper && c.Point.Y >= yLower && c.Point.Y <= yUpper)
                .ToList();
            
            subGrid.ForEach(c => c.SubGrid = subGrid);
        }

        CalculatePossibleValues();
    }
}