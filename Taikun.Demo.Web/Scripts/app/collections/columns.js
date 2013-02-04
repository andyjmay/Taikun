var taikun = taikun || {};

(function(taikun) {
  taikun.Columns = Backbone.Collection.extend({
    initialize: function() {
      console.log("Columns initialized");
    },

    url: function() {
      return '/api/tables/' + this.databaseName + '/' + this.tableName;
    },

    model: taikun.Column,

    setDatabaseName: function(name) {
      this.databaseName = name;
    },

    setTableName: function(name) {
      this.tableName = name;
    }
  });
})(taikun);