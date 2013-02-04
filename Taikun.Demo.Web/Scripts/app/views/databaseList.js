taikun = taikun || {};

(function (taikun) {
  taikun.DatabaseListView = Backbone.View.extend({
    initialize: function () {
      var that = this;
      this.collection = taikun.Databases;
      this.collection.fetch({
        success: function () {
          console.log("database list loaded");
          that.render();
          taikun.App.trigger('databaseListLoaded');
        }
      });
      this.collection.on('selected', this.selectDatabase, this);
      this.collection.on('add', this.renderDatabase, this);
    },

    events:  {
      "click" : "handleClick"
    },
    
    handleClick: function(e) {
      var i = 0;
    },

    render: function () {
      var that = this;
      _.each(this.collection.models, function(database) {
        that.renderDatabase(database);
      }, this);
    },
    
    renderDatabase: function(database) {
      var databaseView = new taikun.DatabaseView({
        model: database,
        className: database.get('Name')
      });
      this.$el.append(databaseView.render().el);
    },
    
    selectDatabase: function (name) {
      this.$el.find('.active').removeClass('active');
      this.$el.find('.' + name).addClass('active');
      taikun.App.trigger('databaseSelected', _.first(this.collection.where({ Name: name })));
    }
  });
})(taikun);