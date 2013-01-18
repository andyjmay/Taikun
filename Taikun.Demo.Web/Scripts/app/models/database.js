var taikun = taikun || {};

(function(taikun) {
  taikun.Database = Backbone.Model.extend({
    defaults: {
      Name: "",
      Description: "",
      TablesLoaded: false
    },
    
    initialize: function () {
      this.set({
        Tables: new taikun.Tables()
      });
      this.get("Tables").setDatabaseName(this.get("Name"));
      console.log("Database model initialized");
    },
    
    getTables: function (callback, context) {
      var _this = this;
      this.get("Tables").fetch({
        success: function () {
          _this.set("TablesLoaded", true);
          if (callback !== undefined) {
            callback.call(context);
          }
        }
      });
    }
  });
})(taikun);