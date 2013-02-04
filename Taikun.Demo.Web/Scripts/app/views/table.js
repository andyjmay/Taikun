taikun = taikun || {};

(function (taikun) {
  taikun.TableView = Backbone.View.extend({
    tagName: 'li',

    template: Handlebars.compile($('#table-template').html()),

    render: function () {
      this.$el.html(this.template(this.model.toJSON()));
      return this;
    }
  });
})(taikun);