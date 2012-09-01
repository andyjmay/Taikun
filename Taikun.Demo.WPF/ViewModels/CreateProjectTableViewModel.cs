using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Taikun.Demo.WPF.Models;

namespace Taikun.Demo.WPF.ViewModels {
  public class CreateProjectTableViewModel : ViewModelBase {
    public ObservableCollection<SqlServerTableColumn> ProjectColumns { get; private set; }

    public RelayCommand AddColumn { get; private set; }

    public CreateProjectTableViewModel(IProjectManager projectManager) {
      if (!IsInDesignMode) {
        ProjectColumns = new ObservableCollection<SqlServerTableColumn>();
      } else {
        ProjectColumns = new ObservableCollection<SqlServerTableColumn> {
          new SqlServerTableColumn {
            Name = "ID",
            Type = "int"
          },
          new SqlServerTableColumn {
            Name = "FirstName",
            Type = "string",
            Length = 255
          },
          new SqlServerTableColumn {
            Name = "LastName",
            Type = "string",
            Length = 255
          },
          new SqlServerTableColumn {
            Name = "State",
            Type = "string",
            Length = 2
          },
          new SqlServerTableColumn {
            Name = "Description",
            Type = "string",
            Length = int.MaxValue
          }
        };
      }

      AddColumn = new RelayCommand(() => {
        ProjectColumns.Add(new SqlServerTableColumn());
      });
    }    
  }
}
