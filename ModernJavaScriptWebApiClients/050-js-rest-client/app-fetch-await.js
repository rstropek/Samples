const pokemonList = document.getElementById('pokemons');

// Read more about fetch API at https://developer.mozilla.org/en-US/docs/Web/API/WindowOrWorkerGlobalScope/fetch

(async function() {
    const response = await fetch('https://pokeapi.co/api/v2/pokemon/');
    const pokelist = await response.json();

    let html = '';
    for(const pokemon of pokelist.results) {
        html += `<li>${pokemon.name}</li>`
    }

    pokemonList.innerHTML = html;
})();

(async function() {
    const response = await fetch('https://httpbin.org/post', {
        method: 'POST',
        headers: {
            'Authorization': 'Basic YWRtaW46YWRtaW4=',
            'Content-Type': 'application/json',
            'X-Demo': 'Foo'
        },
        body: JSON.stringify({ 'foo': 'bar' })
    });

    console.log(await response.json());
})();