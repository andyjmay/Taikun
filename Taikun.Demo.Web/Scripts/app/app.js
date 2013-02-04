var taikun = taikun || {};

(function (taikun) {
  var app = Backbone.View.extend({
    _selectedDatabaseView: {},

    initialize: function() {
      new taikun.DatabaseListView({ el: $("#databases") });
      _selectedDatabaseView = new taikun.SelectedDatabaseView({ el: $("#selectedDatabase") });
      this.on('databaseListLoaded', this.init, this);
      this.on('databaseSelected', this.setSelectedDatabase, this);
      this.on('tableSelected', this.setSelectedTable, this);
    },

    init: function () {
      console.log("Initializing Router");
      new taikun.Router();
      Backbone.history.start();
    },

    setSelectedDatabase: function(database) {
      console.log("Selecting database: " + database.get("Name"));
      _selectedDatabaseView.model = database;
      _selectedDatabaseView.trigger('databaseSelected', database);
    },

    setSelectedTable: function(selectedTable) {
      console.log("Selecting table: " + selectedTable.tableName);
      if (_selectedDatabaseView.model === undefined) {
        taikun.Databases.trigger('selected', selectedTable.databaseName);
      }
      _selectedDatabaseView.selectedTableView.selectTable(selectedTable.databaseName, selectedTable.tableName);
    }
  });
  taikun.App = new app();
})(taikun);