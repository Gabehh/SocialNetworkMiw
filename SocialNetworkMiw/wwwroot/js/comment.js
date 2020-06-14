function WriteComment(post, comment) {
    var url = '/Home/WriteComment';
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
                    $("#" + post).append('<div id="' + data.comment.id + '"><img onclick="DeleteComment(\''+data.postId+'\',\''+data.comment.id +'\',\''+data.comment.userId+'\')" style="width:20px;height:20px;" src="/images/icons/trash.png" /> <label class="control-label"> ' + data.comment.userName +
                        ' (' + (date.toLocaleString()) + ') said: ' + data.comment.description + ' </label><br /></div>');
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


function DeleteComment(post, comment, user) {
    var url = '/Home/DeleteComment';
    fetch(url, {
        method: 'POST',
        body: JSON.stringify({ PostId: post, CommentId: comment, UserId: user }),
        headers: {
            'Content-Type': 'application/json'
        }
    }).then(function (response) {
        if (response.status == 200) {
            response.json().then(function (data) {
                if (data.isValid) {
                    $("#" + comment).remove();
                }
                else {
                    alert("error");
                }
            });
        }
    })
    .catch(function (err) {
        console.log("error:" + err);
    });
}