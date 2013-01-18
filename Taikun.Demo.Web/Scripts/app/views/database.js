taikun = taikun || {};

(function (taikun) {
  taikun.DatabaseView = Backbone.View.extend({
    tagName: 'li',

    template: Handlebars.compile($('#database-template').html()),
    
    render: function () {
      this.$el.html(this.template(this.model.toJSON()));
      return this;
    }
  });
})(taikun);