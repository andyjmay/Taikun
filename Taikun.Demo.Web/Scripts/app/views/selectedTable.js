taikun = taikun || {};

(function(taikun) {
  taikun.SelectedTableView = Backbone.View.extend({
    template: Handlebars.compile($('#selected-table-template').html()),
    
    initialize: function () {
      this.collection = new taikun.Columns();
      this.on('tableSelected', this.selectTable, this);
    },
    
    selectTable: function(databaseName, tableName) {
      console.log("Selecting table " + tableName + " in database " + databaseName);
      this.collection.setDatabaseName(databaseName);
      this.collection.setTableName(tableName);
      var that = this;
      this.collection.fetch({
        success: function() {
          that.render();
        }
      });
    },
    
    render: function() {
      console.log("Rendering selected table");
      //$("#selectedTable").html(this.template({
      //  Name: this.collection.tableName,
      //  PrimaryKeys: this.collection.
      //}));
      $("#selectedTable").html(this.template({ Columns: this.collection.toJSON() }));
    }
  });
})(taikun);   