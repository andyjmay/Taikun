var taikun = taikun || {};

(function (taikun) {
  var app = Backbone.View.extend({
    _selectedDatabaseView: {},
    
    initialize: function() {
      new taikun.DatabaseListView({ el: $("#databases") });
      _selectedDatabaseView = new taikun.SelectedDatabaseView({ el: $("#selectedDatabase") });
      this.on('databaseListLoaded', this.init, this);
      this.on('databaseSelected', this.setSelectedDatabase, this);
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
    }
  });
  taikun.App = new app();
})(taikun);