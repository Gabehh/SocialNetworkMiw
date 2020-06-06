function WriteComment(post, comment) {
    var url = '/Porfile/WriteComment';
    fetch(url, {
        method: 'POST',
        body: JSON.stringify({ IdPost: post, Comment: comment }),
        headers: {
            'Content-Type': 'application/json'
        }
    }).then(function (response) {
        if (response.status == 200) {
            response.json().then(function (data) {
                if (data.isValid) {
                    var date = new Date(data.comment.dateTime)
                    $("#" + post).append('<label class="control-label">' + data.comment.user +
                        ' (' + (date.toLocaleString()) + ') said: ' + data.comment.description + ' </label><br />');
                    $("." + post).val("");
                }
                else {
                    alert("error");
                }
            });
        }
    })
    .catch(function (err) {
        console.log("error:"+err);
    });
}