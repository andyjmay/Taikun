taikun = taikun || {};

(function (taikun) {
  taikun.SelectedDatabaseView = Backbone.View.extend({
    template: Handlebars.compile($('#selected-database-template').html()),

    initialize: function() {
      this.on('databaseSelected', this.selectDatabase, this);
    },

    selectDatabase: function () {
      if (this.model.get("TablesLoaded") === false) {
        console.log("Fetching database information for " + name);
        this.model.getTables(this.render, this);
      } else {
        console.log("Tables already loaded, rendering them");
        this.render();
      }
    },

    render: function () {
      var databaseViewModel = {
        "Name": this.model.get("Name"),
        "Tables": _.map(this.model.get("Tables").models, function (table) { return table.get("Name"); }),
        "Description": this.model.get("Description") 
      };
      this.$el.html(this.template(databaseViewModel));
    }
  });
})(taikun);