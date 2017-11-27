const pokemonList = document.getElementById('pokemons');

(async function() {
    const response = await fetch('https://pokeapi.co/api/v2/pokemon/');
    const pokelist = await response.json();

    let html = '';
    for(const pokemon of pokelist.results) {
        html += `<li>${pokemon.name}</li>`
    }

    pokemonList.innerHTML = html;
})();