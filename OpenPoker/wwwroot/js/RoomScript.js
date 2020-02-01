setInterval(function () {
    $('#cards').load('/Room/Data?' + $.param({roomId : 1}) );
}, 500);