var taikun = taikun || {};

(function(taikun) {
  var Databases = Backbone.Collection.extend({
    url: '/api/databases',
    
    model: taikun.Database
  });

  taikun.Databases = new Databases();
})(taikun);