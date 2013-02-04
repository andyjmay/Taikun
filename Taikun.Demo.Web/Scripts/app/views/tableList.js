taikun = taikun || {};

(function(taikun) {
  taikun.TableListView = Backbone.View.extend({
    initialize: function() {
      this.collection = taikun.Tables;
      this.collection.on('selected', this.selectTable, this);
      this.collection.on('add', this.renderTable, this);
    },
    
    render: function() {
      var that = this;
      _.each(this.collection.models, function(table) {
        that.renderTable(table);
      }, this);
    },

    renderTable: function(table) {
      var tableView = new taikun.TableView({
        model: table
      });
      this.$el.append(tableView.render().el);
    },

    selectTable: function(table) {
      console.log('selected table ' + table);
      
    }
  });
})(taikun);