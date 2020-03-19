# How to append the new text in clipboard which is not available in DataGrid(SfDataGrid)?

## About the sample
This example illustrates how to append the new text in clipboard which is not available in DataGrid(SfDataGrid)?

By default, SfDataGrid copies the selected row or cell data in clipboard, you need to append the new text in clipboard which is not available in SfDataGrid can be achieve by overriding the CopyCells and CopyCellRow method in GridCutCopyPaste class.

```C#
dataGrid.GridCopyPaste = new CustomCopyPaste(dataGrid as SfDataGrid); 

public class CustomCopyPaste : GridCutCopyPaste 
{ 
 
    public CustomCopyPaste(SfDataGrid sfGrid) : base(sfGrid) 
    { 
    }    
 
    protected override void CopyCells(GridSelectedCellsCollection selectedCells, StringBuilder text) 
    { 
        base.CopyCells(selectedCells, text); 
        text.Insert(0, "\t"); 
    } 
 
    protected override void CopyCellRow(GridSelectedCellsInfo row, ref StringBuilder text) 
    { 
        base.CopyCellRow(row, ref text); 
        var DataTable = (dataGrid.ItemsSource as DataTable); 
        int index = DataTable.Rows.IndexOf((row.RowData as DataRowView).Row as System.Data.DataRow) + 1; 
        string value = "Row_" + index + "\t"; 
        text.Insert(0, value); 
    } 
}   
```

## Requirements to run the demo
Visual Studio 2015 and above versions
