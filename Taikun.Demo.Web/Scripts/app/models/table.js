var taikun = taikun || {};

(function (taikun) {
  taikun.Table = Backbone.Model.extend({
    defaults: {
      Name: ''
    },
    
    initialize: function() {
      this.set({
        Columns: new taikun.Columns()
      });
      this.get("Columns").setDatabaseName(this.get("DatabaseName"));
      this.get("Columns").setTableName(this.get("Name"));
    },
    
    getColumns: function(callback, context) {
      var _this = this;
      this.get("Columns").fetch({
        success: function() {
          _this.set("ColumnsLoaded", true);
          if (callback !== undefined) {
            callback.call(context);
          }
        }
      });
    }
  });
})(taikun);