taikun = taikun || {};

(function (taikun) {
  taikun.SelectedDatabaseView = Backbone.View.extend({
    template: Handlebars.compile($('#selected-database-template').html()),
    
    tableTemplate: Handlebars.compile($('#table-template').html()),
    
    initialize: function () {
      this.selectedTableView = new taikun.SelectedTableView();

      this.on('databaseSelected', this.selectDatabase, this);
    },

    selectDatabase: function () {
      if (this.model.get("TablesLoaded") === false) {
        console.log("Fetching database information for " + this.model.get("Name"));
        this.model.getTables(this.render, this);
      } else {
        console.log("Tables already loaded, rendering them");
        this.render();
      }
    },

    render: function () {
      var databaseViewModel = {
        "Name": this.model.get("Name"),
        "Description": this.model.get("Description") 
      };
      this.$el.html(this.template(databaseViewModel));
      var that = this;
      var tables = {
        Tables: _.map(this.model.get("Tables").models, function (table) { return { DatabaseName: that.model.get("Name"), TableName: table.get("Name") }; })
      };
      $("#tables").html(this.tableTemplate(tables));
      $("#columns").empty();
    }
  });
})(taikun);