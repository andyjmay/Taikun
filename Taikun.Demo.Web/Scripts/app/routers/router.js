taikun = taikun || {};

(function (taikun) {
  taikun.Router = Backbone.Router.extend({
    routes: {
      "databases/:name": "setDatabase"
    },
    
    setDatabase: function(name) {
      taikun.Databases.trigger('selected', name);
    }
  });
})(taikun);