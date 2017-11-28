const pokemonList = document.getElementById('pokemons');

(function() {
// Build XHR. Need details about XMLHttpRequest? Check
// https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest

const xhr = new XMLHttpRequest();
xhr.addEventListener('load', () => {
  // Parse result
  const jsonResult = JSON.parse(xhr.response);

  // Iterate over result and print Pokemon results
  jsonResult.results.forEach(pokemon => console.log(pokemon.name));
});

// Build URL
xhr.open('GET', 'https://pokeapi.co/api/v2/pokemon/');
xhr.send();

})();
