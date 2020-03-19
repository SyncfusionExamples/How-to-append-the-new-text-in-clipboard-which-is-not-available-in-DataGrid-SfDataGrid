using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FunctionCommand actionCommand;

        public FunctionCommand ActionCommand
        {
            get
            {
                if (this.actionCommand == null)
                {
                    actionCommand = new FunctionCommand("ActionCommand", typeof(MainWindow));
                }
                return actionCommand;
            }
        }
        public ObservableCollection<string> RowHeaders
        {
            get
            {
                return (ObservableCollection<string>)GetValue(RowHeadersProperty);
            }
            set
            {
                SetValue(RowHeadersProperty, value);
            }
        }
        public DataTable DataTable
        {
            get
            {
                return (DataTable)GetValue(DataTableProperty);
            }
            set
            {
                SetValue(DataTableProperty, value);
            }
        }
        

        public static readonly DependencyProperty DataTableProperty = DependencyProperty.Register("DataTable", typeof(DataTable), typeof(MainWindow), new PropertyMetadata(null));        
        public static readonly DependencyProperty RowHeadersProperty = DependencyProperty.Register("RowHeaders", typeof(ObservableCollection<string>), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            DataTable = GetTable(false);
            CommandBindings.Add(new CommandBinding(this.ActionCommand, this.ActionCommandExecuted));
            InitializeComponent();
            dataGrid.AutoGeneratingColumn += MainWindow_AutoGeneratingColumn;
            dataGrid.FilterItemsPopulating += MainWindow_FilterItemsPopulating;
            dataGrid.GridCopyPaste = new CustomCopyPaste(dataGrid as SfDataGrid);
        }
        private void ActionCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (DataTable != null && RowHeaders != null)
            {
                int index = DataTable.Rows.IndexOf(e.Parameter as System.Data.DataRow);
                object returnValue = "";
                if (index > -1 && index < RowHeaders.Count)
                {
                    returnValue = RowHeaders[index];
                }
                ((FunctionCommand)e.Command).ReturnValue = returnValue;
            }
        }
        private void MainWindow_FilterItemsPopulating(object sender, GridFilterItemsPopulatingEventArgs e)
        {
            if (!(e.Column is GridNumericColumn))
                e.FilterControl.FilterMode = FilterMode.AdvancedFilter;
        }

        private void MainWindow_AutoGeneratingColumn(object sender, AutoGeneratingColumnArgs e)
        {
            if (e.Column.MappingName == "Date Data")
            {
                e.Column = new GridDateTimeColumn()
                {
                    MappingName = "Date Data",
                    Pattern = Syncfusion.Windows.Shared.DateTimePattern.FullDateTime,
                    DateTimeFormat = new System.Globalization.DateTimeFormatInfo() { FullDateTimePattern = "dd/MM/yyyy HH:mm:ss" }
                };
            }
            if (e.Column is GridNumericColumn)
            {
                Style style = new Style(typeof(GridFilterControl));
                Setter setter = new Setter(GridFilterControl.FilterModeProperty, FilterMode.Both);
                style.Setters.Add(setter);
                e.Column.FilterPopupStyle = style;
            }
        }

        public System.Data.DataTable GetTable(bool isDateTime)
        {
            ObservableCollection<string> headers = new ObservableCollection<string>();
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("Custom Data", typeof(string));
            table.Columns.Add("String Data", typeof(int));
            table.Columns.Add("Int Data", typeof(string));
            table.Columns.Add("Date Data", isDateTime ? typeof(DateTime) : typeof(string));
            table.Columns.Add("Double Data", typeof(double));

            double density = 0d;

            for (int i = 0; i < 20; i++)
            {
                switch (i % 5)
                {
                    case 1: density = 0.123d; break;
                    case 2: density = 5.565e+16; break;
                    case 3: density = 123.32423; break;
                    case 4: density = 8.333333333333e+14; break;
                    default: density = 0d; break;
                }
                headers.Add("Row_" + (i + 1));
                table.Rows.Add(((i + 10) % 10 + "," + (i + 20) % 10), (11 + i), (100 - i).ToString(), new DateTime(2009 + i % 10, (i % 12) + 1, (i % 30) + 1, (i % 24), (i % 60), (i % 60)), density);
            }
            RowHeaders = headers;
            return table;
        }

        private void IsDateTime_Checked(object sender, RoutedEventArgs e)
        {
            DataTable = GetTable((sender as CheckBox).IsChecked.Value);
        }

    }

    public class GridRowHeaderTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && (values as Object[]).Count() == 3)
            {
                var command = values[1] as FunctionCommand;
                var cell = values.Last() as GridCell;
                command?.Execute(values.First(), cell);
                return command?.ReturnValue;
            }
            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FunctionCommand : RoutedCommand
    {
        private object returnValue;
        public FunctionCommand(string Name, Type OnerType) : base(Name, OnerType)
        {
        }
        public object ReturnValue
        {
            get
            {
                return returnValue;
            }
            set
            {
                returnValue = value;
            }
        }
    }

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
}
