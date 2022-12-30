$("#table-1").dataTable({
  "columnDefs": [
        { "sortable": false, "targets": [3] }
  ]
});
$("#table-2").dataTable({
  "columnDefs": [
    { "sortable": true, "targets": [0,1,2] }
  ]
});
