$(async () => {
  const client = new Oidc.OidcClient({
    authority: 'http://localhost:5000/',
    client_id: 'spa',
    redirect_uri: 'http://localhost:5002/callback.html',
    response_type: 'id_token token',
    scope: 'openid profile api1'
  });
  const res = await client.processSigninResponse();
  $('#loading-profile').attr('hidden', '');
  $('#saving').removeAttr('hidden');
  $('#name').text(', ' + res.profile.given_name);

  const ajaxResp = await $.post({
    url: 'http://localhost:5001/api/save',
    headers: { 'Authorization': 'Bearer ' + res.access_token},
    data: res.state,
    contentType: 'application/json'
  }).promise();

  $('#saving').attr('hidden', '');
  $('#success').removeAttr('hidden');
});
