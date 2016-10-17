$(document).ready(function () {
    $('#btnGo').click(getContacts);

    var hashIndex = document.location.href.indexOf('#');
    if (hashIndex > 0) {
        var fragment = document.location.href.substring(hashIndex + 1);
        var accessToken = null;

        var keyValuePairs = fragment.split('&');
        for (var i = 0; i < keyValuePairs.length; i++) {
            var keyValue = keyValuePairs[i].split('=');
            var key = decodeURIComponent(keyValue[0]);
            if (key == 'access_token') {
                var value = keyValue[1];
                accessToken = decodeURIComponent(value);
                break;
            }
        };

        if (accessToken) {
            $.support.cors = true; // Allows cross-domain requests in case of IE

            $.ajax({
                type: 'GET',
                url: 'http://www.my-contacts.com/contacts/api/contacts',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                headers: { 'Authorization': 'Bearer ' + accessToken },
                success: function (data) {
                    $('#result').show();
                    $.each(data, function (i, contact) {
                        $('#contacts').append($('<tr>')
                            .append($('<td>')
                                .append($('<input>')
                                    .attr('type', 'checkbox')
                                )
                            )
                            .append($('<td>')
                                .text(contact.Name)
                            )
                            .append($('<td>')
                                .text(contact.Email)
                            )
                        );
                    });
                }
            });
        }
    }
});

function getContacts(evt) {
    var url = 'http://www.my-contacts.com/contacts/OAuth20';
    url = url + '?client_id=0123456789';
    url = url + '&scope=Read.Contacts'; // hard-coded scope for illustration only
    url = url + '&redirect_uri=' + encodeURIComponent('http://www.my-promo.com/promo');
    url = url + '&response_type=token';

    document.location = url;
};