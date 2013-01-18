var taikun = taikun || {};

(function (taikun) {
  taikun.Tables = Backbone.Collection.extend({
    initialize: function() {
      console.log("Tables initialized");
    },

    url: function() {
      return '/api/tables/' + this.databaseName;
    },

    model: taikun.Table,
    
    setDatabaseName: function(name) {
      this.databaseName = name;
    }
  });
})(taikun);