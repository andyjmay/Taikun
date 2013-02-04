taikun = taikun || {};

(function (taikun) {
  taikun.Router = Backbone.Router.extend({
    routes: {
      "databases/:name": "setDatabase",
      "databases/:databaseName/:tableName": "setTable"
    },
    
    setDatabase: function(name) {
      taikun.Databases.trigger('selected', name);
    },
    
    setTable: function(databaseName, tableName) {
      console.log('Selecting ' + databaseName + ' ' + tableName);
      taikun.App.trigger('tableSelected', { databaseName: databaseName, tableName: tableName });
    }
  });
})(taikun);