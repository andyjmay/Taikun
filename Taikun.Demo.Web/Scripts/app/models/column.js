var taikun = taikun || {};

(function(taikun) {
  taikun.Column = Backbone.Model.extend({
    default: {
      ColumnName: '',
      DataType: '',
      MaxLength: ''
    }
  });
})(taikun);